using LawFirm.Application.Features.Clients.Dtos;
using LawFirm.Domain.Entities;

namespace LawFirm.Application.Features.Clients;

public static class ClientMappings
{
    public static ClientListItemDto ToListItem(this Client c) => new(
        c.Id,
        c.ClientCode,
        c.DisplayName,
        c.ClientType,
        c.Email,
        c.Phone,
        c.Status,
        c.CreatedAt);

    public static ClientDetailDto ToDetail(this Client c) => new(
        c.Id,
        c.ClientCode,
        c.DisplayName,
        c.ClientType,
        c.Status,
        c.FirstName,
        c.LastName,
        c.PreferredName,
        c.DateOfBirth,
        c.OrganizationName,
        c.TradingName,
        c.CompanyNumber,
        c.Email,
        c.Phone,
        new AddressDto(c.AddressLine1, c.AddressLine2, c.City, c.State, c.Postcode, c.Country),
        c.InternalNotesSummary,
        c.IsArchived,
        c.CreatedAt,
        c.CreatedBy,
        c.UpdatedAt,
        c.UpdatedBy,
        c.Contacts
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Name)
            .Select(x => x.ToDto())
            .ToList());

    public static ClientContactDto ToDto(this ClientContact x) => new(
        x.Id,
        x.Name,
        x.RelationshipType,
        x.Email,
        x.Phone,
        x.Company,
        x.Notes,
        x.IsActive);

    public static void ApplyCommonFields(this Client c, AddressDto? address, string? email, string? phone,
        string? firstName, string? lastName, string? preferredName, DateOnly? dob,
        string? organizationName, string? tradingName, string? companyNumber, string? internalNotes)
    {
        c.Email = email;
        c.Phone = phone;
        c.FirstName = firstName;
        c.LastName = lastName;
        c.PreferredName = preferredName;
        c.DateOfBirth = dob;
        c.OrganizationName = organizationName;
        c.TradingName = tradingName;
        c.CompanyNumber = companyNumber;
        c.InternalNotesSummary = internalNotes;

        c.AddressLine1 = address?.AddressLine1;
        c.AddressLine2 = address?.AddressLine2;
        c.City = address?.City;
        c.State = address?.State;
        c.Postcode = address?.Postcode;
        c.Country = address?.Country;
    }
}
