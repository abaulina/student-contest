using System.ComponentModel;
using StudentContest.Api.ExceptionMiddleware;
using StudentContest.Api.Models;
using StudentContest.Api.Services.NoteRepository;

namespace StudentContest.Api.Validation
{
    public interface INoteValidator
    {
        Task ValidateNote(Note? note);
        Task ValidateId(int id);
        public void ValidateStatus(object status);
        public void ValidateText(string text);
    }

    public class NoteValidator: INoteValidator
    {
        private readonly INoteRepository _noteRepository;

        public NoteValidator(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }
        
        public async Task ValidateNote(Note? note)
        {
            await ValidateId(note.Id);
            ValidateText(note.Text);
            ValidateStatus(note.Status);
        }

        public async Task ValidateId(int id)
        {
            var note = await _noteRepository.FindByIdAsync(id);
            if (note != null)
                throw new ApiException("The same id already exists!");
        }
        
        public void ValidateText(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException();
        }

        public void ValidateStatus(object status)
        {
            var isValid = status switch
            {
                NoteStatus _ => Enum.IsDefined(typeof(NoteStatus), status),
                string s => Enum.IsDefined(typeof(NoteStatus), (s)),
                _ => false
            };
            if(!isValid)
                throw new InvalidEnumArgumentException();
        }
    }
}
