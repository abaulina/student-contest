namespace StudentContest.Api.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public NoteStatus Status { get; set; }
        public List<UserNote> UserNotes { get; set; }
    }
}
