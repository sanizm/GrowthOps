using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowthOps.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalDomainFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Goals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalType",
                table: "Goals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncrease",
                table: "Goals",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateUtc",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "StartValue",
                table: "Goals",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TargetDateUtc",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TargetValue",
                table: "Goals",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Goals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Goals",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "GoalType",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "IsIncrease",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "StartDateUtc",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "StartValue",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "TargetDateUtc",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "TargetValue",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Goals");
        }
    }
}
