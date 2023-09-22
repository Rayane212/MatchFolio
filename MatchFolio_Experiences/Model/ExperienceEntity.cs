namespace MatchFolio_Skills.Model
{
    public class ExperienceEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string JobTitle { get; set; }
        public required string Company { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
