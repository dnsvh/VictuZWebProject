using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictuZWebProject.Migrations
{
    /// <inheritdoc />
    public partial class OnlyMembersActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MembersOnlyCapacity",
                table: "Activity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "MembersPriorityCapacity",
                table: "Activity",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembersPriorityCapacity",
                table: "Activity");

            migrationBuilder.AlterColumn<int>(
                name: "MembersOnlyCapacity",
                table: "Activity",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
