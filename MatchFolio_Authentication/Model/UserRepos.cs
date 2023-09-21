using System.Data.SqlClient;
using Dapper; 

namespace MatchFolio_Authentication.Model
{
    public class UserRepos
    {
        private readonly IConfiguration? _configuration;

        public UserRepos(IConfiguration? configuration)
        {
            _configuration = configuration;
        }

        public async Task<UserEntity?> GetUserAuthAsync(string? mail, string? username, string password)
        {
            var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));

            var user =  await oSqlConnection.QuerySingleOrDefaultAsync<UserEntity>(
                "SELECT * FROM Users WHERE email = @email OR username = @Username", 
                new { Email = mail, Username = username });
            
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password))
            {
                return user;
            }

            return null;
        }

        public async Task<UserEntity> InsertUserAsync(UserEntity user, string userId)
        {
            var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));

            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);

            var insertUser = await oSqlConnection.QueryFirstOrDefaultAsync<UserEntity>(
                "INSERT INTO Users (username, password, firstName, lastName, birthday, email, phoneNumber, profilePicture ,userType, cvLink, linkedinLink, XLink, githubLink) " +
                "VALUES (@Username, @Password, @FirstName, @LastName, @Birthday, @Email, @PhoneNumber, @ProfilePicture, @UserType, @CvLink, @LinkedinLink, @XLink, @GithubLink)",
                new { user.username, user.password, user.firstName, user.lastName, user.birthday,user.email, user.phoneNumber, user.profilePicture, user.userType ,user.cvLink, user.linkedinLink, user.XLink, user.githubLink ,id = userId });
            return insertUser;
        }

        public async Task<UserEntity> ExistingUser(UserEntity user)
        {
            var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));

            return await oSqlConnection.QuerySingleOrDefaultAsync<UserEntity>(
                   "SELECT * FROM Users WHERE email = @Email", new { user.email });
        }
    }
}
