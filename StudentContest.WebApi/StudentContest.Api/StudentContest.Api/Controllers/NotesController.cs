using Microsoft.AspNetCore.Mvc;

namespace StudentContest.Api.Controllers
{
    [Route("notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"value"};
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void AddNote([FromBody] string value)
        {
        }

        [HttpPatch("{id}")]
        public void EditNote(int id, [FromBody] string value)
        {
        }

        [HttpPatch("{id}")]
        public void MakePublic(int id)
        {
        }

        [HttpDelete("{id}")]
        public void DeleteNote(int id)
        {
        }
    }
}
