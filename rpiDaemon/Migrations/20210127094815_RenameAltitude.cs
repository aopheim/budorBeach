using Microsoft.EntityFrameworkCore.Migrations;

namespace rpiDaemon.Migrations
{
    public partial class RenameAltitude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlititudeInMeters",
                table: "SensorReadings",
                newName: "AltitudeInMeters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AltitudeInMeters",
                table: "SensorReadings",
                newName: "AlititudeInMeters");
        }
    }
}
