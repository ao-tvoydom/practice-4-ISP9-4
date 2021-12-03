using FluentMigrator;

namespace Infrastructure.Migration
{
    public class InitialMigration : FluentMigrator.Migration
    {
        public override void Up()
        {
            this.Execute.EmbeddedScript("UpScript.sql");
        }

        public override void Down()
        {
            this.Execute.EmbeddedScript("DownScript.sql");
        }
    }
}