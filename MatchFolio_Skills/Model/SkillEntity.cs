namespace MatchFolio_Skills.Model
{
    public class SkillEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }
        public int Level { get; set; }
        public int CategoryId { get; set; } 
    }
}
