using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pract4.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfIssue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCards_Students_Id",
                        column: x => x.Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Email", "FirstName", "Phone", "SecondName" },
                values: new object[,]
                {
                    { 1, "johnDoe@gmail.com", "John", "123456789", "Doe" },
                    { 2, "janeSmith@gmail.com", "Emily", "987654321", "Smith" },
                    { 3, "maryJohnson@gmail.com", "Mary", "555555555", "Johnson" }
                });

            migrationBuilder.InsertData(
                table: "StudentCards",
                columns: new[] { "Id", "DateOfIssue", "IdNumber", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 9, 16, 16, 53, 15, 177, DateTimeKind.Local).AddTicks(559), "A12345", true },
                    { 2, new DateTime(2023, 9, 16, 16, 53, 15, 177, DateTimeKind.Local).AddTicks(595), "B54321", true },
                    { 3, new DateTime(2023, 9, 16, 16, 53, 15, 177, DateTimeKind.Local).AddTicks(597), "C67890", false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCards");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
