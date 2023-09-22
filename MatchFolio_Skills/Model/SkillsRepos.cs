using Dapper;
using MatchFolio_Skills.Model;
using System.Data.SqlClient;

namespace MatchFolio_Skills.Repository
{
    public class SkillsRepos
    {
        private readonly IConfiguration _configuration;

        public SkillsRepos(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<SkillEntity>> GetSkillsByPortfolioIdAsync(int portfolioId)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration.GetConnectionString("SQL"));
                return await oSqlConnection.QueryAsync<SkillEntity>("SELECT * FROM Skills WHERE PortfolioId = @PortfolioId", new { PortfolioId = portfolioId });
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la récupération des compétences.", ex);
            }
        }

        public async Task<SkillEntity> AddSkillAsync(SkillEntity skill)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration.GetConnectionString("SQL"));
                var insertSkill = await oSqlConnection.QueryFirstOrDefaultAsync<SkillEntity>("INSERT INTO Skills (Name, Level, PortfolioId) VALUES (@Name, @Level, @PortfolioId);", skill);
                return insertSkill;
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de l'ajout de la compétence.", ex);
            }
        }

        public async Task UpdateSkillAsync(SkillEntity skill)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration.GetConnectionString("SQL"));
                var query = "UPDATE Skills SET Name = @Name, Level = @Level WHERE Id = @Id";
                await oSqlConnection.ExecuteAsync(query, skill);
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la mise à jour de la compétence.", ex);
            }
        }

        public async Task DeleteSkillAsync(int skillId)
        {
            try
            {
                using var oSqlConnection = new SqlConnection(_configuration.GetConnectionString("SQL"));
                var query = "DELETE FROM Skills WHERE Id = @Id";
                await oSqlConnection.ExecuteAsync(query, new { Id = skillId });
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la suppression de la compétence.", ex);
            }
        }
    }
}
