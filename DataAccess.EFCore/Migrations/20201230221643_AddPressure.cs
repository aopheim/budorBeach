using Microsoft.EntityFrameworkCore.Migrations;

namespace rpiDaemon.Migrations
{
    public partial class AddPressure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PressureInKPa",
                table: "SensorReadings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PressureInKPa",
                table: "SensorReadings");
        }
    }
}
