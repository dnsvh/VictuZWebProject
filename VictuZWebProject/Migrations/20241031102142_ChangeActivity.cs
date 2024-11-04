using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictuZWebProject.Migrations
{
    /// <inheritdoc />
    public partial class ChangeActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembersOnlyCapacity",
                table: "Activity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "MembersOnlyVisibilityEnd",
                table: "Activity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MembersPreRegistration",
                table: "Activity",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnlyMembers",
                table: "Activity",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembersOnlyCapacity",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "MembersOnlyVisibilityEnd",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "MembersPreRegistration",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "OnlyMembers",
                table: "Activity");
        }
    }
}
