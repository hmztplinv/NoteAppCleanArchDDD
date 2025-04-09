using AutoMapper;
using MediatR;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Application.Wrappers;
using NoteApp.Domain.Entities;

namespace NoteApp.Application.Features.Notes.Commands;

// Handler to create a new note
public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, ApiResponse<Guid>>
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateNoteCommandHandler(
        INoteRepository noteRepository, 
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        // Kullanıcı giriş yapmadıysa hata döndür
        if (_currentUserService.UserId == null)
        {
            return new ApiResponse<Guid>("Not oluşturmak için önce giriş yapmalısınız.");
        }

        var newNote = new Note
        {
            Title = request.Title,
            Content = request.Content,
            CategoryId = request.CategoryId,
            UserId = _currentUserService.UserId.Value, // CurrentUserService'den UserId al
            IsPublished = false
        };

        await _noteRepository.AddAsync(newNote);

        return new ApiResponse<Guid>(newNote.Id);
    }
}