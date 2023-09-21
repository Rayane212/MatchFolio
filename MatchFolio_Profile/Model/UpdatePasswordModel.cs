namespace MatchFolio_Profile.Model
{
    public class UpdatePasswordModel
    {
        public required string oldPassword { get; set; }
        public required string newPassword { get; set; }
        public required string confirmPassword { get; set; }
    }
}
