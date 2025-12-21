using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unseal.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "open_iddict");

            migrationBuilder.EnsureSchema(
                name: "auth_management");

            migrationBuilder.EnsureSchema(
                name: "setting_management");

            migrationBuilder.CreateTable(
                name: "applications",
                schema: "open_iddict",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    client_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    client_secret = table.Column<string>(type: "text", nullable: true),
                    client_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    consent_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    json_web_key_set = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirect_uris = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    settings = table.Column<string>(type: "text", nullable: true),
                    front_channel_logout_uri = table.Column<string>(type: "text", nullable: true),
                    client_uri = table.Column<string>(type: "text", nullable: true),
                    logo_uri = table.Column<string>(type: "text", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "claim_types",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    required = table.Column<bool>(type: "boolean", nullable: false),
                    is_static = table.Column<bool>(type: "boolean", nullable: false),
                    regex = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    regex_description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    value_type = table.Column<int>(type: "integer", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claim_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "link_users",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_link_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization_units",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(95)", maxLength: 95, nullable: false),
                    display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_units", x => x.id);
                    table.ForeignKey(
                        name: "FK_organization_units_organization_units_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "auth_management",
                        principalTable: "organization_units",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "permission_grants",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    provider_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_grants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permission_groups",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    parent_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    multi_tenancy_side = table.Column<byte>(type: "smallint", nullable: false),
                    providers = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    state_checkers = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_static = table.Column<bool>(type: "boolean", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scopes",
                schema: "open_iddict",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scopes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "security_logs",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    application_name = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    identity = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    action = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    tenant_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    client_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    client_ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    browser_info = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_security_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    device = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    device_info = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ip_addresses = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    signed_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_accessed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "setting_definitions",
                schema: "setting_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    display_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    default_value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    is_visible_to_clients = table.Column<bool>(type: "boolean", nullable: false),
                    providers = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    is_inherited = table.Column<bool>(type: "boolean", nullable: false),
                    is_encrypted = table.Column<bool>(type: "boolean", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setting_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                schema: "setting_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    provider_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    provider_key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_delegations",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_delegations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    security_stamp = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_external = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    phone_number = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    should_change_password_on_next_login = table.Column<bool>(type: "boolean", nullable: false),
                    entity_version = table.Column<int>(type: "integer", nullable: false),
                    last_password_change_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "authorizations",
                schema: "open_iddict",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_authorizations_applications_application_id",
                        column: x => x.application_id,
                        principalSchema: "open_iddict",
                        principalTable: "applications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "organization_unit_roles",
                schema: "auth_management",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_unit_roles", x => new { x.organization_unit_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_organization_unit_roles_organization_units_organization_uni~",
                        column: x => x.organization_unit_id,
                        principalSchema: "auth_management",
                        principalTable: "organization_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organization_unit_roles_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth_management",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    claim_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    claim_value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth_management",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_connection_strings",
                schema: "auth_management",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_connection_strings", x => new { x.tenant_id, x.name });
                    table.ForeignKey(
                        name: "FK_tenant_connection_strings_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "auth_management",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                schema: "auth_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    claim_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    claim_value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_claims_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                schema: "auth_management",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider_key = table.Column<string>(type: "character varying(196)", maxLength: 196, nullable: false),
                    provider_display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => new { x.user_id, x.login_provider });
                    table.ForeignKey(
                        name: "FK_user_logins_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_organization_units",
                schema: "auth_management",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_organization_units", x => new { x.organization_unit_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_user_organization_units_organization_units_organization_uni~",
                        column: x => x.organization_unit_id,
                        principalSchema: "auth_management",
                        principalTable: "organization_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_organization_units_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "auth_management",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth_management",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                schema: "auth_management",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_user_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth_management",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                schema: "open_iddict",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    authorization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemption_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_applications_application_id",
                        column: x => x.application_id,
                        principalSchema: "open_iddict",
                        principalTable: "applications",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tokens_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalSchema: "open_iddict",
                        principalTable: "authorizations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_applications_client_id",
                schema: "open_iddict",
                table: "applications",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorizations_application_id_status_subject_type",
                schema: "open_iddict",
                table: "authorizations",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "IX_link_users_source_user_id_source_tenant_id_target_user_id_t~",
                schema: "auth_management",
                table: "link_users",
                columns: new[] { "source_user_id", "source_tenant_id", "target_user_id", "target_tenant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organization_unit_roles_role_id_organization_unit_id",
                schema: "auth_management",
                table: "organization_unit_roles",
                columns: new[] { "role_id", "organization_unit_id" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_units_code",
                schema: "auth_management",
                table: "organization_units",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_organization_units_parent_id",
                schema: "auth_management",
                table: "organization_units",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_permission_grants_tenant_id_name_provider_name_provider_key",
                schema: "auth_management",
                table: "permission_grants",
                columns: new[] { "tenant_id", "name", "provider_name", "provider_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permission_groups_name",
                schema: "auth_management",
                table: "permission_groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_group_name",
                schema: "auth_management",
                table: "permissions",
                column: "group_name");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_name",
                schema: "auth_management",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_role_id",
                schema: "auth_management",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_normalized_name",
                schema: "auth_management",
                table: "roles",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "IX_scopes_name",
                schema: "open_iddict",
                table: "scopes",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_tenant_id_action",
                schema: "auth_management",
                table: "security_logs",
                columns: new[] { "tenant_id", "action" });

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_tenant_id_application_name",
                schema: "auth_management",
                table: "security_logs",
                columns: new[] { "tenant_id", "application_name" });

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_tenant_id_identity",
                schema: "auth_management",
                table: "security_logs",
                columns: new[] { "tenant_id", "identity" });

            migrationBuilder.CreateIndex(
                name: "IX_security_logs_tenant_id_user_id",
                schema: "auth_management",
                table: "security_logs",
                columns: new[] { "tenant_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_sessions_device",
                schema: "auth_management",
                table: "sessions",
                column: "device");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_session_id",
                schema: "auth_management",
                table: "sessions",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_tenant_id_user_id",
                schema: "auth_management",
                table: "sessions",
                columns: new[] { "tenant_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_setting_definitions_name",
                schema: "setting_management",
                table: "setting_definitions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_settings_name_provider_name_provider_key",
                schema: "setting_management",
                table: "settings",
                columns: new[] { "name", "provider_name", "provider_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_name",
                schema: "auth_management",
                table: "tenants",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_normalized_name",
                schema: "auth_management",
                table: "tenants",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_application_id_status_subject_type",
                schema: "open_iddict",
                table: "tokens",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "IX_tokens_authorization_id",
                schema: "open_iddict",
                table: "tokens",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_reference_id",
                schema: "open_iddict",
                table: "tokens",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_user_id",
                schema: "auth_management",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_login_provider_provider_key",
                schema: "auth_management",
                table: "user_logins",
                columns: new[] { "login_provider", "provider_key" });

            migrationBuilder.CreateIndex(
                name: "IX_user_organization_units_user_id_organization_unit_id",
                schema: "auth_management",
                table: "user_organization_units",
                columns: new[] { "user_id", "organization_unit_id" });

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id_user_id",
                schema: "auth_management",
                table: "user_roles",
                columns: new[] { "role_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "auth_management",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_users_normalized_email",
                schema: "auth_management",
                table: "users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "IX_users_normalized_user_name",
                schema: "auth_management",
                table: "users",
                column: "normalized_user_name");

            migrationBuilder.CreateIndex(
                name: "IX_users_user_name",
                schema: "auth_management",
                table: "users",
                column: "user_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claim_types",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "link_users",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "organization_unit_roles",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "permission_grants",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "permission_groups",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "role_claims",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "scopes",
                schema: "open_iddict");

            migrationBuilder.DropTable(
                name: "security_logs",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "sessions",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "setting_definitions",
                schema: "setting_management");

            migrationBuilder.DropTable(
                name: "settings",
                schema: "setting_management");

            migrationBuilder.DropTable(
                name: "tenant_connection_strings",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "tokens",
                schema: "open_iddict");

            migrationBuilder.DropTable(
                name: "user_claims",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "user_delegations",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "user_logins",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "user_organization_units",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "user_tokens",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "authorizations",
                schema: "open_iddict");

            migrationBuilder.DropTable(
                name: "organization_units",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "users",
                schema: "auth_management");

            migrationBuilder.DropTable(
                name: "applications",
                schema: "open_iddict");
        }
    }
}
