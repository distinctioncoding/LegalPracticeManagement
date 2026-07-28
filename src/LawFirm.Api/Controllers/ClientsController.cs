using LawFirm.Application.Features.Clients;
using LawFirm.Application.Features.Clients.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LawFirm.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientsService;

    public ClientsController(IClientService clients)
    {
        _clientsService = clients;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ClientListRequest request)
    {
        var result = await _clientsService.ListAsync(request);
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = nameof(GetClient))]
    public async Task<IActionResult> GetClient(int id)
    {
        var result = await _clientsService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        var result = await _clientsService.CreateAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetClient), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientRequest request)
    {
        var result = await _clientsService.UpdateAsync(id, request);
        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.Error!.Code == "not_found")
            return NotFound(result.Error);
        if (result.Error.Code == "conflict")
            return Conflict(result.Error);
        return BadRequest(result.Error);
    }

    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var result = await _clientsService.ArchiveAsync(id);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpPost("{id:int}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var result = await _clientsService.RestoreAsync(id);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpPost("{id:int}/contacts")]
    public async Task<IActionResult> AddContact(int id, [FromBody] CreateClientContactRequest request)
    {
        var result = await _clientsService.AddContactAsync(id, request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetClient), new { id }, result.Value)
            : NotFound(result.Error);
    }

    [HttpDelete("{id:int}/contacts/{contactId:int}")]
    public async Task<IActionResult> DeactivateContact(int id, int contactId)
    {
        var result = await _clientsService.DeactivateContactAsync(id, contactId);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
