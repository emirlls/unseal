using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordResetOnNotificationEventTypeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "unseal",
                table: "notification_event_types",
                columns: new[] { "id", "code", "name" },
                values: new object[] { new Guid("00eb1757-1089-4094-ae49-cd23f1f58856"), 4, "PasswordReset" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "unseal",
                table: "notification_event_types",
                keyColumn: "id",
                keyValue: new Guid("00eb1757-1089-4094-ae49-cd23f1f58856"));
        }
    }
}
