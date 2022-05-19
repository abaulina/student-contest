using Microsoft.AspNetCore.JsonPatch;
using StudentContest.Api.Models;
using StudentContest.Api.Services.NoteRepository;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Services
{
    public interface INoteService
    {
        Task<Note> Add(Note note);
        Task Edit(int id, JsonPatchDocument<Note> notePatchDocument);
        Task Delete(int id);
        Task<IEnumerable<Note>> GetAllNotes();
        Task<IEnumerable<Note>> GetPublicNotes();
        Task ChangeReadStatus(int id, int userId);
        Task<Note> GetNoteAsync(int id);
    }


    public class NoteService: INoteService
    {
        private readonly INoteValidator _noteValidator;
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteValidator noteValidator, INoteRepository noteRepository)
        {
            _noteValidator = noteValidator;
            _noteRepository = noteRepository;
        }

        public async Task<Note> Add(Note note)
        {
            _noteValidator.ValidateNote(note);
            return await _noteRepository.AddNoteAsync(note);
        }

        public async Task Edit(int id, JsonPatchDocument<Note> notePatchDocument)
        {
            var existingNote = await GetNoteAsync(id);
            notePatchDocument.ApplyToSafely(existingNote, _noteValidator);
            await _noteRepository.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var existingNote = await GetNoteAsync(id);
            await _noteRepository.DeleteNoteAsync(existingNote);
        }

        public async Task<IEnumerable<Note>> GetAllNotes()
        {
            return await _noteRepository.GetAllNotesAsync();
        }

        public async Task<IEnumerable<Note>> GetPublicNotes()
        {
            return await _noteRepository.GetPublicNotesAsync();
        }

        public async Task ChangeReadStatus(int id, int userId)
        {
            var existingNote = await GetNoteAsync(id);
            var userNote = existingNote.UserNotes.Find(un => un.UserId == userId && un.NoteId == id);
            if (userNote == null)
                throw new KeyNotFoundException();
            userNote.IsRead = !userNote.IsRead;
            await _noteRepository.SaveChangesAsync();
        }

        public async Task<Note> GetNoteAsync(int id)
        {
            var existingNote = await _noteRepository.GetNoteAsync(id);
            if (existingNote == null)
                throw new KeyNotFoundException();
            return existingNote;
        }
    }
}
