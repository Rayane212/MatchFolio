namespace MatchFolio_Authentication.Model
{
    public class SignInModel
    {
        public string? Mail { get; set; }
        public string? Username { get; set; }
        public required string Password { get; set; }
    }
}
