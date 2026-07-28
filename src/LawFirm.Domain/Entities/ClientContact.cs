namespace LawFirm.Domain.Entities;

public class ClientContact
{
    private ClientContact() { }

    public int Id { get; private set; }
    public int ClientId { get; private set; }

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public string Name { get; set; } = string.Empty;
    public string RelationshipType { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Notes { get; set; }

    public bool IsActive { get; private set; } = true;

    public static ClientContact Create(int clientId, string name, string relationshipType,
        string? email, string? phone, string? company, string? notes) => new()
    {
        ClientId = clientId,
        Name = name,
        RelationshipType = relationshipType,
        Email = email,
        Phone = phone,
        Company = company,
        Notes = notes
    };

    public void Deactivate() => IsActive = false;
}
