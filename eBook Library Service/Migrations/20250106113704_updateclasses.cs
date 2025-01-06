using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eBook_Library_Service.Migrations
{
    /// <inheritdoc />
    public partial class updateclasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "EpubFilePath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "F2bFilePath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Formats",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobiFilePath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PdfFilePath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpubFilePath",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "F2bFilePath",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Formats",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "MobiFilePath",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PdfFilePath",
                table: "Books");

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { 1, "Author1" },
                    { 2, "Author2" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "AgeLimit", "BorrowPrice", "BuyPrice", "Category", "Description", "DiscountEndDate", "DiscountPrice", "ImageUrl", "Publisher", "Stock", "Title", "YearPublished" },
                values: new object[,]
                {
                    { 1, "18+", 5.99m, 15.99m, "Fiction", "A novel written by American author F. Scott Fitzgerald. It is a critique of the American Dream in the 1920s.", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 12.99m, "https://via.placeholder.com/150", "Scribner", 3, "The Great Gatsby", 1925 },
                    { 2, "16+", 4.99m, 12.99m, "Science Fiction", "A dystopian social science fiction novel and cautionary tale, written by the English writer George Orwell.", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 10.99m, "https://via.placeholder.com/150", "Secker & Warburg", 3, "1984", 1949 },
                    { 3, "12+", 6.99m, 14.99m, "Classic", "A novel by Harper Lee published in 1960. It was immediately successful, winning the Pulitzer Prize for Fiction in 1961.", new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 11.99m, "https://via.placeholder.com/150", "J.B. Lippincott & Co.", 3, "To Kill a Mockingbird", 1960 }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 2, 2 },
                    { 1, 3 }
                });
        }
    }
}
