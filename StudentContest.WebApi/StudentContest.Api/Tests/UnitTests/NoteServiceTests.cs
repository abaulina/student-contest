using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using StudentContest.Api.Models;
using StudentContest.Api.Services;
using StudentContest.Api.Services.NoteRepository;
using StudentContest.Api.Validation;
using Xunit;

namespace StudentContest.Api.Tests.UnitTests
{
    public class NoteServiceTests
    {
        private readonly INoteValidator _noteValidator;
        private readonly INoteRepository _noteRepository;
        private readonly ApplicationContext _context;

        public NoteServiceTests()
        {
            _context = new DatabaseFake().GetContext();
            _noteRepository = new DatabaseNoteRepository(_context);
            _noteValidator = new NoteValidator(_noteRepository);
        }

        [Fact]
        public async Task Add_Success_ChangesDb()
        {
            var note = new Note {Status = NoteStatus.Public, Text = "Note"};
            var noteService = new NoteService(_noteValidator, _noteRepository);
            var count = _context.Notes.Count();

            await noteService.Add(note);

            Assert.Equal(count+1, _context.Notes.Count());
            Assert.Equal("Note", _context.Notes.Last()?.Text);
        }

        [Fact]
        public async Task Edit_Success_ChangesDb()
        {
            var jsonPatchDoc = new JsonPatchDocument<Note>().Replace(n=> n.Status, NoteStatus.Public);
            var noteService = new NoteService(_noteValidator, _noteRepository);

            await noteService.Edit(1, jsonPatchDoc);

            Assert.Equal(NoteStatus.Public, _context.Notes.First(n=>n.Id==1)?.Status);
        }

        [Fact]
        public async Task Delete_Success_ChangesDb()
        {
            var noteService = new NoteService(_noteValidator, _noteRepository);
            var count = _context.Notes.Count();

            await noteService.Delete(1);

            Assert.Equal(count - 1, _context.Notes.Count());
            Assert.Null(_context.Notes.FirstOrDefault(n => n.Id == 1));
        }

        [Fact]
        public async Task ChangeReadStatus_Success_ChangesDb()
        {
            var noteService = new NoteService(_noteValidator, _noteRepository);

            await noteService.ChangeReadStatus(1, 2);

            Assert.Equal(true,
                _context.Notes.FirstOrDefault(n => n.Id == 1)?.UserNotes
                    .FirstOrDefault(un => un.NoteId == 1 && un.UserId == 2)?.IsRead);
        }

        [Fact]
        public async Task Edit_InvalidId_Exception()
        {
            var noteService = new NoteService(_noteValidator, _noteRepository);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await noteService.Edit(-1, new JsonPatchDocument<Note>()));
        }

        [Fact]
        public async Task Delete_InvalidId_Exception()
        {
            var noteService = new NoteService(_noteValidator, _noteRepository);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await noteService.Delete(-1));
        }

        [Fact]
        public async Task ChangeReadStatus_InvalidId_Exception()
        {
            var noteService = new NoteService(_noteValidator, _noteRepository);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await noteService.ChangeReadStatus(-1,0));
        }

        [Fact]
        public async Task ChangeReadStatus_NoUserNote_Exception()
        {
            var noteService = new NoteService(_noteValidator, _noteRepository);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await noteService.ChangeReadStatus(1, -1));
        }
    }
}
