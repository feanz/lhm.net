using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using FastMember;
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

            Lhm.ChangeTable("Staff", migrator =>
            {
                migrator.AddColumn("IsSuspended", "bit");
            });

            Lhm.ChangeTable("Staff", migrator =>
            {
                migrator.AddColumn("DateOfBirth", "DateTime2");
            });

            Lhm.ChangeTable("Staff", migrator =>
            {
                migrator.RemoveColumn("DateOfBirth");
            });

            Lhm.ChangeTable("Staff", migrator =>
            {
                migrator.AddIndex("Email", true, new IndexDef("Email", IndexOrder.DESC));
            });

            Lhm.ChangeTable("Staff", migrator =>
            {
                migrator.AddIndex("FirstNameLastname", false, new IndexDef("FirstName", IndexOrder.ASC), new IndexDef("LastName", IndexOrder.DESC));
            });

            Lhm.ChangeTable("Staff", migrator =>
            {
                migrator.RemoveIndex("Email");
            });

            Console.ReadLine();
        }

        private static void SetupSampleDatabase(string connectionString, int numberOfUsers = 50000)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var setup = File.ReadAllText(Path.Combine(directoryName, "SetupSampleDatabase.sql"));
            var tearDown = File.ReadAllText(Path.Combine(directoryName, "TearDownSampleDatabase.sql"));

            var users = new List<User>();
            for (var i = 0; i < numberOfUsers; i++)
            {
                users.Add(CreateUser());
            }

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                con.Execute(tearDown);
                con.Execute(setup);

                using (var bulkCopy = new SqlBulkCopy(con))
                {
                    var table = new DataTable();
                    using (var reader = ObjectReader.Create(users))
                    {
                        table.Load(reader);
                    }

                    bulkCopy.ColumnMappings.Add("Username", "Username");
                    bulkCopy.ColumnMappings.Add("Email", "Email");
                    bulkCopy.ColumnMappings.Add("FirstName", "FirstName");
                    bulkCopy.ColumnMappings.Add("LastName", "LastName");
                    bulkCopy.ColumnMappings.Add("Telephone", "Telephone");
                    bulkCopy.ColumnMappings.Add("DepartmentID", "DepartmentID");
                    bulkCopy.ColumnMappings.Add("PositionID", "PositionID");

                    bulkCopy.DestinationTableName = "Staff";
                    bulkCopy.BatchSize = 10000;
                    bulkCopy.WriteToServer(table);
                }
            }
        }

        private static User CreateUser()
        {
            var newUserGuid = Guid.NewGuid();
            return new User
            {
                Username = "Username" + newUserGuid,
                FirstName = "Firstname" + newUserGuid,
                LastName = "LastName" + newUserGuid,
                Email = "Email" + newUserGuid + "@email.com",
                Telephone = "Telephone" + newUserGuid,
                DepartmentID = Helper.RandomNumber(1, 3),
                PositionID = Helper.RandomNumber(1, 3)
            };
        }

        private static class Helper
        {
            private static readonly Random Random = new Random();

            public static int RandomNumber(int min, int max)
            {
                return Random.Next(min, max + 1);
            }
        }
    }
}
