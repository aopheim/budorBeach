using Microsoft.EntityFrameworkCore.Migrations;

namespace rpiDaemon.Migrations
{
    public partial class AddTaxonomySpeciesId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EBirdTaxonomyId",
                table: "SpeciesRecognitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EBirdTaxonomyId",
                table: "SpeciesRecognitions");
        }
    }
}
