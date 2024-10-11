using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictuZWebProject.Migrations.VictuZ_Lars_DbMigrations
{
    /// <inheritdoc />
    public partial class UpdateActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MemberVisibilityEnd",
                table: "Activity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MemberVisibilityStart",
                table: "Activity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MembersOnlyRegistration",
                table: "Activity",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnlyVisibleMembers",
                table: "Activity",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberVisibilityEnd",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "MemberVisibilityStart",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "MembersOnlyRegistration",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "OnlyVisibleMembers",
                table: "Activity");
        }
    }
}
