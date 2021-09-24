using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace masterInfrastructure.Helper
{
    public static class AdoQuery
    {
        public static async Task<JArray> AdoSqlQueryAsync(this DatabaseFacade dbDatabase, string cn, string parseSqlQuery)
        {
            var jArrayR = new JArray();

            using (var connection = new SqlConnection(cn))
            {
                var command = new SqlCommand(parseSqlQuery, connection) { CommandTimeout = 0 };
                connection.Open();

                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var jObjectR = new JObject();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        bool tryBool;
                        if (bool.TryParse(reader[i].ToString(), out tryBool))
                        {
                            jObjectR.Add(reader.GetName(i), tryBool);
                        }
                        else
                        {
                            jObjectR.Add(reader.GetName(i), reader[i].ToString());
                        }

                    }
                    jArrayR.Add(jObjectR);
                }
                connection.Close();
            }
            return jArrayR;
        }
    }
}
