using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pluralize.NET.Core;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Data
{
    public class Bulk
    {
        public IConfiguration Configuration { get; }
        private readonly string _os;
        private readonly string _conn;
        private readonly ApplicationDbContext _context;
        public Bulk(IConfiguration configuration,
            ApplicationDbContext context)
        {
            _context = context;
            _os = Environment.OSVersion.Platform.ToString();
            Configuration = configuration;
            _conn = Configuration.GetConnectionString($"{_os}Connection");
        }
        public async Task AddProcedure()
        {
            string query = "select * from sysobjects where type='P' and name='BulkInsert'";
            var sp = @"CREATE PROCEDURE BulkInsert(@TableName NVARCHAR(50), @Tsv NVARCHAR(100))
AS
BEGIN 
DECLARE @SQLSelectQuery NVARCHAR(MAX)=''
SET @SQLSelectQuery = 'BULK INSERT ' + @TableName + ' FROM ' + QUOTENAME(@Tsv) + ' WITH (DATAFILETYPE=' + QUOTENAME('widechar') + ')'
  exec(@SQLSelectQuery)
END";
            bool spExists = false;
            using (SqlConnection connection = new SqlConnection(_conn))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;
                    //command.Parameters.Add(
                    //    "@dbo", SqlDbType.NChar).Value = $"";
                    //command.Parameters.Add(
                    //    "@file", SqlDbType.NChar).Value = $"'{Path.Combine(path, $"{name}.tsv")}'";
                    //command.CommandType = CommandType.Text;
                    //command.CommandTimeout = 0;
                    //command.Parameters.Add("@tsv", SqlDbType.NChar).Value = Path.Combine(path, $"{name}.tsv");
                    //command.CommandText = 
                    //    $"BULK INSERT dbo.{name} FROM @tsv WITH (DATAFILETYPE='widechar')";
                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while(await reader.ReadAsync().ConfigureAwait(false))
                        {
                            spExists = true;
                            break;
                        }
                    }
                    if (!spExists)
                    {
                        command.CommandText = sp;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                spExists = true;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
            }
        }
        public async Task Insert<TSource>(string path)
        {
            var name = new Pluralizer().Pluralize(typeof(TSource).ToString().Split(".").Last());
            await _context.Database
                .ExecuteSqlCommandAsync("BulkInsert @p0, @p1", parameters: new[] { $"dbo.{name}", 
                    Path.Combine(path, $"{name}.tsv") })
                .ConfigureAwait(false);
            return;
        }
        //public static bool IsDBGenerated<TSource>(Expression<Func<TSource, object>> propertyExpression)
        //{
        //    var property = GetPropertyInfo(propertyExpression);
        //    var attribute = property.GetCustomAttribute<InsertOffAttribute>();
        //    return attribute != null;
        //}
        //public static PropertyInfo GetPropertyInfo<TSource>(Expression<Func<TSource, object>> propertyLambda)
        //{
        //    if (!(propertyLambda?.Body is MemberExpression member))
        //    {
        //        var ubody = (UnaryExpression)propertyLambda.Body;
        //        member = ubody.Operand as MemberExpression;
        //    }
        //    return member.Member as PropertyInfo;
        //}
    }
}
