using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DapperDemo01
{
    class Program
    {
        public static string ConnString = "packet size=4096;integrated security=SSPI;data source=CLYDEGAO-LAP;initial catalog=DB_Test;Connection Lifetime=15;pooling=true; Min Pool Size=5;Max Pool Size=200; Enlist=false;Application Name=Drivecam.Turnstile;Type System Version=SQL Server 2008";
        static void Main(string[] args)
        {
            Execute();
            //Execute_New();
            var people_01 = GetPersonList(2);
            if (Update("2"))
            {
                Console.WriteLine("Update completed!");
            }

            if (Delete(5))
            {
                Console.WriteLine("Delete completed!");
            }

            TransTest();
            var people = GetPeople(8);
        }

        public static List<Person> GetPersonList( int minId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                var a = conn.Query<Person>("select * from Person where id>@id", new { id = minId });
                conn.Close();
                return a.ToList();
            }
        }

        public static void Execute()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                var r = conn.Execute(@"insert into Person(username, password,age,registerDate,address) values (@a, @b,@c,@d,@e)",
                new[] { 
			    new { a = 14, b = 14, c = 14, d = DateTime.Now, e = 14 }
			    , new { a = 2, b = 2, c = 2, d = DateTime.Now, e = 2 }
			    , new { a = 3, b = 3, c = 3, d = DateTime.Now, e = 3 } 
		        });
                conn.Close();
            }
        }

        //public static void Execute_New()
        //{
        //    string sqlQuery = @"insert into Person(username, password,age,registerDate,address) values (@a, @b,@c,@d,@e)";
        //   var a =  InsertMultiple<Array>(sqlQuery, new[] { 
        //        new  { a = 14, b = 14, c = 14, d = DateTime.Now, e = 14 }
        //        , new  { a = 2, b = 2, c = 2, d = DateTime.Now, e = 2 }
        //        , new  { a = 3, b = 3, c = 3, d = DateTime.Now, e = 3 } 
        //        }, ConnString);
        //}

        public static bool Update(string name)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                var r = conn.Execute(@"update Person set password='www.lanhuseo.com' where username=@username", new { username = name });
                conn.Close();
                return r > 0;
            }
        }

        public static List<Person> GetPeople(int minId)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                return conn.Query<Person>(
                    "[dbo].[GetPeopleList]" ,
                    new { id = minId },
                    commandType:CommandType.StoredProcedure
                    ).ToList();
            }
        }

        public static bool Delete(int deleteNum)
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                var r = conn.Execute(@"delete from Person where id=@id", new { id = deleteNum });
                conn.Close();
                return r > 0;
            }
        }

        public static void TransTest()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                IDbTransaction trans = conn.BeginTransaction();
                int row = conn.Execute(@"update Person set password='www.lanhuseo.com' where id=@id", new { id = 3 }, trans, null, null);
                row += conn.Execute("delete from Person where id=@id", new { id = 6 }, trans, null, null);
                trans.Commit();
            }
        }

        public static int InsertMultiple<T>(string sql, IEnumerable<T> entities, string connectionName = null) where T : class, new()
        {
            using (SqlConnection cnn = GetConnection(connectionName))
            {
                int records = 0;
                using (var trans = cnn.BeginTransaction())
                {
                    try
                    {
                        cnn.Execute(sql, entities, trans, 30, CommandType.Text);
                    }
                    catch (DataException ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    trans.Commit();
                }
                //foreach (T entity in entities)
                //{
                //    records += cnn.Execute(sql, entity);
                //}
                return records;
            }
        }

        public static SqlConnection GetConnection(string name)
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[name].ConnectionString);
        }
    }

    public class Person
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int age { get; set; }
        public DateTime registerDate { get; set; }
        public string address { set; get; }
    }  
}
