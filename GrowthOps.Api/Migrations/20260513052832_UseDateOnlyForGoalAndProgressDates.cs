using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowthOps.Api.Migrations
{
    /// <inheritdoc />
    public partial class UseDateOnlyForGoalAndProgressDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDateUtc",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "TargetDateUtc",
                table: "Goals");

            migrationBuilder.RenameColumn(
                name: "LoggedAtUtc",
                table: "ProgressEntries",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LoggedDate",
                table: "ProgressEntries",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Goals",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "TargetDate",
                table: "Goals",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoggedDate",
                table: "ProgressEntries");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "TargetDate",
                table: "Goals");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "ProgressEntries",
                newName: "LoggedAtUtc");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateUtc",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TargetDateUtc",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
