using System;
using lhm.net;

namespace lhm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Lhm.Setup("Server=(localdb)\\v11.0;Initial Catalog=Lhm.Test;Integrated Security=True");

            Lhm.ChangeTable("User", migrator =>
            {
                migrator.RenameColumn("Telephone", "ContactNo");
            });

            Console.ReadLine();
        }
    }
}