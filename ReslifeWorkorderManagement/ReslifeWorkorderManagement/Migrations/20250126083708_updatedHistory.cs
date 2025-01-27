using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReslifeWorkorderManagement.Migrations
{
    /// <inheritdoc />
    public partial class updatedHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "History",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "History");
        }
    }
}
