using System.Data.SqlClient;
using Dapper; 

namespace MatchFolio_Profile.Model
{
    public class UserReposProfile
    {
        private readonly IConfiguration? _configuration;

        public UserReposProfile(IConfiguration? configuration)
        {
            _configuration = configuration;
        }

        public async Task<UserEntityProfile> ExistingUser(UserEntityProfile user)
        {
            var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));

            return await oSqlConnection.QuerySingleOrDefaultAsync<UserEntityProfile>(
                   "SELECT * FROM Users WHERE email = @Email", new { user.email });
        }
    }
}
