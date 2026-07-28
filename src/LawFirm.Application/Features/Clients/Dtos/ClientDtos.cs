using LawFirm.Domain.Enums;

namespace LawFirm.Application.Features.Clients.Dtos;

public class ClientListRequest
{
    public string? Keyword { get; set; }
    public ClientType? ClientType { get; set; }
    public ClientStatus? Status { get; set; }

    public bool IncludeArchived { get; set; }
}

public record ClientListItemDto(
    int Id,
    string ClientCode,
    string DisplayName,
    ClientType ClientType,
    string? Email,
    string? Phone,
    ClientStatus Status,
    DateTimeOffset CreatedAt);

public record ClientDetailDto(
    int Id,
    string ClientCode,
    string DisplayName,
    ClientType ClientType,
    ClientStatus Status,
    string? FirstName,
    string? LastName,
    string? PreferredName,
    DateOnly? DateOfBirth,
    string? OrganizationName,
    string? TradingName,
    string? CompanyNumber,
    string? Email,
    string? Phone,
    AddressDto Address,
    string? InternalNotesSummary,
    bool IsArchived,
    DateTimeOffset CreatedAt,
    string CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy,
    List<ClientContactDto> Contacts);

public record AddressDto(
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? Postcode,
    string? Country);

public record ClientContactDto(
    int Id,
    string Name,
    string RelationshipType,
    string? Email,
    string? Phone,
    string? Company,
    string? Notes,
    bool IsActive);

public record CreateClientRequest(
    ClientType ClientType,
    ClientStatus Status,
    string? FirstName,
    string? LastName,
    string? PreferredName,
    DateOnly? DateOfBirth,
    string? OrganizationName,
    string? TradingName,
    string? CompanyNumber,
    string? Email,
    string? Phone,
    AddressDto? Address,
    string? InternalNotesSummary);

public record UpdateClientRequest(
    ClientStatus Status,
    string? FirstName,
    string? LastName,
    string? PreferredName,
    DateOnly? DateOfBirth,
    string? OrganizationName,
    string? TradingName,
    string? CompanyNumber,
    string? Email,
    string? Phone,
    AddressDto? Address,
    string? InternalNotesSummary);

public record CreateClientContactRequest(
    string Name,
    string RelationshipType,
    string? Email,
    string? Phone,
    string? Company,
    string? Notes);
