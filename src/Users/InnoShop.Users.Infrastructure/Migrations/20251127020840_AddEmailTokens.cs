using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnoShop.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "email_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(88)", maxLength: 88, nullable: false),
                    Purpose = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_tokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_email_tokens_TokenHash_Purpose",
                table: "email_tokens",
                columns: new[] { "TokenHash", "Purpose" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_tokens_UserId",
                table: "email_tokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_tokens");
        }
    }
}
