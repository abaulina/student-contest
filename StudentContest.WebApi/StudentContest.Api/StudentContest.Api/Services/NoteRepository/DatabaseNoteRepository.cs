using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;

namespace StudentContest.Api.Services.NoteRepository
{
    public class DatabaseNoteRepository:INoteRepository
    {
        private readonly ApplicationContext _applicationContext;

        public DatabaseNoteRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<Note?> GetNoteAsync(int id)
        {
            var note = await _applicationContext.Notes.FindAsync(id);
            if (note == null)
                throw new KeyNotFoundException();
            return note;
        }

        public async Task<Note> AddNoteAsync(Note note)
        {
            _applicationContext.Notes.Add(note);
            await _applicationContext.SaveChangesAsync();
            return note;
        }

        public async Task DeleteNoteAsync(Note note)
        {
            _applicationContext.Notes.Remove(note);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            return await _applicationContext.Notes.ToListAsync();
        }

        public async Task<IEnumerable<Note>> GetPublicNotesAsync()
        {
            return await _applicationContext.Notes.Where(n => n.Status==NoteStatus.Public).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }
    }
}
