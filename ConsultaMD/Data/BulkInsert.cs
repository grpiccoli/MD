using ConsultaMD.Models;
using Pluralize.NET.Core;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsultaMD.Data
{
    public static class BulkInsert
    {
        public static void RunSql<TSource>(
            string path,
            string conn)
        {
            var name = new Pluralizer().Pluralize(typeof(TSource).ToString().Split(".").Last());
            using (SqlConnection connection = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    //command.Parameters.Add(
                    //    "@dbo", SqlDbType.NChar).Value = $"";
                    //command.Parameters.Add(
                    //    "@file", SqlDbType.NChar).Value = $"'{Path.Combine(path, $"{name}.tsv")}'";
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 0;
                    command.CommandText = 
                        $"BULK INSERT dbo.{name} FROM '{Path.Combine(path, $"{name}.tsv")}' WITH (DATAFILETYPE='widechar')";
                    connection.Open();
                    object accountNumber = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public static bool IsDBGenerated<TSource>(Expression<Func<TSource, object>> propertyExpression)
        {
            var property = GetPropertyInfo(propertyExpression);
            var attribute = property.GetCustomAttribute<InsertOffAttribute>();
            return attribute != null;
        }

        public static PropertyInfo GetPropertyInfo<TSource>(Expression<Func<TSource, object>> propertyLambda)
        {
            if (!(propertyLambda?.Body is MemberExpression member))
            {
                var ubody = (UnaryExpression)propertyLambda.Body;
                member = ubody.Operand as MemberExpression;
            }
            return member.Member as PropertyInfo;
        }
    }
}
