using StudentContest.Api.Models;

namespace StudentContest.Api.Services.NoteRepository
{
    public interface INoteRepository
    {
        Task<Note?> GetNoteAsync(int id);
        Task<Note?> FindByIdAsync(int id);
        Task<Note?> AddNoteAsync(Note? note);
        Task DeleteNoteAsync(Note? note);
        Task<IEnumerable<Note?>> GetAllNotesAsync();
        Task<IEnumerable<Note?>> GetPublicNotesAsync();
        Task SaveChangesAsync();
    }
}
