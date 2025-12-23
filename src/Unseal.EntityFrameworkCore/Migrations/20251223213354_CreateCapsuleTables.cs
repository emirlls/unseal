using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class CreateCapsuleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "unseal");

            migrationBuilder.CreateTable(
                name: "capsule_types",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_capsule_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "capsules",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: true),
                    is_opened = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    reveal_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    capsule_type_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("p_k_capsules", x => x.id);
                    table.ForeignKey(
                        name: "f_k_capsules_capsule_types_capsule_type_id",
                        column: x => x.capsule_type_id,
                        principalSchema: "unseal",
                        principalTable: "capsule_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "capsule_items",
                schema: "unseal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    capsule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    text_context = table.Column<string>(type: "text", nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    file_name = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("p_k_capsule_items", x => x.id);
                    table.ForeignKey(
                        name: "f_k_capsule_items_capsules_capsule_id",
                        column: x => x.capsule_id,
                        principalSchema: "unseal",
                        principalTable: "capsules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "capsule_types",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { new Guid("0b2374ec-0040-4d5d-b9a6-5c6333b458c8"), 0, "Personal" },
                    { new Guid("34b8f36b-8f43-449a-a86b-011ceb8c7f5b"), 4, "Public" },
                    { new Guid("6e859f2b-3b3c-44df-9a16-a0c2d5bfc51e"), 1, "DirectMessage" },
                    { new Guid("8bd05f2f-5abc-4e58-aa1b-bd7d7419d243"), 3, "Collaborative" },
                    { new Guid("fd4f8d92-deb2-493d-a38c-5509b4ad7382"), 2, "PublicBroadcast" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_capsule_items_capsule_id",
                schema: "unseal",
                table: "capsule_items",
                column: "capsule_id");

            migrationBuilder.CreateIndex(
                name: "IX_capsules_capsule_type_id",
                schema: "unseal",
                table: "capsules",
                column: "capsule_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "capsule_items",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "capsules",
                schema: "unseal");

            migrationBuilder.DropTable(
                name: "capsule_types",
                schema: "unseal");
        }
    }
}
