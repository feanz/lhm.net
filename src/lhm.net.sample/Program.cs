using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using lhm.net;

namespace lhm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=(localdb)\\v11.0;;Initial Catalog=Lhm.Test;Integrated Security=True";

            Lhm.Setup(connectionString);

            Lhm.CleanUp(true);

            SetupSampleDatabase(connectionString);

            Lhm.ChangeTable("User", migrator =>
            {
                migrator.AddColumn("IsSuspended", "bit");
            });

            Lhm.ChangeTable("User", migrator =>
            {
                migrator.AddColumn("DateOfBirth", "DateTime2");
            });

            Lhm.ChangeTable("User", migrator =>
            {
                migrator.RemoveColumn("DateOfBirth");
            });

            Console.ReadLine();
        }

        private static void SetupSampleDatabase(string connectionString)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var sql = File.ReadAllText(Path.Combine(directoryName, "SampleDatabase.sql"));

            using (var con = new SqlConnection(connectionString))
            {
                con.Execute(sql);
            }
        }
    }
}
