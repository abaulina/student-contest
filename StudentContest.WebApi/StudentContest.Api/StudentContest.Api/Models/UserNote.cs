namespace StudentContest.Api.Models
{
    public class UserNote
    {
        public int NoteId { get; set; }
        public int UserId { get; set; }
        public bool IsRead { get; set; }

        public User User { get; private set; }
        public Note Note { get; private set; }
    }
}
