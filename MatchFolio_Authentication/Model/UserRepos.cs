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

        public async Task<UserEntity> GetUserAuthAsync(string? mail, string? username, string password)
        {
            var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));

            return await oSqlConnection.QuerySingleOrDefaultAsync<UserEntity>(
                "SELECT * FROM Users WHERE MailUser = @MailUser OR UserName = @Username AND PasswordUser = @PasswordUser", new { MailUser = mail, Username = username, PasswordUser = password });
        }

        public async Task<UserEntity> InsertUserAsync(UserEntity user, string userId)
        {
            var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));
            var insertUser = await oSqlConnection.QueryFirstOrDefaultAsync<UserEntity>(
                "INSERT INTO Users (NameUser, LastNameUser, Username, MailUser, PasswordUser, CodeUser) " +
                "VALUES (@NameUser, @LastNameUser, @Username, @MailUser, @PasswordUser, @CodeUser)",
                new { user.NameUser, user.LastNameUser, user.Username, user.MailUser, user.PasswordUser, CodeUser = userId });
            return insertUser;
        }

        public async Task<UserEntity> ExistingUser(UserEntity user)
        {
            var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));

            return await oSqlConnection.QuerySingleOrDefaultAsync<UserEntity>(
                   "SELECT * FROM Users WHERE MailUser = @MailUser OR Username = @Username", new { user.MailUser, user.Username });
        }
    }
}
