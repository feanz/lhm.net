using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using Serilog;

namespace lhm.net.sample
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=(localdb)\\v11.0;;Initial Catalog=Lhm.Test;Integrated Security=True";

            Log.Logger = new LoggerConfiguration()
             .WriteTo
             .LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
             .CreateLogger();

            Lhm.Setup(connectionString);

            Lhm.CleanUp(true);

            SetupSampleDatabase(connectionString);

            Lhm.ChangeTable("Position", migrator =>
            {
                migrator.RenameColumn("Name", "Type");
            });

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

            Lhm.ChangeTable("User", migrator =>
            {
                migrator.AddIndex("Email", true, new IndexDef("Email", IndexOrder.DESC));
            });

            Lhm.ChangeTable("User", migrator =>
            {
                migrator.AddIndex("FirstNameLastname", false, new IndexDef("FirstName", IndexOrder.ASC), new IndexDef("LastName", IndexOrder.DESC));
            });

            Lhm.ChangeTable("User", migrator =>
            {
                migrator.RemoveIndex("Email");
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
