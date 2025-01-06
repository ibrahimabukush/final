using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eBook_Library_Service.Migrations
{
    /// <inheritdoc />
    public partial class u : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1,
                column: "ImageUrl",
                value: "images/BookDefult.png");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2,
                column: "ImageUrl",
                value: "images/BookDefult.png");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 3,
                column: "ImageUrl",
                value: "images/BookDefult.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://via.placeholder.com/150");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://via.placeholder.com/150");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://via.placeholder.com/150");
        }
    }
}
