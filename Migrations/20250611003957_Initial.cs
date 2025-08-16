using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TypicalTechTools.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductPrice = table.Column<double>(type: "float", nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "Password", "Role", "UserName" },
                values: new object[] { 1, "$2a$11$mBVWNIeL3o8VCqJlOUy0euC1n2qf0xlTX5R87uM/26VCD.IxX/Wh6", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "ProductDescription", "ProductName", "ProductPrice", "UpdatedDate" },
                values: new object[,]
                {
                    { 12345, " bluetooth headphones with fair battery life and a 1 month warranty", " Generic Headphones", 84.989999999999995, new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4499) },
                    { 12346, " bluetooth headphones with good battery life and a 6 month warranty", " Expensive Headphones", 149.99000000000001, new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4501) },
                    { 12347, " bluetooth headphones with good battery life and a 12 month warranty", " Name Brand Headphones", 199.99000000000001, new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4502) },
                    { 12348, " simple bluetooth pointing device", " Generic Wireless Mouse", 39.990000000000002, new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4503) },
                    { 12349, " mouse and keyboard wired combination", " Logitach Mouse and Keyboard", 73.989999999999995, new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4503) },
                    { 12350, " quality wireless mouse", " Logitach Wireless Mouse", 149.99000000000001, new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4504) }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "CommentId", "CommentText", "CreatedDate", "ProductId", "SessionId" },
                values: new object[,]
                {
                    { 1, "This is a great product. Highly Recommended!", new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4405), 12345, "session-001" },
                    { 2, "Not worth the excessive price. Stick with a cheaper generic one.", new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4478), 12350, "session-002" },
                    { 3, "A great budget buy. As good as some of the expensive alternatives.", new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4486), 12345, "session-003" },
                    { 4, "Total garbage. Never buying this brand again. ", new DateTime(2025, 6, 11, 10, 39, 56, 301, DateTimeKind.Local).AddTicks(4491), 12347, "session-004" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProductId",
                table: "Comments",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
