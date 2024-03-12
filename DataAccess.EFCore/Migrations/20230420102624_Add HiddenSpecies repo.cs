using Microsoft.EntityFrameworkCore.Migrations;

namespace rpiDaemon.Migrations
{
    public partial class AddHiddenSpeciesrepo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HiddenSpecies",
                columns: table => new
                {
                    TaxonomySpeciesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiddenSpecies", x => x.TaxonomySpeciesId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HiddenSpecies");
        }
    }
}
