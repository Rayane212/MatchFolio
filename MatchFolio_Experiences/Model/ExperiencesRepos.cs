using System.Data.SqlClient;
using Dapper;

namespace MatchFolio_Experiences.Model
{
    public class ExperiencesRepos
    {
        private readonly IConfiguration? _configuration;

        public ExperiencesRepos(IConfiguration? configuration)
        {
            _configuration = configuration;
        }
    }
}
