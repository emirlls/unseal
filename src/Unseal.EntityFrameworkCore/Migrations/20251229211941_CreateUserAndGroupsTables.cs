using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserAndGroupsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    group_image_url = table.Column<string>(type: "text", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_followers",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    follower_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_muted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_followers", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_followers_users_follower_id",
                        column: x => x.follower_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_user_followers_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_locked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    allow_join_group = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    last_activity = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_profiles", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_profiles_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_members",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    join_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_group_members", x => x.id);
                    table.ForeignKey(
                        name: "f_k_group_members_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "unseal",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_group_members_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "notification_event_types",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { new Guid("a9745b1b-3aa1-4a57-b5a6-8a4e07fe778b"), 0, "UserRegister" },
                    { new Guid("d5961c50-522f-4bea-a641-ab88ae983985"), 1, "UserDelete" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_members_group_id",
                schema: "unseal",
                table: "group_members",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_members_user_id",
                schema: "unseal",
                table: "group_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_followers_follower_id",
                schema: "unseal",
                table: "user_followers",
                column: "follower_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_followers_user_id",
                schema: "unseal",
                table: "user_followers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_id",
                schema: "unseal",
                table: "user_profiles",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_members",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "user_followers",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "user_profiles",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "groups",
                schema: "unseal");

            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "notification_event_types",
                keyColumn: "id",
                keyValue: new Guid("a9745b1b-3aa1-4a57-b5a6-8a4e07fe778b"));

            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "notification_event_types",
                keyColumn: "id",
                keyValue: new Guid("d5961c50-522f-4bea-a641-ab88ae983985"));
        }
    }
}
