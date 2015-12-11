using System.Data.Entity.Migrations;

namespace lhm.net.sample.ef.Migrations
{
    public partial class AddIsSuperUser : DbMigration
    {
        public override void Up()
        {
            Lhm.ChangeTable("Employees", migrator =>
            {
                migrator.AddColumn("IsSuperuser", "bit");
            });
        }
        
        public override void Down()
        {
            Lhm.ChangeTable("Employees", migrator =>
            {
                migrator.RemoveColumn("IsSuperuser");
            });
        }
    }
}
