using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class AddedCommentAndLikeAndMapFeatureTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "capsule_comments",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    capsule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_capsule_comments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_capsule_comments_capsules_capsule_id",
                        column: x => x.capsule_id,
                        principalSchema: "unseal",
                        principalTable: "capsules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_capsule_comments_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "capsule_likes",
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
                    table.PrimaryKey("p_k_capsule_likes", x => x.id);
                    table.ForeignKey(
                        name: "f_k_capsule_likes_capsules_capsule_id",
                        column: x => x.capsule_id,
                        principalSchema: "unseal",
                        principalTable: "capsules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_capsule_likes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "capsule_map_features",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    capsule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    geom = table.Column<Geometry>(type: "geometry", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_capsule_map_features", x => x.id);
                    table.ForeignKey(
                        name: "f_k_capsule_map_features_capsules_capsule_id",
                        column: x => x.capsule_id,
                        principalSchema: "unseal",
                        principalTable: "capsules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_capsule_comments_capsule_id",
                schema: "unseal",
                table: "capsule_comments",
                column: "capsule_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsule_comments_user_id",
                schema: "unseal",
                table: "capsule_comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsule_likes_capsule_id",
                schema: "unseal",
                table: "capsule_likes",
                column: "capsule_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsule_likes_user_id",
                schema: "unseal",
                table: "capsule_likes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsule_map_features_capsule_id",
                schema: "unseal",
                table: "capsule_map_features",
                column: "capsule_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "capsule_comments",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "capsule_likes",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "capsule_map_features",
                schema: "unseal");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
