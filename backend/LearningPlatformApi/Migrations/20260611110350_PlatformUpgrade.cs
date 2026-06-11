using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearningPlatformApi.Migrations
{
    /// <inheritdoc />
    public partial class PlatformUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromptHistories");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "Id", "CategoryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "C#" },
                    { 2, 1, "JavaScript" },
                    { 3, 1, "Python" },
                    { 4, 2, "Machine Learning" },
                    { 5, 2, "Statistics" },
                    { 6, 2, "Data Visualization" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_CategoryId",
                table: "Prompts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_SubCategoryId",
                table: "Prompts",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Categories_CategoryId",
                table: "Prompts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_SubCategories_SubCategoryId",
                table: "Prompts",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_Categories_CategoryId",
                table: "Prompts");

            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_SubCategories_SubCategoryId",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_CategoryId",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_SubCategoryId",
                table: "Prompts");

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "PromptHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Answer = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptHistories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PromptHistories",
                columns: new[] { "Id", "Answer", "CreatedAt", "Question" },
                values: new object[,]
                {
                    { 1, "C# is a modern, object-oriented programming language.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "What is C#?" },
                    { 2, "Python is a high-level, interpreted programming language.", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "What is Python?" }
                });
        }
    }
}
