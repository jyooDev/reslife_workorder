using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReslifeWorkorderManagement.Migrations
{
    /// <inheritdoc />
    public partial class updatedHistory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkorderId",
                table: "History",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_History_WorkorderId",
                table: "History",
                column: "WorkorderId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_WorkOrder_WorkorderId",
                table: "History",
                column: "WorkorderId",
                principalTable: "WorkOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_WorkOrder_WorkorderId",
                table: "History");

            migrationBuilder.DropIndex(
                name: "IX_History_WorkorderId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "WorkorderId",
                table: "History");
        }
    }
}
