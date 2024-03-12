using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rpiDaemon.Migrations
{
    public partial class AddRecordingUploadedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RecordingUploadedAt",
                table: "SpeciesRecognitions",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordingUploadedAt",
                table: "SpeciesRecognitions");
        }
    }
}
