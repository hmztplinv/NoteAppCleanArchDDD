using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteApp.Application.Features.Notes.Commands;
using NoteApp.Application.Features.Notes.Queries;
using NoteApp.Application.Wrappers;

namespace NoteApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Tüm controller'a Authorize attribute'u ekledik
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<object>>>> Get()
    {
        var response = await _mediator.Send(new GetAllNotesQuery());
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateNoteCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    // Diğer endpoint'ler eklenecek (Get by Id, Update, Delete, vb.)
}