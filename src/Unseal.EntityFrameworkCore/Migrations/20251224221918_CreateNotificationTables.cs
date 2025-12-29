using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class CreateNotificationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification_event_types",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notification_event_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_templates",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    culture = table.Column<string>(type: "text", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: true),
                    app_name = table.Column<string>(type: "text", nullable: false),
                    notification_event_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notification_templates", x => x.id);
                    table.ForeignKey(
                        name: "f_k_notification_templates_notification_event_types_notificatio~",
                        column: x => x.notification_event_type_id,
                        principalSchema: "unseal",
                        principalTable: "notification_event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    destination_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notification_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notifications", x => x.id);
                    table.ForeignKey(
                        name: "f_k_notifications_notification_event_types_notification_event_id",
                        column: x => x.notification_event_id,
                        principalSchema: "unseal",
                        principalTable: "notification_event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_templates_notification_event_type_id",
                schema: "unseal",
                table: "notification_templates",
                column: "notification_event_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_notification_event_id",
                schema: "unseal",
                table: "notifications",
                column: "notification_event_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_templates",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "notification_event_types",
                schema: "unseal");
        }
    }
}
