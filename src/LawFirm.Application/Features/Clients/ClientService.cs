using System.ComponentModel.DataAnnotations;
using LawFirm.Application.Features.Clients.Dtos;
using LawFirm.Domain.Entities;
using LawFirm.Domain.Enums;
using LawFirm.Infrastructure.Persistence;
using LawFirm.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace LawFirm.Application.Features.Clients;

public class ClientService : IClientService
{
    private readonly LawFirmDbContext _db;

    public ClientService(LawFirmDbContext db)
    {
        _db = db;
    }

    public async Task<List<ClientListItemDto>> ListAsync(ClientListRequest request)
    {
        IQueryable<Client> query = _db.Clients.AsNoTracking();

        if (!request.IncludeArchived)
            query = query.Where(c => !c.IsArchived);

        if (request.ClientType is { } type)
            query = query.Where(c => c.ClientType == type);

        if (request.Status is { } status)
            query = query.Where(c => c.Status == status);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(c =>
                (c.FirstName != null && c.FirstName.Contains(keyword)) ||
                (c.LastName != null && c.LastName.Contains(keyword)) ||
                (c.OrganizationName != null && c.OrganizationName.Contains(keyword)) ||
                (c.Email != null && c.Email.Contains(keyword)) ||
                (c.Phone != null && c.Phone.Contains(keyword)) ||
                c.ClientCode.Contains(keyword));
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.ToListItem())
            .ToListAsync();
    }

    public async Task<Result<ClientDetailDto>> GetByIdAsync(int id)
    {
        var client = await _db.Clients
            .AsNoTracking()
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == id);

        return client is null
            ? Error.NotFound("Client", id)
            : client.ToDetail();
    }

    public async Task<Result<ClientDetailDto>> CreateAsync(CreateClientRequest request)
    {
        if (ValidateCreate(request) is { } error)
            return error;

        var clientCode = await GenerateClientCodeAsync();

        var client = request.ClientType == ClientType.Organization
            ? Client.CreateOrganization(clientCode, request.OrganizationName!, request.Status)
            : Client.CreateIndividual(clientCode, request.FirstName!, request.LastName!, request.Status);

        client.ApplyCommonFields(
            request.Address, request.Email, request.Phone,
            request.FirstName, request.LastName, request.PreferredName, request.DateOfBirth,
            request.OrganizationName, request.TradingName, request.CompanyNumber,
            request.InternalNotesSummary);

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        return client.ToDetail();
    }

    public async Task<Result<ClientDetailDto>> UpdateAsync(int id, UpdateClientRequest request)
    {
        if (!Enum.IsDefined(request.Status))
            return Error.Validation("Status is required.");

        if (!IsValidOptionalEmail(request.Email))
            return Error.Validation("Email must be a valid email address.");

        var client = await _db.Clients
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client is null)
            return Error.NotFound("Client", id);

        if (client.IsArchived)
            return Error.Conflict("This client is archived and cannot be edited. Restore it first.");

        if (client.ClientType == ClientType.Individual &&
            (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName)))
        {
            return Error.Validation("First Name and Last Name are required for an individual client.");
        }

        if (client.ClientType == ClientType.Organization && string.IsNullOrWhiteSpace(request.OrganizationName))
        {
            return Error.Validation("Organization Name is required for an organization client.");
        }

        client.Status = request.Status;
        client.ApplyCommonFields(
            request.Address, request.Email, request.Phone,
            request.FirstName, request.LastName, request.PreferredName, request.DateOfBirth,
            request.OrganizationName, request.TradingName, request.CompanyNumber,
            request.InternalNotesSummary);

        await _db.SaveChangesAsync();

        return client.ToDetail();
    }

    public async Task<Result> ArchiveAsync(int id)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.Id == id);
        if (client is null)
            return Result.Failure(Error.NotFound("Client", id));

        client.Archive();
        client.Status = ClientStatus.Archived;
        await _db.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> RestoreAsync(int id)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.Id == id);
        if (client is null)
            return Result.Failure(Error.NotFound("Client", id));

        client.Restore();
        client.Status = ClientStatus.ActiveClient;
        await _db.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<ClientContactDto>> AddContactAsync(int clientId, CreateClientContactRequest request)
    {
        if (ValidateContact(request) is { } error)
            return error;

        var client = await _db.Clients
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == clientId);

        if (client is null)
            return Error.NotFound("Client", clientId);

        var contact = client.AddContact(
            request.Name, request.RelationshipType, request.Email,
            request.Phone, request.Company, request.Notes);

        await _db.SaveChangesAsync();
        return contact.ToDto();
    }

    public async Task<Result> DeactivateContactAsync(int clientId, int contactId)
    {
        var contact = await _db.ClientContacts
            .FirstOrDefaultAsync(x => x.Id == contactId && x.ClientId == clientId);

        if (contact is null)
            return Result.Failure(Error.NotFound("Contact", contactId));

        contact.Deactivate();
        await _db.SaveChangesAsync();
        return Result.Success();
    }

    private static Error? ValidateCreate(CreateClientRequest r)
    {
        if (!Enum.IsDefined(r.ClientType))
            return Error.Validation("Client Type is required.");

        if (!Enum.IsDefined(r.Status))
            return Error.Validation("Status is required.");

        if (r.ClientType == ClientType.Individual &&
            (string.IsNullOrWhiteSpace(r.FirstName) || string.IsNullOrWhiteSpace(r.LastName)))
        {
            return Error.Validation("First Name and Last Name are required for an individual client.");
        }

        if (r.ClientType == ClientType.Organization && string.IsNullOrWhiteSpace(r.OrganizationName))
            return Error.Validation("Organization Name is required for an organization client.");

        if (!IsValidOptionalEmail(r.Email))
            return Error.Validation("Email must be a valid email address.");

        return null;
    }

    private static Error? ValidateContact(CreateClientContactRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Name))
            return Error.Validation("Contact Name is required.");

        if (string.IsNullOrWhiteSpace(r.RelationshipType))
            return Error.Validation("Relationship Type is required.");

        if (!IsValidOptionalEmail(r.Email))
            return Error.Validation("Email must be a valid email address.");

        return null;
    }

    private static bool IsValidOptionalEmail(string? email) =>
        string.IsNullOrWhiteSpace(email) || new EmailAddressAttribute().IsValid(email);

    private async Task<string> GenerateClientCodeAsync()
    {
        var prefix = $"CLI-{DateTimeOffset.UtcNow.Year}-";

        var lastCodeThisYear = await _db.Clients
            .Where(c => c.ClientCode.StartsWith(prefix))
            .OrderByDescending(c => c.ClientCode)
            .Select(c => c.ClientCode)
            .FirstOrDefaultAsync();

        var nextNumber = 1;
        if (lastCodeThisYear is not null &&
            int.TryParse(lastCodeThisYear[prefix.Length..], out var lastNumber))
        {
            nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D6}";
    }
}
