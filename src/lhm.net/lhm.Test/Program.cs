using lhm.net;

namespace lhm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Lhm.Setup("Server=(localdb)\\v11.0;;Initial Catalog=Lhm.Test;Integrated Security=True");

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
        }
    }
}
