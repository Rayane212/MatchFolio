namespace MatchFolio_Profile.Model
{
    public class UserEntityProfile
    {
        public int id { get; set; }
        public required string username { get; set; }
        public required string password { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public DateTime? birthday { get; set; }
        public required string email { get; set; }
        public string? phoneNumber { get; set; }
        public bool? userType { get; set; }
        public string? cvLink { get; set; }
        public string? linkedinLink { get; set; }
        public string? XLink { get; set; }
        public string? githubLink { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
}