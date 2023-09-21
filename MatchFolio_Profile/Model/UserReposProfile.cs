using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MatchFolio_Profile.Model
{
    public class UserReposProfile
    {
        private readonly IConfiguration? _configuration;

        public UserReposProfile(IConfiguration? configuration)
        {
            _configuration = configuration;
        }

        public async Task<UserEntityProfile> ExistingUserByEmail(UserEntityProfile user)
        {
            using var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));
            return await oSqlConnection.QuerySingleOrDefaultAsync<UserEntityProfile>(
                   "SELECT * FROM Users WHERE email = @Email", new { user.email });
        }

        public async Task<UserEntityProfile> ExistingUserByUsername(UserEntityProfile user)
        {
            using var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));
            return await oSqlConnection.QuerySingleOrDefaultAsync<UserEntityProfile>(
                   "SELECT * FROM Users WHERE username = @Username", new { user.username });
        }

        public async Task<UserEntityProfile> GetCurrentUser(string userId)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));
                return await oSqlConnection.QuerySingleOrDefaultAsync<UserEntityProfile>(
                       "SELECT * FROM Users WHERE id = @Id", new { Id = userId });
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la récupération de l'utilisateur.", ex);
            }
        }

        public async Task<bool> UpdateUserProfile(UserEntityProfile user)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));
                var result = await oSqlConnection.ExecuteAsync(
                    "UPDATE Users SET username = @Username, email = @Email, firstName = @FirstName, lastName = @LastName, " +
                    "birthday = @Birthday, phoneNumber = @PhoneNumber, userType = @UserType, cvLink = @CvLink, linkedinLink = @LinkedinLink , " +
                    "XLink = @XLink, githubLink = @GithubLink  WHERE id = @Id", user);
                return result > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la mise à jour de l'utilisateur.", ex);
            }
        }

        public async Task<bool> UpdatePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));
                var user = await oSqlConnection.QuerySingleOrDefaultAsync<UserEntityProfile>(
                       "SELECT * FROM Users WHERE id = @Id", new { Id = userId });

                if (user == null)
                {
                    throw new Exception("Utilisateur non trouvé.");
                }

                if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.password))
                {
                    throw new Exception("L'ancien mot de passe est incorrect.");
                }

                var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                var result = await oSqlConnection.ExecuteAsync(
                    "UPDATE Users SET password = @Password WHERE id = @Id",
                    new { Password = hashedNewPassword, Id = userId });

                return result > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la modification du mot de passe.", ex);
            }
        }


        public async Task<bool> DeleteUserProfile(int userId)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration?.GetConnectionString("SQL"));
                var result = await oSqlConnection.ExecuteAsync(
                    "DELETE FROM Users WHERE id = @Id", new { Id = userId });
                return result > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception("Erreur lors de la suppression de l'utilisateur.", ex);
            }
        }
    }
}
