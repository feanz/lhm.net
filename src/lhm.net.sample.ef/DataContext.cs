using System.Data.Entity;

namespace lhm.net.sample.ef
{
    public class DataContext : DbContext
    {
        public DataContext() : 
            base("Server=(localdb)\\v11.0;;Initial Catalog=Lhm.Test.Ef;Integrated Security=True")
        {}

        public DbSet<Employee> Employees { get; set; }
    }
}
