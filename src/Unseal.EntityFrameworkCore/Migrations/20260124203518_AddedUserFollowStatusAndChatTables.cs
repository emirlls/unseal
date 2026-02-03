using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserFollowStatusAndChatTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "capsule_types",
                keyColumn: "id",
                keyValue: new Guid("6e859f2b-3b3c-44df-9a16-a0c2d5bfc51e"));

            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "capsule_types",
                keyColumn: "id",
                keyValue: new Guid("8bd05f2f-5abc-4e58-aa1b-bd7d7419d243"));

            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "capsule_types",
                keyColumn: "id",
                keyValue: new Guid("fd4f8d92-deb2-493d-a38c-5509b4ad7382"));

            migrationBuilder.DropColumn(
                name: "is_public",
                schema: "unseal",
                table: "capsules");

            migrationBuilder.RenameColumn(
                name: "last_activity",
                schema: "unseal",
                table: "user_profiles",
                newName: "last_activity_time");

            migrationBuilder.AddColumn<string>(
                name: "content",
                schema: "unseal",
                table: "user_profiles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profile_picture_url",
                schema: "unseal",
                table: "user_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "creation_time",
                schema: "unseal",
                table: "user_followers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "creator_id",
                schema: "unseal",
                table: "user_followers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modification_time",
                schema: "unseal",
                table: "user_followers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "last_modifier_id",
                schema: "unseal",
                table: "user_followers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "status_id",
                schema: "unseal",
                table: "user_followers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_profile_id",
                schema: "unseal",
                table: "capsules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "chat_types",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_chat_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_follow_statuses",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_follow_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chat_messages",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_chat_messages", x => x.id);
                    table.ForeignKey(
                        name: "f_k_chat_messages_chat_types_chat_type_id",
                        column: x => x.chat_type_id,
                        principalSchema: "unseal",
                        principalTable: "chat_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_chat_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "unseal",
                table: "capsule_types",
                keyColumn: "id",
                keyValue: new Guid("34b8f36b-8f43-449a-a86b-011ceb8c7f5b"),
                column: "code",
                value: 1);

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "chat_types",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { new Guid("121b1120-d7f8-41f9-bc06-32d72811d989"), 1, "Group" },
                    { new Guid("b15c886a-2929-43d2-ba06-416683a19420"), 0, "Directly" }
                });

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "notification_event_types",
                columns: new[] { "id", "code", "name" },
                values: new object[] { new Guid("b91d42b6-fae7-4376-a2e5-8cdca52402a0"), 2, "ConfirmChangeMail" });

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "user_follow_statuses",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { new Guid("585e849d-42aa-4ec7-a581-a56a7da434d8"), 1, "Accepted" },
                    { new Guid("9badcd74-212f-4e7e-9d04-5d0f2c28b713"), 2, "Rejected" },
                    { new Guid("e96877f3-5a69-4610-ac3b-7b224a1c17a9"), 0, "Pending" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_followers_status_id",
                schema: "unseal",
                table: "user_followers",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsules_user_profile_id",
                schema: "unseal",
                table: "capsules",
                column: "user_profile_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_chat_type_id",
                schema: "unseal",
                table: "chat_messages",
                column: "chat_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_sender_id",
                schema: "unseal",
                table: "chat_messages",
                column: "sender_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_capsules_user_profiles_user_profile_id",
                schema: "unseal",
                table: "capsules",
                column: "user_profile_id",
                principalSchema: "unseal",
                principalTable: "user_profiles",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_user_followers_user_follow_statuses_status_id",
                schema: "unseal",
                table: "user_followers",
                column: "status_id",
                principalSchema: "unseal",
                principalTable: "user_follow_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_capsules_user_profiles_user_profile_id",
                schema: "unseal",
                table: "capsules");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_followers_user_follow_statuses_status_id",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropTable(
                name: "chat_messages",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "user_follow_statuses",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "chat_types",
                schema: "unseal");

            migrationBuilder.DropIndex(
                name: "IX_user_followers_status_id",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropIndex(
                name: "IX_capsules_user_profile_id",
                schema: "unseal",
                table: "capsules");

            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "notification_event_types",
                keyColumn: "id",
                keyValue: new Guid("b91d42b6-fae7-4376-a2e5-8cdca52402a0"));

            migrationBuilder.DropColumn(
                name: "content",
                schema: "unseal",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "profile_picture_url",
                schema: "unseal",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "creation_time",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropColumn(
                name: "creator_id",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropColumn(
                name: "last_modification_time",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropColumn(
                name: "last_modifier_id",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropColumn(
                name: "status_id",
                schema: "unseal",
                table: "user_followers");

            migrationBuilder.DropColumn(
                name: "user_profile_id",
                schema: "unseal",
                table: "capsules");

            migrationBuilder.RenameColumn(
                name: "last_activity_time",
                schema: "unseal",
                table: "user_profiles",
                newName: "last_activity");

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                schema: "unseal",
                table: "capsules",
                type: "boolean",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "unseal",
                table: "capsule_types",
                keyColumn: "id",
                keyValue: new Guid("34b8f36b-8f43-449a-a86b-011ceb8c7f5b"),
                column: "code",
                value: 4);

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "capsule_types",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { new Guid("6e859f2b-3b3c-44df-9a16-a0c2d5bfc51e"), 1, "DirectMessage" },
                    { new Guid("8bd05f2f-5abc-4e58-aa1b-bd7d7419d243"), 3, "Collaborative" },
                    { new Guid("fd4f8d92-deb2-493d-a38c-5509b4ad7382"), 2, "PublicBroadcast" }
                });
        }
    }
}
