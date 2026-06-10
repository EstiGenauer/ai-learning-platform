using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearningPlatformApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPromptHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromptHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: true),
                    Answer = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    { 1, "C# is a modern, object-oriented programming language.", new DateTime(2026, 6, 9, 21, 23, 20, 362, DateTimeKind.Utc).AddTicks(409), "What is C#?" },
                    { 2, "Python is a high-level, interpreted programming language.", new DateTime(2026, 6, 9, 21, 23, 20, 362, DateTimeKind.Utc).AddTicks(1236), "What is Python?" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromptHistories");
        }
    }
}
