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
    public class BulkInsert
    {
        public static void RunSql<TSource>(
            string path,
            string conn)
        {
            var name = new Pluralizer().Pluralize(typeof(TSource).ToString().Split(".").Last());
            var bulkinsert = "BULK INSERT dbo.{0} FROM '{1}' WITH (DATAFILETYPE='widechar')";
            var entries = Path.Combine(path, $"{name}.tsv");
            var queryString = string.Format(bulkinsert, name, entries);
            using (SqlConnection connection = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 0;
                    command.Connection.Open();
                    //connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    //SqlDataReader reader = command.ExecuteReader();
                    //try
                    //{
                    //    while (reader.Read())
                    //    {
                    //        Console.WriteLine("{0}", reader.ToString());
                    //    }
                    //}
                    //finally
                    //{
                    //    reader.Close();
                    //}
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
            if (!(propertyLambda.Body is MemberExpression member))
            {
                var ubody = (UnaryExpression)propertyLambda.Body;
                member = ubody.Operand as MemberExpression;
            }
            return member.Member as PropertyInfo;
        }
    }
}
