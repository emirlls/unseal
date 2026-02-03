using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsActiveColumnOnCapsuleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "unseal",
                table: "capsules",
                type: "boolean",
                nullable: true,
                defaultValue: true);

            migrationBuilder.InsertData(
                schema: "unseal",
                table: "notification_event_types",
                columns: new[] { "id", "code", "name" },
                values: new object[] { new Guid("5d5650d6-b2ce-42d1-9f00-a4eea225d81e"), 3, "UserActivation" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "notification_event_types",
                keyColumn: "id",
                keyValue: new Guid("5d5650d6-b2ce-42d1-9f00-a4eea225d81e"));

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "unseal",
                table: "capsules");
        }
    }
}
