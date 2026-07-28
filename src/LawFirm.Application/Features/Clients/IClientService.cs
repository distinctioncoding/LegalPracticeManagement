using LawFirm.Application.Features.Clients.Dtos;
using LawFirm.Shared.Results;

namespace LawFirm.Application.Features.Clients;

public interface IClientService
{
    Task<List<ClientListItemDto>> ListAsync(ClientListRequest request);

    Task<Result<ClientDetailDto>> GetByIdAsync(int id);

    Task<Result<ClientDetailDto>> CreateAsync(CreateClientRequest request);

    Task<Result<ClientDetailDto>> UpdateAsync(int id, UpdateClientRequest request);

    Task<Result> ArchiveAsync(int id);

    Task<Result> RestoreAsync(int id);

    Task<Result<ClientContactDto>> AddContactAsync(int clientId, CreateClientContactRequest request);

    Task<Result> DeactivateContactAsync(int clientId, int contactId);
}
