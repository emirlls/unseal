using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueIndexOnCapsuleLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_capsule_likes_capsule_id",
                schema: "unseal",
                table: "capsule_likes");

            // Keep only the earliest like per (capsule, user) so the unique index can be created.
            migrationBuilder.Sql(
                """
                DELETE FROM unseal.capsule_likes a
                USING unseal.capsule_likes b
                WHERE a.capsule_id = b.capsule_id
                  AND a.user_id = b.user_id
                  AND (a.creation_time, a.id) > (b.creation_time, b.id);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_capsule_likes_capsule_id_user_id",
                schema: "unseal",
                table: "capsule_likes",
                columns: new[] { "capsule_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_capsule_likes_capsule_id_user_id",
                schema: "unseal",
                table: "capsule_likes");

            migrationBuilder.CreateIndex(
                name: "IX_capsule_likes_capsule_id",
                schema: "unseal",
                table: "capsule_likes",
                column: "capsule_id");
        }
    }
}
