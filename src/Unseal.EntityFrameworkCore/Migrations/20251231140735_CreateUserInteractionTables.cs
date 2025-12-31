using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserInteractionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_blocked",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropColumn(
                name: "is_muted",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.CreateTable(
                name: "user_interactions",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_muted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_interactions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_interactions_users_source_user_id",
                        column: x => x.source_user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_user_interactions_users_target_user_id",
                        column: x => x.target_user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_interactions_source_user_id",
                schema: "unseal",
                table: "user_interactions",
                column: "source_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_interactions_target_user_id",
                schema: "unseal",
                table: "user_interactions",
                column: "target_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_interactions",
                schema: "unseal");

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                schema: "unseal",
                table: "user_followers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_muted",
                schema: "unseal",
                table: "user_followers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
