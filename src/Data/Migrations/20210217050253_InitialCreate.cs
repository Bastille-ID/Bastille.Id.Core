/*
 * Bastille.ID Identity Server
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace Bastille.Id.Core.Data.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class implements the Bastille database enhancements and tables schema.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Migrations.Migration" />
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// <para>Builds the operations that will migrate the database 'up'.</para>
        /// <para>
        /// That is, builds the operations that will take the database from the state left in by the previous migration so that it is up-to-date with regard to
        /// this migration.
        /// </para>
        /// <para>This method must be overridden in each class the inherits from <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Migration" />.</para>
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder" /> that will build the operations.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    PasswordlessEnabled = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    AvailableLocale = table.Column<bool>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageCode);
                });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    TimeZoneId = table.Column<string>(maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(maxLength: 200, nullable: true),
                    LongName = table.Column<string>(maxLength: 300, nullable: true),
                    Offset = table.Column<double>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.TimeZoneId);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventDateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UserId = table.Column<Guid>(maxLength: 450, nullable: true),
                    Event = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Action = table.Column<string>(maxLength: 30, nullable: true),
                    Result = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: true),
                    ClientAddress = table.Column<string>(maxLength: 30, nullable: true),
                    Location = table.Column<string>(maxLength: 250, nullable: true),
                    Request = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    RoleType = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedByUserId = table.Column<Guid>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedByUserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_IdentityRoles_IdentityUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdentityRoles_IdentityUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserClaims",
                columns: table => new
                {
                    UserClaimId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaims", x => x.UserClaimId);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaims_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogins_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_IdentityUserTokens_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    Slug = table.Column<string>(maxLength: 150, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    OwnerUserId = table.Column<Guid>(nullable: false),
                    Address1 = table.Column<string>(maxLength: 200, nullable: true),
                    Address2 = table.Column<string>(maxLength: 200, nullable: true),
                    City = table.Column<string>(maxLength: 200, nullable: true),
                    Region = table.Column<string>(maxLength: 200, nullable: true),
                    PostalCode = table.Column<string>(maxLength: 50, nullable: true),
                    Country = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.OrganizationId);
                    table.ForeignKey(
                        name: "FK_Organizations_IdentityUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    OrganizationId = table.Column<Guid>(nullable: true),
                    TemplateKey = table.Column<string>(maxLength: 50, nullable: false),
                    TemplateType = table.Column<string>(type: "varchar(20)", nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 5, nullable: true),
                    Content = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedByUserId = table.Column<Guid>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedByUserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_Templates_IdentityUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Templates_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Templates_IdentityUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleClaims",
                columns: table => new
                {
                    RoleClaimId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaims", x => x.RoleClaimId);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaims_IdentityRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRoles_IdentityRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "IdentityRoles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUserRoles_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    ParentGroupId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    OwnerUserId = table.Column<Guid>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedByUserId = table.Column<Guid>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedByUserId = table.Column<Guid>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_Groups_IdentityUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Groups_IdentityUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_Groups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_IdentityUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationAllowedDomains",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Domain = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationAllowedDomains", x => new { x.OrganizationId, x.Domain });
                    table.ForeignKey(
                        name: "FK_OrganizationAllowedDomains_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantConfigs",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(nullable: false, defaultValueSql: "newid()"),
                    TenantKey = table.Column<string>(maxLength: 100, nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LogoUrl = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedByUserId = table.Column<Guid>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedByUserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantConfigs", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_TenantConfigs_IdentityUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantConfigs_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantConfigs_IdentityUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupUsers",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(maxLength: 450, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUsers", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupUsers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_GroupUsers_IdentityUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "LanguageCode", "Active", "AvailableLocale", "Default", "Name" },
                values: new object[,]
                {
                    { "af-ZA", true, false, false, "Afrikaans (South Africa)" },
                    { "ms-BN", true, false, false, "Malay (Brunei)" },
                    { "mr-IN", true, false, false, "Marathi (India)" },
                    { "mn-MN", true, false, false, "Mongolian" },
                    { "mk-MK", true, false, false, "Macedonian" },
                    { "lv-LV", true, false, false, "Latvian" },
                    { "lt-LT", true, false, false, "Lithuanian" },
                    { "ky-KZ", true, false, false, "Kyrgyz" },
                    { "ko-KR", true, false, false, "Korean (Korea)" },
                    { "kn-IN", true, false, false, "Kannada - India" },
                    { "kk-KZ", true, false, false, "Kazakh" },
                    { "ka-GE", true, false, false, "Georgian" },
                    { "ja-JP", true, false, false, "Japanese" },
                    { "it-IT", true, false, false, "Italian" },
                    { "ms-MY", true, false, false, "Malay (Malaysia)" },
                    { "it-CH", true, false, false, "Italian (Switzerland)" },
                    { "id-ID", true, false, false, "Indonesian" },
                    { "hy-AM", true, false, false, "Armenian (Armenia)" },
                    { "hu-HU", true, false, false, "Hungarian" },
                    { "hr-HR", true, false, false, "Croatian" },
                    { "hi-IN", true, false, false, "Hindi (India)" },
                    { "he-IL", true, false, false, "Hebrew (Israel)" },
                    { "gu-IN", true, false, false, "Gujarati (India)" },
                    { "gl-ES", true, false, false, "Galician" },
                    { "fr-MC", true, false, false, "French (Manaco)" },
                    { "fr-LU", true, false, false, "French (Luxembourg)" },
                    { "fr-FR", true, false, false, "French" },
                    { "fr-CH", true, false, false, "French (Switzerland)" },
                    { "fr-CA", true, false, false, "French (Canada)" },
                    { "is-IS", true, false, false, "Icelandic" },
                    { "fr-BE", true, false, false, "French (Belgium)" },
                    { "nl-BE", true, false, false, "Dutch (Belgium)" },
                    { "nn-NO", true, false, false, "Norwegian (Nynorsk)" },
                    { "zh-TW", true, false, false, "Chinese (Taiwan)" },
                    { "zh-SG", true, false, false, "Chinese (Singapore)" },
                    { "zh-MO", true, false, false, "Chinese (Macau SAR)" },
                    { "zh-HK", true, false, false, "Chinese (Hon Kong SAR)" },
                    { "zh-CN", true, false, false, "Chinese (PRC)" },
                    { "vi-VN", true, false, false, "Vietnamese" },
                    { "ur-PK", true, false, false, "Urdu (Pakistan)" },
                    { "uk-UA", true, false, false, "Ukrainian" },
                    { "tx-TX", true, false, false, "Texas" },
                    { "tt-RU", true, false, false, "Tatar (Russia)" },
                    { "tr-TR", true, false, false, "Turkish" },
                    { "th-TH", true, false, false, "Thai (Thailand)" },
                    { "te-IN", true, false, false, "Telugu (India)" },
                    { "nl-NL", true, false, false, "Dutch (Standard)" },
                    { "ta-IN", true, false, false, "Tamil (India)" },
                    { "sw-KE", true, false, false, "Swahili (Kenya)" },
                    { "sv-SE", true, false, false, "Swedish (Sweden)" },
                    { "sv-FI", true, false, false, "Swedish (Finland)" },
                    { "sq-AL", true, false, false, "Albania" },
                    { "sl-SI", true, false, false, "Slovenian" },
                    { "sk-SK", true, false, false, "Slovak" },
                    { "sa-IN", true, false, false, "Sanskrit (India)" },
                    { "ru-RU", true, false, false, "Russian" },
                    { "ro-RO", true, false, false, "Romanian" },
                    { "pt-PT", true, false, false, "Portuguese (Portugal)" },
                    { "pt-BR", true, false, false, "Portuguese (Brazil)" },
                    { "pl-PL", true, false, false, "Polish" },
                    { "pa-IN", true, false, false, "Punjabi (India)" },
                    { "sy-SY", true, false, false, "Syriac" },
                    { "fo-FO", true, false, false, "Faroese" },
                    { "nb-NO", true, false, false, "Norwegian (Bokmal)" },
                    { "fa-IR", true, false, false, "Farsi (Iran)" },
                    { "en-AU", true, false, false, "English (Australia)" },
                    { "el-GR", true, false, false, "Greek" },
                    { "de-LU", true, false, false, "German (Luxembourg)" },
                    { "de-LI", true, false, false, "German (Lechtenstein)" },
                    { "de-DE", true, false, false, "German" },
                    { "de-CH", true, false, false, "German (Switzerland)" },
                    { "de-AT", true, false, false, "German (Austria)" },
                    { "da-DK", true, false, false, "Danish (Denmark)" },
                    { "cs-CZ", true, false, false, "Czech" },
                    { "ca-ES", true, false, false, "Catalan" },
                    { "bg-BG", true, false, false, "Bulgarian" },
                    { "be-BY", true, false, false, "Belarusian (Belarus)" },
                    { "ar-TN", true, false, false, "Arabic (Tunisia)" },
                    { "ar-SY", true, false, false, "Arabic (Syria)" },
                    { "ar-SA", true, false, false, "Arabic (Saudi Arabia)" },
                    { "ar-QA", true, false, false, "Arabic (Qatar)" },
                    { "ar-OM", true, false, false, "Arabic (Oman)" },
                    { "ar-MA", true, false, false, "Arabic (Morocco)" },
                    { "ar-LY", true, false, false, "Arabic (Libya)" },
                    { "ar-LB", true, false, false, "Arabic (Lebanon)" },
                    { "ar-KW", true, false, false, "Arabic (Kuwait)" },
                    { "ar-JO", true, false, false, "Arabic (Jordan)" },
                    { "ar-IQ", true, false, false, "Arabic (Iraq)" },
                    { "ar-EG", true, false, false, "Arabic (Egypt)" },
                    { "ar-DZ", true, false, false, "Arabic (Algeria)" },
                    { "ar-BH", true, false, false, "Arabic (Bahrain)" },
                    { "ar-AR", true, false, false, "Arabic" },
                    { "ar-AE", true, false, false, "Arabic (U.A.E.)" },
                    { "fi-FI", true, false, false, "Finish (Finland)" },
                    { "en-CA", true, false, false, "English (Canada)" },
                    { "en-CB", true, false, false, "English (Caribbean)" },
                    { "ar-YE", true, false, false, "Arabic (Yemen)" },
                    { "en-GB", true, false, false, "English (UK)" },
                    { "et-EE", true, false, false, "Estonian" },
                    { "es-VE", true, false, false, "Spanish (Venezuela)" },
                    { "es-UY", true, false, false, "Spanish (Uruguay)" },
                    { "es-SV", true, false, false, "Spanish (El Salvador)" },
                    { "es-PY", true, false, false, "Spanish (Paraguay)" },
                    { "es-PR", true, false, false, "Spanish (Puerto Rico)" },
                    { "es-PE", true, false, false, "Spanish (Peru)" },
                    { "es-PA", true, false, false, "Spanish (Panama)" },
                    { "es-NI", true, false, false, "Spanish (Nicaragua)" },
                    { "es-MX", true, false, false, "Spanish (Mexico)" },
                    { "es-HN", true, false, false, "Spanish (Hondorus)" },
                    { "es-GT", true, false, false, "Spanish (Guatemala)" },
                    { "es-ES", true, false, false, "Spanish" },
                    { "es-EC", true, false, false, "Spanish (Ecuador)" },
                    { "eu-ES", true, false, false, "Basque" },
                    { "es-CR", true, false, false, "Spanish (Costa Rica)" },
                    { "es-DO", true, false, false, "Spanish (Dominican Republic)" },
                    { "en-JM", true, false, false, "English (Jamaica)" },
                    { "en-NZ", true, false, false, "English (New Zealand)" },
                    { "en-PH", true, false, false, "English (Philippines)" },
                    { "en-TT", true, false, false, "English (Trinidad & Tobago)" },
                    { "en-US", true, true, true, "English" },
                    { "en-IE", true, false, false, "English (Ireland)" },
                    { "en-ZW", true, false, false, "English (Zimbabwe)" },
                    { "es-AR", true, false, false, "Spanish (Argentina)" },
                    { "es-BO", true, false, false, "Spanish (Bolivia)" },
                    { "es-CL", true, false, false, "Spanish (Chile)" },
                    { "es-CO", true, false, false, "Spanish (Columbia)" },
                    { "en-ZA", true, false, false, "English (South Africa)" }
                });

            migrationBuilder.InsertData(
                table: "TimeZones",
                columns: new[] { "TimeZoneId", "Active", "Default", "LongName", "Offset", "ShortName" },
                values: new object[,]
                {
                    { "Asia/Kolkata", true, false, "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi", 5.5, "India Standard Time" },
                    { "Asia/Colombo", true, false, "(UTC+05:30) Sri Jayawardenepura", 5.5, "Sri Lanka Standard Time" },
                    { "Asia/Kathmandu", true, false, "(UTC+05:45) Kathmandu", 5.75, "Nepal Standard Time" },
                    { "Asia/Novosibirsk", true, false, "(UTC+07:00) Novosibirsk", 7.0, "N. Central Asia Standard Time" },
                    { "Asia/Almaty", true, false, "(UTC+06:00) Astana", 6.0, "Central Asia Standard Time" },
                    { "Asia/Dhaka", true, false, "(UTC+06:00) Dhaka", 6.0, "Bangladesh Standard Time" },
                    { "Asia/Bangkok", true, false, "(UTC+07:00) Bangkok, Hanoi, Jakarta", 7.0, "SE Asia Standard Time" },
                    { "Asia/Yangon", true, false, "(UTC+06:30) Yangon (Rangoon)", 6.5, "Myanmar Standard Time" },
                    { "Asia/Barnaul", true, false, "(UTC+07:00) Barnaul, Gorno-Altaysk", 7.0, "Altai Standard Time" },
                    { "Asia/Hovd", true, false, "(UTC+07:00) Hovd", 7.0, "W. Mongolia Standard Time" },
                    { "Asia/Krasnoyarsk", true, false, "(UTC+07:00) Krasnoyarsk", 7.0, "North Asia Standard Time" },
                    { "Asia/Karachi", true, false, "(UTC+05:00) Islamabad, Karachi", 5.0, "Pakistan Standard Time" },
                    { "Asia/Omsk", true, false, "(UTC+06:00) Omsk", 6.0, "Omsk Standard Time" },
                    { "Asia/Yekaterinburg", true, false, "(UTC+05:00) Ekaterinburg", 5.0, "Ekaterinburg Standard Time" },
                    { "Asia/Baku", true, false, "(UTC+04:00) Baku", 4.0, "Azerbaijan Standard Time" },
                    { "Asia/Kabul", true, false, "(UTC+04:30) Kabul", 4.5, "Afghanistan Standard Time" },
                    { "Asia/Yerevan", true, false, "(UTC+04:00) Yerevan", 4.0, "Caucasus Standard Time" },
                    { "Asia/Tbilisi", true, false, "(UTC+04:00) Tbilisi", 4.0, "Georgian Standard Time" },
                    { "Indian/Mauritius", true, false, "(UTC+04:00) Port Louis", 4.0, "Mauritius Standard Time" },
                    { "Europe/Samara", true, false, "(UTC+04:00) Izhevsk, Samara", 4.0, "Russia Time Zone 3" },
                    { "Europe/Astrakhan", true, false, "(UTC+04:00) Astrakhan, Ulyanovsk", 4.0, "Astrakhan Standard Time" },
                    { "Asia/Dubai", true, false, "(UTC+04:00) Abu Dhabi, Muscat", 4.0, "Arabian Standard Time" },
                    { "Asia/Tehran", true, false, "(UTC+03:30) Tehran", 3.5, "Iran Standard Time" },
                    { "Africa/Nairobi", true, false, "(UTC+03:00) Nairobi", 3.0, "E. Africa Standard Time" },
                    { "Europe/Moscow", true, false, "(UTC+03:00) Moscow, St. Petersburg, Volgograd", 3.0, "Russian Standard Time" },
                    { "Europe/Minsk", true, false, "(UTC+03:00) Minsk", 3.0, "Belarus Standard Time" },
                    { "Asia/Riyadh", true, false, "(UTC+03:00) Kuwait, Riyadh", 3.0, "Arab Standard Time" },
                    { "Asia/Baghdad", true, false, "(UTC+03:00) Baghdad", 3.0, "Arabic Standard Time" },
                    { "Asia/Tomsk", true, false, "(UTC+07:00) Tomsk", 7.0, "Tomsk Standard Time" },
                    { "Asia/Tashkent", true, false, "(UTC+05:00) Ashgabat, Tashkent", 5.0, "West Asia Standard Time" },
                    { "Asia/Shanghai", true, false, "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi", 8.0, "China Standard Time" },
                    { "Pacific/Tongatapu", true, false, "(UTC+13:00) Nuku'alofa", 13.0, "Tonga Standard Time" },
                    { "Asia/Singapore", true, false, "(UTC+08:00) Kuala Lumpur, Singapore", 8.0, "Singapore Standard Time" },
                    { "Africa/Tripoli", true, false, "(UTC+02:00) Tripoli", 2.0, "Libya Standard Time" },
                    { "Pacific/Chatham", true, false, "(UTC+12:45) Chatham Islands", 12.75, "Chatham Islands Standard Time" },
                    { "Asia/Kamchatka", true, false, "(UTC+12:00) Petropavlovsk-Kamchatsky - Old", 12.0, "Kamchatka Standard Time" },
                    { "Pacific/Fiji", true, false, "(UTC+12:00) Fiji", 12.0, "Fiji Standard Time" },
                    { "Etc/GMT-12", true, false, "(UTC+12:00) Coordinated Universal Time+12", 12.0, "UTC+12" },
                    { "Pacific/Auckland", true, false, "(UTC+12:00) Auckland, Wellington", 12.0, "New Zealand Standard Time" },
                    { "Pacific/Guadalcanal", true, false, "(UTC+11:00) Solomon Is., New Caledonia", 11.0, "Central Pacific Standard Time" },
                    { "Asia/Sakhalin", true, false, "(UTC+11:00) Sakhalin", 11.0, "Sakhalin Standard Time" },
                    { "Pacific/Norfolk", true, false, "(UTC+11:00) Norfolk Island", 11.0, "Norfolk Standard Time" },
                    { "Asia/Magadan", true, false, "(UTC+11:00) Magadan", 11.0, "Magadan Standard Time" },
                    { "Asia/Srednekolymsk", true, false, "(UTC+11:00) Chokurdakh", 11.0, "Russia Time Zone 10" },
                    { "Pacific/Bougainville", true, false, "(UTC+11:00) Bougainville Island", 11.0, "Bougainville Standard Time" },
                    { "Australia/Lord_Howe", true, false, "(UTC+10:30) Lord Howe Island", 10.5, "Lord Howe Standard Time" },
                    { "Asia/Vladivostok", true, false, "(UTC+10:00) Vladivostok", 10.0, "Vladivostok Standard Time" },
                    { "Australia/Hobart", true, false, "(UTC+10:00) Hobart", 10.0, "Tasmania Standard Time" },
                    { "Pacific/Port_Moresby", true, false, "(UTC+10:00) Guam, Port Moresby", 10.0, "West Pacific Standard Time" },
                    { "Australia/Sydney", true, false, "(UTC+10:00) Canberra, Melbourne, Sydney", 10.0, "AUS Eastern Standard Time" },
                    { "Australia/Brisbane", true, false, "(UTC+10:00) Brisbane", 10.0, "E. Australia Standard Time" },
                    { "Australia/Darwin", true, false, "(UTC+09:30) Darwin", 9.5, "AUS Central Standard Time" },
                    { "Australia/Adelaide", true, false, "(UTC+09:30) Adelaide", 9.5, "Cen. Australia Standard Time" },
                    { "Asia/Yakutsk", true, false, "(UTC+09:00) Yakutsk", 9.0, "Yakutsk Standard Time" },
                    { "Asia/Seoul", true, false, "(UTC+09:00) Seoul", 9.0, "Korea Standard Time" },
                    { "Asia/Tokyo", true, false, "(UTC+09:00) Osaka, Sapporo, Tokyo", 9.0, "Tokyo Standard Time" },
                    { "Asia/Chita", true, false, "(UTC+09:00) Chita", 9.0, "Transbaikal Standard Time" },
                    { "Australia/Eucla", true, false, "(UTC+08:45) Eucla", 8.75, "Aus Central W. Standard Time" },
                    { "Asia/Pyongyang", true, false, "(UTC+08:30) Pyongyang", 8.5, "North Korea Standard Time" },
                    { "Asia/Ulaanbaatar", true, false, "(UTC+08:00) Ulaanbaatar", 8.0, "Ulaanbaatar Standard Time" },
                    { "Asia/Taipei", true, false, "(UTC+08:00) Taipei", 8.0, "Taipei Standard Time" },
                    { "Australia/Perth", true, false, "(UTC+08:00) Perth", 8.0, "W. Australia Standard Time" },
                    { "Asia/Irkutsk", true, false, "(UTC+08:00) Irkutsk", 8.0, "North Asia East Standard Time" },
                    { "Europe/Kaliningrad", true, false, "(UTC+02:00) Kaliningrad", 2.0, "Kaliningrad Standard Time" },
                    { "Etc/GMT+11", true, false, "(UTC-11:00) Coordinated Universal Time-11", -11.0, "UTC-11" },
                    { "Europe/Istanbul", true, false, "(UTC+02:00) Istanbul", 2.0, "Turkey Standard Time" },
                    { "America/La_Paz", true, false, "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan", -4.0, "SA Western Standard Time" },
                    { "America/Cuiaba", true, false, "(UTC-04:00) Cuiaba", -4.0, "Central Brazilian Standard Time" },
                    { "America/Caracas", true, false, "(UTC-04:00) Caracas", -4.0, "Venezuela Standard Time" },
                    { "America/Halifax", true, false, "(UTC-04:00) Atlantic Time (Canada)", -4.0, "Atlantic Standard Time" },
                    { "America/Asuncion", true, false, "(UTC-04:00) Asuncion", -4.0, "Paraguay Standard Time" },
                    { "America/Indiana/Indianapolis", true, false, "(UTC-05:00) Indiana (East)", -5.0, "US Eastern Standard Time" },
                    { "America/Havana", true, false, "(UTC-05:00) Havana", -5.0, "Cuba Standard Time" },
                    { "America/Port-au-Prince", true, false, "(UTC-05:00) Haiti", -5.0, "Haiti Standard Time" },
                    { "America/New_York", true, false, "(UTC-05:00) Eastern Time (US & Canada)", -5.0, "Eastern Standard Time" },
                    { "America/Cancun", true, false, "(UTC-05:00) Chetumal", -5.0, "Eastern Standard Time (Mexico)" },
                    { "America/Bogota", true, false, "(UTC-05:00) Bogota, Lima, Quito, Rio Branco", -5.0, "SA Pacific Standard Time" },
                    { "America/Regina", true, false, "(UTC-06:00) Saskatchewan", -6.0, "Canada Central Standard Time" },
                    { "America/Mexico_City", true, false, "(UTC-06:00) Guadalajara, Mexico City, Monterrey", -6.0, "Central Standard Time (Mexico)" },
                    { "Pacific/Easter", true, false, "(UTC-06:00) Easter Island", -6.0, "Easter Island Standard Time" },
                    { "America/Chicago", true, false, "(UTC-06:00) Central Time (US & Canada)", -6.0, "Central Standard Time" },
                    { "America/Guatemala", true, false, "(UTC-06:00) Central America", -6.0, "Central America Standard Time" },
                    { "America/Denver", true, false, "(UTC-07:00) Mountain Time (US & Canada)", -7.0, "Mountain Standard Time" },
                    { "America/Chihuahua", true, false, "(UTC-07:00) Chihuahua, La Paz, Mazatlan", -7.0, "Mountain Standard Time (Mexico)" },
                    { "America/Phoenix", true, false, "(UTC-07:00) Arizona", -7.0, "US Mountain Standard Time" },
                    { "America/Los_Angeles", true, false, "(UTC-08:00) Pacific Time (US & Canada)", -8.0, "Pacific Standard Time" },
                    { "Etc/GMT+8", true, false, "(UTC-08:00) Coordinated Universal Time-08", -8.0, "UTC-08" },
                    { "America/Tijuana", true, false, "(UTC-08:00) Baja California", -8.0, "Pacific Standard Time (Mexico)" },
                    { "Etc/GMT+9", true, false, "(UTC-09:00) Coordinated Universal Time-09", -9.0, "UTC-09" },
                    { "America/Anchorage", true, false, "(UTC-09:00) Alaska", -9.0, "Alaskan Standard Time" },
                    { "Pacific/Marquesas", true, false, "(UTC-09:30) Marquesas Islands", -9.5, "Marquesas Standard Time" },
                    { "Pacific/Honolulu", true, false, "(UTC-10:00) Hawaii", -10.0, "Hawaiian Standard Time" },
                    { "America/Adak", true, false, "(UTC-10:00) Aleutian Islands", -10.0, "Aleutian Standard Time" },
                    { "Pacific/Apia", true, false, "(UTC+13:00) Samoa", 13.0, "Samoa Standard Time" },
                    { "Etc/GMT+12", true, false, "(UTC-12:00) International Date Line West", -12.0, "Dateline Standard Time" },
                    { "America/Santiago", true, false, "(UTC-04:00) Santiago", -4.0, "Pacific SA Standard Time" },
                    { "Asia/Jerusalem", true, false, "(UTC+02:00) Jerusalem", 2.0, "Israel Standard Time" },
                    { "America/Grand_Turk", true, false, "(UTC-04:00) Turks and Caicos", -4.0, "Turks And Caicos Standard Time" },
                    { "America/Araguaina", true, false, "(UTC-03:00) Araguaina", -3.0, "Tocantins Standard Time" },
                    { "Europe/Kiev", true, false, "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius", 2.0, "FLE Standard Time" },
                    { "Africa/Johannesburg", true, false, "(UTC+02:00) Harare, Pretoria", 2.0, "South Africa Standard Time" },
                    { "Asia/Hebron", true, false, "(UTC+02:00) Gaza, Hebron", 2.0, "West Bank Standard Time" },
                    { "Asia/Damascus", true, false, "(UTC+02:00) Damascus", 2.0, "Syria Standard Time" },
                    { "Europe/Chisinau", true, false, "(UTC+02:00) Chisinau", 2.0, "E. Europe Standard Time" },
                    { "Africa/Cairo", true, false, "(UTC+02:00) Cairo", 2.0, "Egypt Standard Time" },
                    { "Asia/Beirut", true, false, "(UTC+02:00) Beirut", 2.0, "Middle East Standard Time" },
                    { "Europe/Bucharest", true, false, "(UTC+02:00) Athens, Bucharest", 2.0, "GTB Standard Time" },
                    { "Asia/Amman", true, false, "(UTC+02:00) Amman", 2.0, "Jordan Standard Time" },
                    { "Africa/Windhoek", true, false, "(UTC+01:00) Windhoek", 1.0, "Namibia Standard Time" },
                    { "Africa/Lagos", true, false, "(UTC+01:00) West Central Africa", 1.0, "W. Central Africa Standard Time" },
                    { "Europe/Warsaw", true, false, "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb", 1.0, "Central European Standard Time" },
                    { "Europe/Paris", true, false, "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris", 1.0, "Romance Standard Time" },
                    { "Europe/Budapest", true, false, "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague", 1.0, "Central Europe Standard Time" },
                    { "Europe/Berlin", true, false, "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna", 1.0, "W. Europe Standard Time" },
                    { "Atlantic/Reykjavik", true, false, "(UTC+00:00) Monrovia, Reykjavik", 0.0, "Greenwich Standard Time" },
                    { "Europe/London", true, false, "(UTC+00:00) Dublin, Edinburgh, Lisbon, London", 0.0, "GMT Standard Time" },
                    { "Africa/Casablanca", true, false, "(UTC+00:00) Casablanca", 0.0, "Morocco Standard Time" },
                    { "Etc/UTC", true, true, "(UTC) Coordinated Universal Time", 0.0, "UTC" },
                    { "Atlantic/Cape_Verde", true, false, "(UTC-01:00) Cabo Verde Is.", -1.0, "Cape Verde Standard Time" },
                    { "Atlantic/Azores", true, false, "(UTC-01:00) Azores", -1.0, "Azores Standard Time" },
                    { "Etc/GMT+2", true, false, "(UTC-02:00) Coordinated Universal Time-02", -2.0, "UTC-02" },
                    { "America/Bahia", true, false, "(UTC-03:00) Salvador", -3.0, "Bahia Standard Time" },
                    { "America/Miquelon", true, false, "(UTC-03:00) Saint Pierre and Miquelon", -3.0, "Saint Pierre Standard Time" },
                    { "America/Montevideo", true, false, "(UTC-03:00) Montevideo", -3.0, "Montevideo Standard Time" },
                    { "America/Godthab", true, false, "(UTC-03:00) Greenland", -3.0, "Greenland Standard Time" },
                    { "America/Argentina/Buenos_Aires", true, false, "(UTC-03:00) City of Buenos Aires", -3.0, "Argentina Standard Time" },
                    { "America/Cayenne", true, false, "(UTC-03:00) Cayenne, Fortaleza", -3.0, "SA Eastern Standard Time" },
                    { "America/Sao_Paulo", true, false, "(UTC-03:00) Brasilia", -3.0, "E. South America Standard Time" },
                    { "America/St_Johns", true, false, "(UTC-03:30) Newfoundland", -3.5, "Newfoundland Standard Time" },
                    { "Pacific/Kiritimati", true, false, "(UTC+14:00) Kiritimati Island", 14.0, "Line Islands Standard Time" }
                });

            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "TemplateId", "Content", "CreatedByUserId", "CreatedDate", "LanguageCode", "OrganizationId", "TemplateKey", "TemplateType", "UpdatedByUserId", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("41e1d669-53b1-431d-ba1d-7bafca4a9b50"), @"Forgot Your Password?

                        It happens. Click the link below to reset your password.

                        $LINK$

                        If you did not request a password reset, please contact us at $SUPPORTEMAIL$.", null, new DateTime(2021, 2, 17, 5, 2, 53, 456, DateTimeKind.Utc).AddTicks(1902), LocaleExtensions.DefaultLanguageCode, null, "reset_password_en-US_txt", "Message", null, null },
                    { new Guid("9a1cb149-67dd-4b7f-bf91-d3566d9a17bf"), @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
                <html>
                <head>
                <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" >
                <title>Reset Your Password</title>
                <style type=""text/css"">
                html { -webkit-text-size-adjust: none; -ms-text-size-adjust: none;}

                    @media only screen and (min-device-width: 750px) {
                        .table750 {width: 750px !important;}
                    }
                    @media only screen and (max-device-width: 750px), only screen and (max-width: 750px){
                      table[class=""table750""] {width: 100% !important;}
                      .mob_b {width: 93% !important; max-width: 93% !important; min-width: 93% !important;}
                      .mob_b1 {width: 100% !important; max-width: 100% !important; min-width: 100% !important;}
                      .mob_left {text-align: left !important;}
                      .mob_soc {width: 50% !important; max-width: 50% !important; min-width: 50% !important;}
                      .mob_menu {width: 50% !important; max-width: 50% !important; min-width: 50% !important; box-shadow: inset -1px -1px 0 0 rgba(255, 255, 255, 0.2); }
                      .mob_center {text-align: center !important;}
                      .top_pad {height: 15px !important; max-height: 15px !important; min-height: 15px !important;}
                      .mob_pad {width: 15px !important; max-width: 15px !important; min-width: 15px !important;}
                      .mob_div {display: block !important;}
                    }
                   @media only screen and (max-device-width: 550px), only screen and (max-width: 550px){
                      .mod_div {display: block !important;}
                   }
                    .table750 {width: 750px;}
                </style>
                </head>
                <body style=""margin: 0; padding: 0;"">

                <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #f3f3f3; min-width: 350px; font-size: 1px; line-height: normal;"">
                    <tr>
                    <td align=""center"" valign=""top"">
                        <!--[if (gte mso 9)|(IE)]>
                         <table border=""0"" cellspacing=""0"" cellpadding=""0"">
                         <tr><td align=""center"" valign=""top"" width=""750""><![endif]-->
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""750"" class=""table750"" style=""width: 100%; max-width: 750px; min-width: 350px; background: #f3f3f3;"">
                            <tr>
                               <td class=""mob_pad"" width=""25"" style=""width: 25px; max-width: 25px; min-width: 25px;"">&nbsp;</td>
                                <td align=""center"" valign=""top"" style=""background: #ffffff;"">

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""width: 100% !important; min-width: 100%; max-width: 100%; background: #f3f3f3;"">
                                     <tr>
                                        <td align=""right"" valign=""top"">
                                           <div class=""top_pad"" style=""height: 25px; line-height: 25px; font-size: 23px;"">&nbsp;</div>
                                        </td>
                                     </tr>
                                  </table>

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
                                     <tr>
                                        <td align=""center"" valign=""top"">
                                           <div style=""height: 39px; line-height: 39px; font-size: 37px;"">&nbsp;</div>
                                           <a href=""https://bastille.id"" target=""_blank"" style=""display: block; max-width: 128px;"" alt=""Bastille.Id"">
                                              <img src=""$URL$/img/logo.png"" alt=""img"" width=""128"" border=""0"" style=""display: block; width: 128px;"" />
                                           </a>
                                           <div style=""height: 32px; line-height: 32px; font-size: 51px;"">&nbsp;</div>
                                        </td>
                                     </tr>
                                  </table>

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
                                     <tr>
                                        <td align=""left"" valign=""top"">
                                           <font face=""'Source Sans Pro', sans-serif"" color=""#585858"" style=""font-size: 24px; line-height: 32px;"">
                                              <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #585858; font-size: 24px; line-height: 32px;"">
                                              Hey, it happens. Crazy passwords are hard to remember. Click the link below to reset your password.
                                              </span>
                                           </font>
                                           <div style=""height: 33px; line-height: 33px; font-size: 31px;"">&nbsp;</div>
                                           <table class=""mob_btn"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background: #27cbcc; border-radius: 4px;"">
                                              <tr>
                                                 <td align=""center"" valign=""top"">
                                                    <a href=""$LINK$"" target=""_blank"" style=""display: block; border: 1px solid #27cbcc; border-radius: 4px; padding: 12px 23px; font-family: 'Source Sans Pro', Arial, Verdana, Tahoma, Geneva, sans-serif; color: #ffffff; font-size: 20px; line-height: 30px; text-decoration: none; white-space: nowrap; font-weight: 600;"">
                                                       <font face=""'Source Sans Pro', sans-serif"" color=""#ffffff"" style=""font-size: 20px; line-height: 30px; text-decoration: none; white-space: nowrap; font-weight: 600;"">
                                                          <span style=""font-family: 'Source Sans Pro', Arial, Verdana, Tahoma, Geneva, sans-serif; color: #ffffff; font-size: 20px; line-height: 30px; text-decoration: none; white-space: nowrap; font-weight: 600;"">
                                                          Reset Password
                                                          </span>
                                                       </font>
                                                    </a>
                                                 </td>
                                              </tr>
                                           </table>
                                           <div style=""height: 20px; line-height: 20px; font-size: 18px;"">&nbsp;</div>
                                           <font face=""'Source Sans Pro', sans-serif"" color=""#585858"" style=""font-size: 24px; line-height: 32px;"">
                                              <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #585858; font-size: 24px; line-height: 32px;"">
                                              If you have trouble seeing the link button above, please <a href=""$LINK$"" target=""_blank"">follow the link here</a>.
                                              </span>
                                           </font>
                                           <div style=""height: 20px; line-height: 20px; font-size: 18px;"">&nbsp;</div>
                                           <font face=""'Source Sans Pro', sans-serif"" color=""#585858"" style=""font-size: 24px; line-height: 32px;"">
                                              <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #585858; font-size: 24px; line-height: 32px;"">
                                              If you did not request a password reset, please <a href=""mailto:$SUPPORTEMAIL$"">contact us</a>.
                                              </span>
                                           </font>
                                           <div style=""height: 75px; line-height: 75px; font-size: 73px;"">&nbsp;</div>
                                        </td>
                                     </tr>
                                  </table>

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""width: 100% !important; min-width: 100%; max-width: 100%; background: #f3f3f3;"">
                                     <tr>
                                        <td align=""center"" valign=""top"">
                                           <div style=""height: 15px; line-height: 15px; font-size: 12px;"">&nbsp;</div>
                                           <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
                                              <tr>
                                                 <td align=""center"" valign=""top"">
                                                    <div style=""height: 34px; line-height: 34px; font-size: 32px;"">&nbsp;</div>
                                                    <font face=""'Source Sans Pro', sans-serif"" color=""#868686"" style=""font-size: 17px; line-height: 20px;"">
                                                       <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #868686; font-size: 17px; line-height: 20px;"">
                                                        Copyright &copy; <a href=""https://bastille.id"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">Bastille.Id</a>. All&nbsp;Rights&nbsp;Reserved.
                                                       </span>
                                                    </font>
                                                    <div style=""height: 3px; line-height: 3px; font-size: 1px;"">&nbsp;</div>
                                                    <font face=""'Source Sans Pro', sans-serif"" color=""#1a1a1a"" style=""font-size: 17px; line-height: 20px;"">
                                                       <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px;"">
                                                        <a href=""mailto:support@bastille.id"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">
                                                         support@bastille.id
                                                        </a>
                                                        &nbsp;&nbsp;|&nbsp;&nbsp;
                                                        <a href=""phone:1-800-555-1234"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">
                                                         1-800-555-1234
                                                        </a>
                                                        &nbsp;&nbsp;|&nbsp;&nbsp;
                                                        <a href=""/Account#Email"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">
                                                         E-mail Preferences
                                                        </a>
                                                       </span>
                                                    </font>
                                                    <div style=""height: 35px; line-height: 35px; font-size: 33px;"">&nbsp;</div>
                                                    <table cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                       <tr>
                                                          <td align=""center"" valign=""top"">
                                                             <a href=""https://facebook.com/Talegen"" target=""_blank"" style=""display: block; max-width: 19px;"">
                                                                <img src=""https://www.bastille.id/wp-content/uploads/2021/04/soc_faceb.png"" alt=""img"" width=""19"" border=""0"" style=""display: block; width: 19px;"" />
                                                             </a>
                                                          </td>
                                                          <td width=""45"" style=""width: 45px; max-width: 45px; min-width: 45px;"">&nbsp;</td>
                                                          <td align=""center"" valign=""top"">
                                                             <a href=""https://instagram.com/TalegenInc"" target=""_blank"" style=""display: block; max-width: 18px;"">
                                                                <img src=""https://www.bastille.id/wp-content/uploads/2021/04/soc_insta.png"" alt=""img"" width=""18"" border=""0"" style=""display: block; width: 18px;"" />
                                                             </a>
                                                          </td>
                                                          <td width=""45"" style=""width: 45px; max-width: 45px; min-width: 45px;"">&nbsp;</td>
                                                          <td align=""center"" valign=""top"">
                                                             <a href=""https://twitter.com/TalegenInc"" target=""_blank"" style=""display: block; max-width: 21px;"">
                                                                <img src=""https://www.bastille.id/wp-content/uploads/2021/04/soc_twitt.png"" alt=""img"" width=""21"" border=""0"" style=""display: block; width: 21px;"" />
                                                             </a>
                                                          </td>
                                                       </tr>
                                                    </table>
                                                    <div style=""height: 35px; line-height: 35px; font-size: 33px;"">&nbsp;</div>
                                                 </td>
                                              </tr>
                                           </table>
                                        </td>
                                     </tr>
                                  </table>

                               </td>
                               <td class=""mob_pad"" width=""25"" style=""width: 25px; max-width: 25px; min-width: 25px;"">&nbsp;</td>
                            </tr>
                         </table>
                         <!--[if (gte mso 9)|(IE)]>
                         </td></tr>
                         </table><![endif]-->
                      </td>
                   </tr>
                </table>
                </body>
                </html>", null, new DateTime(2021, 2, 17, 5, 2, 53, 457, DateTimeKind.Utc).AddTicks(934), LocaleExtensions.DefaultLanguageCode, null, "reset_password_en-US_htm", "Message", null, null },
                    { new Guid("9a151f65-7673-4e88-9f77-f2104a76de64"), @"You're almost there! Please click the link below to verify your e-mail address and activate your account.

                Please click the link below to verify your e-mail address and activate your account.

                $LINK$

                If you did not request an account from us, please contact us at $SUPPORTEMAIL$.", null, new DateTime(2021, 2, 17, 5, 2, 53, 457, DateTimeKind.Utc).AddTicks(1912), LocaleExtensions.DefaultLanguageCode, null, "verify_account_en-US_txt", "Message", null, null },
                    { new Guid("34c00483-ae50-465b-b8d1-4fb9c82dfd8e"), @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
                <html>
                <head>
                <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" >
                <title>Verify Your Account</title>
                <style type=""text/css"">
                html { -webkit-text-size-adjust: none; -ms-text-size-adjust: none;}

                    @media only screen and (min-device-width: 750px) {
                        .table750 {width: 750px !important;}
                    }
                    @media only screen and (max-device-width: 750px), only screen and (max-width: 750px){
                      table[class=""table750""] {width: 100% !important;}
                      .mob_b {width: 93% !important; max-width: 93% !important; min-width: 93% !important;}
                      .mob_b1 {width: 100% !important; max-width: 100% !important; min-width: 100% !important;}
                      .mob_left {text-align: left !important;}
                      .mob_soc {width: 50% !important; max-width: 50% !important; min-width: 50% !important;}
                      .mob_menu {width: 50% !important; max-width: 50% !important; min-width: 50% !important; box-shadow: inset -1px -1px 0 0 rgba(255, 255, 255, 0.2); }
                      .mob_center {text-align: center !important;}
                      .top_pad {height: 15px !important; max-height: 15px !important; min-height: 15px !important;}
                      .mob_pad {width: 15px !important; max-width: 15px !important; min-width: 15px !important;}
                      .mob_div {display: block !important;}
                    }
                   @media only screen and (max-device-width: 550px), only screen and (max-width: 550px){
                      .mod_div {display: block !important;}
                   }
                    .table750 {width: 750px;}
                </style>
                </head>
                <body style=""margin: 0; padding: 0;"">

                <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background: #f3f3f3; min-width: 350px; font-size: 1px; line-height: normal;"">
                    <tr>
                    <td align=""center"" valign=""top"">
                        <!--[if (gte mso 9)|(IE)]>
                         <table border=""0"" cellspacing=""0"" cellpadding=""0"">
                         <tr><td align=""center"" valign=""top"" width=""750""><![endif]-->
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""750"" class=""table750"" style=""width: 100%; max-width: 750px; min-width: 350px; background: #f3f3f3;"">
                            <tr>
                               <td class=""mob_pad"" width=""25"" style=""width: 25px; max-width: 25px; min-width: 25px;"">&nbsp;</td>
                                <td align=""center"" valign=""top"" style=""background: #ffffff;"">

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""width: 100% !important; min-width: 100%; max-width: 100%; background: #f3f3f3;"">
                                     <tr>
                                        <td align=""right"" valign=""top"">
                                           <div class=""top_pad"" style=""height: 25px; line-height: 25px; font-size: 23px;"">&nbsp;</div>
                                        </td>
                                     </tr>
                                  </table>

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
                                     <tr>
                                        <td align=""center"" valign=""top"">
                                           <div style=""height: 39px; line-height: 39px; font-size: 37px;"">&nbsp;</div>
                                           <a href=""https://bastille.id"" target=""_blank"" style=""display: block; max-width: 128px;"" alt=""Bastille.Id"">
                                              <img src=""$URL$/img/logo.png"" alt=""img"" width=""128"" border=""0"" style=""display: block; width: 128px;"" />
                                           </a>
                                           <div style=""height: 32px; line-height: 32px; font-size: 51px;"">&nbsp;</div>
                                        </td>
                                     </tr>
                                  </table>

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
                                     <tr>
                                        <td align=""left"" valign=""top"">
                                           <font face=""'Source Sans Pro', sans-serif"" color=""#585858"" style=""font-size: 24px; line-height: 32px;"">
                                              <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #585858; font-size: 24px; line-height: 32px;"">
                                              You're almost there! Please click the link below to verify your e-mail address and activate your account.
                                              </span>
                                           </font>
                                           <div style=""height: 33px; line-height: 33px; font-size: 31px;"">&nbsp;</div>
                                           <table class=""mob_btn"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background: #27cbcc; border-radius: 4px;"">
                                              <tr>
                                                 <td align=""center"" valign=""top"">
                                                    <a href=""$LINK$"" target=""_blank"" style=""display: block; border: 1px solid #27cbcc; border-radius: 4px; padding: 12px 23px; font-family: 'Source Sans Pro', Arial, Verdana, Tahoma, Geneva, sans-serif; color: #ffffff; font-size: 20px; line-height: 30px; text-decoration: none; white-space: nowrap; font-weight: 600;"">
                                                       <font face=""'Source Sans Pro', sans-serif"" color=""#ffffff"" style=""font-size: 20px; line-height: 30px; text-decoration: none; white-space: nowrap; font-weight: 600;"">
                                                          <span style=""font-family: 'Source Sans Pro', Arial, Verdana, Tahoma, Geneva, sans-serif; color: #ffffff; font-size: 20px; line-height: 30px; text-decoration: none; white-space: nowrap; font-weight: 600;"">
                                                          Verify Account
                                                          </span>
                                                       </font>
                                                    </a>
                                                 </td>
                                              </tr>
                                           </table>
                                           <div style=""height: 20px; line-height: 20px; font-size: 18px;"">&nbsp;</div>
                                           <font face=""'Source Sans Pro', sans-serif"" color=""#585858"" style=""font-size: 24px; line-height: 32px;"">
                                              <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #585858; font-size: 24px; line-height: 32px;"">
                                              If you have trouble seeing the link button above, please <a href=""$LINK$"" target=""_blank"">follow the link here</a>.
                                              </span>
                                           </font>
                                           <div style=""height: 20px; line-height: 20px; font-size: 18px;"">&nbsp;</div>
                                           <font face=""'Source Sans Pro', sans-serif"" color=""#585858"" style=""font-size: 24px; line-height: 32px;"">
                                              <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #585858; font-size: 24px; line-height: 32px;"">
                                              If you did not request an account from us, please <a href=""mailto:$SUPPORTEMAIL$"">contact us</a>.
                                              </span>
                                           </font>
                                           <div style=""height: 75px; line-height: 75px; font-size: 73px;"">&nbsp;</div>
                                        </td>
                                     </tr>
                                  </table>

                                  <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""width: 100% !important; min-width: 100%; max-width: 100%; background: #f3f3f3;"">
                                     <tr>
                                        <td align=""center"" valign=""top"">
                                           <div style=""height: 15px; line-height: 15px; font-size: 12px;"">&nbsp;</div>
                                           <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
                                              <tr>
                                                 <td align=""center"" valign=""top"">
                                                    <div style=""height: 34px; line-height: 34px; font-size: 32px;"">&nbsp;</div>
                                                    <font face=""'Source Sans Pro', sans-serif"" color=""#868686"" style=""font-size: 17px; line-height: 20px;"">
                                                       <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #868686; font-size: 17px; line-height: 20px;"">
                                                        Copyright &copy; <a href=""https://bastille.id"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">Bastille.Id</a>. All&nbsp;Rights&nbsp;Reserved.
                                                       </span>
                                                    </font>
                                                    <div style=""height: 3px; line-height: 3px; font-size: 1px;"">&nbsp;</div>
                                                    <font face=""'Source Sans Pro', sans-serif"" color=""#1a1a1a"" style=""font-size: 17px; line-height: 20px;"">
                                                       <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px;"">
                                                        <a href=""mailto:support@bastille.id"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">
                                                         support@bastille.id
                                                        </a>
                                                        &nbsp;&nbsp;|&nbsp;&nbsp;
                                                        <a href=""phone:1-800-555-1234"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">
                                                         1-800-555-1234
                                                        </a>
                                                        &nbsp;&nbsp;|&nbsp;&nbsp;
                                                        <a href=""/Account#Email"" target=""_blank"" style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 17px; line-height: 20px; text-decoration: none;"">
                                                         E-mail Preferences
                                                        </a>
                                                       </span>
                                                    </font>
                                                    <div style=""height: 35px; line-height: 35px; font-size: 33px;"">&nbsp;</div>
                                                    <table cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                       <tr>
                                                          <td align=""center"" valign=""top"">
                                                             <a href=""https://facebook.com/Talegen"" target=""_blank"" style=""display: block; max-width: 19px;"">
                                                                <img src=""https://www.bastille.id/wp-content/uploads/2021/04/soc_faceb.png"" alt=""img"" width=""19"" border=""0"" style=""display: block; width: 19px;"" />
                                                             </a>
                                                          </td>
                                                          <td width=""45"" style=""width: 45px; max-width: 45px; min-width: 45px;"">&nbsp;</td>
                                                          <td align=""center"" valign=""top"">
                                                             <a href=""https://instagram.com/TalegenInc"" target=""_blank"" style=""display: block; max-width: 18px;"">
                                                                <img src=""https://www.bastille.id/wp-content/uploads/2021/04/soc_insta.png"" alt=""img"" width=""18"" border=""0"" style=""display: block; width: 18px;"" />
                                                             </a>
                                                          </td>
                                                          <td width=""45"" style=""width: 45px; max-width: 45px; min-width: 45px;"">&nbsp;</td>
                                                          <td align=""center"" valign=""top"">
                                                             <a href=""https://twitter.com/TalegenInc"" target=""_blank"" style=""display: block; max-width: 21px;"">
                                                                <img src=""https://www.bastille.id/wp-content/uploads/2021/04/soc_twitt.png"" alt=""img"" width=""21"" border=""0"" style=""display: block; width: 21px;"" />
                                                             </a>
                                                          </td>
                                                       </tr>
                                                    </table>
                                                    <div style=""height: 35px; line-height: 35px; font-size: 33px;"">&nbsp;</div>
                                                 </td>
                                              </tr>
                                           </table>
                                        </td>
                                     </tr>
                                  </table>

                               </td>
                               <td class=""mob_pad"" width=""25"" style=""width: 25px; max-width: 25px; min-width: 25px;"">&nbsp;</td>
                            </tr>
                         </table>
                         <!--[if (gte mso 9)|(IE)]>
                         </td></tr>
                         </table><![endif]-->
                      </td>
                   </tr>
                </table>
                </body>
                </html>", null, new DateTime(2021, 2, 17, 5, 2, 53, 457, DateTimeKind.Utc).AddTicks(2614), LocaleExtensions.DefaultLanguageCode, null, "verify_account_en-US_htm", "Message", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatedByUserId",
                table: "Groups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OrganizationId",
                table: "Groups",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ParentGroupId",
                table: "Groups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_UpdatedByUserId",
                table: "Groups",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUsers_UserId",
                table: "GroupUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRoleClaims_RoleId",
                table: "IdentityRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRoles_CreatedByUserId",
                table: "IdentityRoles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "IdentityRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRoles_UpdatedByUserId",
                table: "IdentityRoles",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserClaims_UserId",
                table: "IdentityUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserLogins_UserId",
                table: "IdentityUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRoles_RoleId",
                table: "IdentityUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "IdentityUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "IdentityUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Name",
                table: "Languages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OwnerUserId",
                table: "Organizations",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedByUserId",
                table: "Templates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_LanguageCode",
                table: "Templates",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_TemplateKey",
                table: "Templates",
                column: "TemplateKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedByUserId",
                table: "Templates",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantConfigs_CreatedByUserId",
                table: "TenantConfigs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantConfigs_OrganizationId",
                table: "TenantConfigs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantConfigs_UpdatedByUserId",
                table: "TenantConfigs",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeZones_ShortName",
                table: "TimeZones",
                column: "ShortName",
                unique: true,
                filter: "[ShortName] IS NOT NULL");
        }

        /// <summary>
        /// <para>Builds the operations that will migrate the database 'down'.</para>
        /// <para>
        /// That is, builds the operations that will take the database from the state left in by this migration so that it returns to the state that it was in
        /// before this migration was applied.
        /// </para>
        /// <para>
        /// This method must be overridden in each class the inherits from <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Migration" /> if both 'up' and
        /// 'down' migrations are to be supported. If it is not overridden, then calling it will throw and it will not be possible to migrate in the 'down' direction.
        /// </para>
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder" /> that will build the operations.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "GroupUsers");

            migrationBuilder.DropTable(
                name: "IdentityRoleClaims");

            migrationBuilder.DropTable(
                name: "IdentityUserClaims");

            migrationBuilder.DropTable(
                name: "IdentityUserLogins");

            migrationBuilder.DropTable(
                name: "IdentityUserRoles");

            migrationBuilder.DropTable(
                name: "IdentityUserTokens");

            migrationBuilder.DropTable(
                name: "OrganizationAllowedDomains");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "TenantConfigs");

            migrationBuilder.DropTable(
                name: "TimeZones");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "IdentityRoles");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "IdentityUsers");
        }
    }
}