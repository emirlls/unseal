using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserViewTrackingTypeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "capsule_id",
                schema: "unseal",
                table: "user_view_trackings",
                newName: "external_id");

            migrationBuilder.AddColumn<Guid>(
                name: "user_view_tracking_type_id",
                schema: "unseal",
                table: "user_view_trackings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_view_tracking_types",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_view_tracking_types", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "user_view_tracking_types",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { new Guid("0a08cad2-e210-43a7-99ce-513a752d422e"), 0, "Capsule" },
                    { new Guid("4ac149d8-d61a-4efe-9a6c-81c6c0520fba"), 1, "UserProfile" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_view_trackings_user_view_tracking_type_id",
                schema: "unseal",
                table: "user_view_trackings",
                column: "user_view_tracking_type_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_user_view_trackings_user_view_tracking_types_user_view_trac~",
                schema: "unseal",
                table: "user_view_trackings",
                column: "user_view_tracking_type_id",
                principalSchema: "unseal",
                principalTable: "user_view_tracking_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_user_view_trackings_user_view_tracking_types_user_view_trac~",
                schema: "unseal",
                table: "user_view_trackings");

            migrationBuilder.DropTable(
                name: "user_view_tracking_types",
                schema: "unseal");

            migrationBuilder.DropIndex(
                name: "IX_user_view_trackings_user_view_tracking_type_id",
                schema: "unseal",
                table: "user_view_trackings");

            migrationBuilder.DropColumn(
                name: "user_view_tracking_type_id",
                schema: "unseal",
                table: "user_view_trackings");

            migrationBuilder.RenameColumn(
                name: "external_id",
                schema: "unseal",
                table: "user_view_trackings",
                newName: "capsule_id");
        }
    }
}
