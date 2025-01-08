using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eBook_Library_Service.Migrations
{
    /// <inheritdoc />
    public partial class no : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Publisher",
                table: "PurchaseHistories",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "YearPublished",
                table: "PurchaseHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearPublished",
                table: "PurchaseHistories");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PurchaseHistories",
                newName: "Publisher");
        }
    }
}
