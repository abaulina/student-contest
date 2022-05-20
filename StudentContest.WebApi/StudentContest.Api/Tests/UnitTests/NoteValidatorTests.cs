using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using StudentContest.Api.ExceptionMiddleware;
using StudentContest.Api.Models;
using StudentContest.Api.Services.NoteRepository;
using StudentContest.Api.Validation;
using Xunit;

namespace StudentContest.Api.Tests.UnitTests
{
    public class NoteValidatorTests
    {
        private readonly NoteValidator _noteValidator;

        public NoteValidatorTests()
        {
            var context = new DatabaseFake().GetContext();
            var databaseNoteRepository = new DatabaseNoteRepository(context);
            _noteValidator = new NoteValidator(databaseNoteRepository);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData(".")]
        [InlineData("public")]
        [InlineData("global")]
        [InlineData("hidden")]
        public void ValidateStatus_InvalidEnum_Exception(object status)
        {
            Assert.Throws<InvalidEnumArgumentException>(() => _noteValidator.ValidateStatus(status));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ValidateText_NullOrEmpty_Exception(string text)
        {
            Assert.Throws<ArgumentNullException>(() => _noteValidator.ValidateText(text));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ValidateId_AlreadyExisting_Exception(int id)
        {
            Assert.ThrowsAsync<ApiException>(() => _noteValidator.ValidateId(id));
        }

        [Fact]
        public async Task ValidateNote_Success()
        {
            var note = new Note {Id = 100, Status = NoteStatus.Public, Text = "New note text", UserNotes = new List<UserNote>()};

            await _noteValidator.ValidateNote(note);
        }
    }
}
