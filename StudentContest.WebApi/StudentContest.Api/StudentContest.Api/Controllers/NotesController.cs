using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StudentContest.Api.Models;
using StudentContest.Api.Services;

namespace StudentContest.Api.Controllers
{
    [Route("notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<Note?> GetNoteById(int id)
        {
            return await _noteService.GetNoteAsync(id);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            var requester = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            return requester switch
            {
                "Admin" => Ok(await _noteService.GetAllNotes()),
                "User" => Ok(await _noteService.GetPublicNotes()),
                _ => Unauthorized()
            };
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] Note? note)
        {
            var createdNote = await _noteService.Add(note);
            return CreatedAtAction(nameof(GetNoteById), new {id = createdNote.Id}, createdNote);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditNote(int id, [FromBody] JsonPatchDocument<Note> notePatchDocument)
        {
            await _noteService.Edit(id, notePatchDocument);
            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpPatch("read/{id:int}")]
        public async Task<IActionResult> ChangeReadStatus(int id)
        {
            var userId = HttpContext.User.FindFirstValue("id");
            if (!int.TryParse(userId, out var uId))
            {
                return Unauthorized();
            }
            await _noteService.ChangeReadStatus(uId, id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            await _noteService.Delete(id);
            return NoContent();
        }
    }
}
