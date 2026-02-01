using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserViewTrackingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_capsules_user_profiles_user_profile_id",
                schema: "unseal",
                table: "capsules");

            migrationBuilder.DropIndex(
                name: "IX_capsules_user_profile_id",
                schema: "unseal",
                table: "capsules");

            migrationBuilder.DropIndex(
                name: "IX_capsule_map_features_capsule_id",
                schema: "unseal",
                table: "capsule_map_features");

            migrationBuilder.DropIndex(
                name: "IX_capsule_items_capsule_id",
                schema: "unseal",
                table: "capsule_items");

            migrationBuilder.DropColumn(
                name: "user_profile_id",
                schema: "unseal",
                table: "capsules");

            migrationBuilder.CreateTable(
                name: "user_view_trackings",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    capsule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_view_trackings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_capsule_map_features_capsule_id",
                schema: "unseal",
                table: "capsule_map_features",
                column: "capsule_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_capsule_items_capsule_id",
                schema: "unseal",
                table: "capsule_items",
                column: "capsule_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_view_trackings",
                schema: "unseal");

            migrationBuilder.DropIndex(
                name: "IX_capsule_map_features_capsule_id",
                schema: "unseal",
                table: "capsule_map_features");

            migrationBuilder.DropIndex(
                name: "IX_capsule_items_capsule_id",
                schema: "unseal",
                table: "capsule_items");

            migrationBuilder.AddColumn<Guid>(
                name: "user_profile_id",
                schema: "unseal",
                table: "capsules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_capsules_user_profile_id",
                schema: "unseal",
                table: "capsules",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsule_map_features_capsule_id",
                schema: "unseal",
                table: "capsule_map_features",
                column: "capsule_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsule_items_capsule_id",
                schema: "unseal",
                table: "capsule_items",
                column: "capsule_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_capsules_user_profiles_user_profile_id",
                schema: "unseal",
                table: "capsules",
                column: "user_profile_id",
                principalSchema: "unseal",
                principalTable: "user_profiles",
                principalColumn: "id");
        }
    }
}
