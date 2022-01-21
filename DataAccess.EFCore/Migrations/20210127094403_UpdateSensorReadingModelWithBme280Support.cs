using Microsoft.EntityFrameworkCore.Migrations;

namespace rpiDaemon.Migrations
{
    public partial class UpdateSensorReadingModelWithBme280Support : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PressureInKPa",
                table: "SensorReadings",
                newName: "RelativeHumidityInPercent");

            migrationBuilder.AddColumn<double>(
                name: "AlititudeInMeters",
                table: "SensorReadings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PressureInhPa",
                table: "SensorReadings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlititudeInMeters",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "PressureInhPa",
                table: "SensorReadings");

            migrationBuilder.RenameColumn(
                name: "RelativeHumidityInPercent",
                table: "SensorReadings",
                newName: "PressureInKPa");
        }
    }
}
