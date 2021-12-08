using FluentMigrator;

namespace Infrastructure.Migration
{
    [Migration(202112081157254)]
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