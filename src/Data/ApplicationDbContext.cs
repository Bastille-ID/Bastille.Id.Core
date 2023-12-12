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

namespace Bastille.Id.Core.Data
{
    using System;
    using Bastille.Id.Core.Security;
    using Bastille.Id.Core.Data.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using TimeZone = Entities.TimeZone;
    using Bastille.Id.Core.Properties;
    using Talegen.Common.Messaging.Templates;
    using Template = Entities.Template;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class implements and extends the Identity Database Context to support extended VIDS tables and entities.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        #region Private Fields

        /// <summary>
        /// The connection string
        /// </summary>
        private readonly string connectionString;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ApplicationDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext" /> class.
        /// </summary>
        /// <param name="options">Contains additional database context initialization options for the given database context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #endregion

        #region Public Entity Navigation Properties

        /// <summary>
        /// Gets or sets the organization allowed domains.
        /// </summary>
        /// <value>The organization allowed domains.</value>
        public DbSet<OrganizationAllowedDomain> OrganizationAllowedDomains { get; set; }

        /// <summary>
        /// Gets or sets security log events within the identity server data store.
        /// </summary>
        public DbSet<AuditLog> AuditLogs { get; set; }

        /// <summary>
        /// Gets or sets groups/tenants within the identity server data store.
        /// </summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>
        /// Gets or sets organization user associations within the identity server data store.
        /// </summary>
        public DbSet<GroupUser> GroupUsers { get; set; }

        /// <summary>
        /// Gets or sets languages within the identity server data store.
        /// </summary>
        public DbSet<Language> Languages { get; set; }

        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>The organizations.</value>
        public DbSet<Organization> Organizations { get; set; }

        /// <summary>
        /// Gets or sets time zones within the identity server data store.
        /// </summary>
        public DbSet<TimeZone> TimeZones { get; set; }

        /// <summary>
        /// Gets or sets the tenant configs.
        /// </summary>
        /// <value>The tenant configs.</value>
        public DbSet<TenantConfig> TenantConfigs { get; set; }

        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>The templates.</value>
        public DbSet<Template> Templates { get; set; }

        /// <summary>
        /// Gets or sets the notifications.
        /// </summary>
        /// <value>The notifications.</value>
        public DbSet<Notification> Notifications { get; set; }

        #endregion

        #region Entity Framework Protected Methods

        /// <summary>
        /// <para>
        /// Override this method to configure the database (and other options) to be used for this context. This method is called for each instance of the
        /// context that is created. The base implementation does nothing.
        /// </para>
        /// </summary>
        /// <param name="optionsBuilder">
        /// A builder used to create or modify options for this context. Databases (and other extensions) typically define extension methods on this object that
        /// allow you to configure the context.
        /// </param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this.connectionString);
            }
        }

        /// <summary>
        /// This method is called automatically when generating models inside EF code-first.
        /// </summary>
        /// <param name="builder">Contains an instance of the EF model builder.</param>
        /// <remarks>This is the method where specifics about entities inside the overall database design is specified.</remarks>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // build the initial identity models.
            base.OnModelCreating(builder);

            // Default Identity names (e.g. AspNetUsers) are ugly. Use clean up some table and column names.
            builder.Entity<User>().ToTable("IdentityUsers").Property(p => p.Id).HasColumnName("UserId");
            builder.Entity<User>().HasKey(u => u.Id);
            builder.Entity<Role>().ToTable("IdentityRoles").Property(p => p.Id).HasColumnName("RoleId");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("IdentityUserRoles");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("IdentityRoleClaims").Property(p => p.Id).HasColumnName("RoleClaimId");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("IdentityUserClaims").Property(p => p.Id).HasColumnName("UserClaimId");
            builder.Entity<IdentityUserClaim<Guid>>()
                .HasOne("Bastille.Id.Core.Data.Entities.User", null)
                .WithMany("Claims")
                .HasForeignKey("UserId");

            builder.Entity<IdentityUserLogin<Guid>>().ToTable("IdentityUserLogins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("IdentityUserTokens");
            builder.Entity<User>().HasKey(u => u.Id);
            builder.Entity<Role>().HasKey(r => r.Id);

            // create composite keys
            builder.Entity<GroupUser>().HasKey(groupUser => new { groupUser.GroupId, groupUser.UserId });
            builder.Entity<OrganizationAllowedDomain>().HasKey(od => new { od.OrganizationId, od.Domain });

            builder.Entity<TimeZone>().HasIndex(a => a.ShortName).IsUnique();
            builder.Entity<Language>().HasIndex(a => a.Name).IsUnique();
            builder.Entity<Language>().Property(l => l.LanguageCode).HasColumnType("char(5)");

            builder.Entity<Template>().HasIndex(t => t.TemplateKey).IsUnique();

            // set all computed column sql statements
            string getUtcDate = "getutcdate()";
            string newId = "newid()";

            builder.Entity<AuditLog>().Property(a => a.EventDateTime).HasDefaultValueSql(getUtcDate);
            builder.Entity<Group>().Property(a => a.GroupId).HasDefaultValueSql(newId);
            builder.Entity<Group>().Property(a => a.CreatedDate).HasDefaultValueSql(getUtcDate);
            builder.Entity<GroupUser>().Property(a => a.CreatedDate).HasDefaultValueSql(getUtcDate);

            builder.Entity<Organization>().Property(a => a.OrganizationId).HasDefaultValueSql(newId);
            builder.Entity<Organization>().Property(a => a.CreatedDate).HasDefaultValueSql(getUtcDate);

            builder.Entity<Role>().Property(a => a.Id).HasDefaultValueSql(newId);
            builder.Entity<Role>().Property(a => a.CreatedDate).HasDefaultValueSql(getUtcDate);

            builder.Entity<Template>().Property(a => a.TemplateId).HasDefaultValueSql(newId);
            builder.Entity<Template>().Property(a => a.CreatedDate).HasDefaultValueSql(getUtcDate);

            builder.Entity<TenantConfig>().Property(a => a.TenantId).HasDefaultValueSql(newId);
            builder.Entity<TenantConfig>().Property(a => a.CreatedDate).HasDefaultValueSql(getUtcDate);

            builder.Entity<User>().Property(a => a.Id).HasDefaultValueSql(newId);
            builder.Entity<User>().Property(a => a.CreatedDate).HasDefaultValueSql(getUtcDate);

            builder.Entity<Notification>().Property(n => n.NotificationId).HasDefaultValueSql(newId);
            builder.Entity<Notification>().Property(n => n.NotificationDate).HasDefaultValueSql(getUtcDate);

            // data seeding add languages
            builder.Entity<Language>().HasData(
                    new Language { LanguageCode = "af-ZA", Name = "Afrikaans (South Africa)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-AE", Name = "Arabic (U.A.E.)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-AR", Name = "Arabic", Default = false, Active = true },
                    new Language { LanguageCode = "ar-BH", Name = "Arabic (Bahrain)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-DZ", Name = "Arabic (Algeria)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-EG", Name = "Arabic (Egypt)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-IQ", Name = "Arabic (Iraq)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-JO", Name = "Arabic (Jordan)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-KW", Name = "Arabic (Kuwait)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-LB", Name = "Arabic (Lebanon)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-LY", Name = "Arabic (Libya)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-MA", Name = "Arabic (Morocco)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-OM", Name = "Arabic (Oman)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-QA", Name = "Arabic (Qatar)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-SA", Name = "Arabic (Saudi Arabia)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-SY", Name = "Arabic (Syria)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-TN", Name = "Arabic (Tunisia)", Default = false, Active = true },
                    new Language { LanguageCode = "ar-YE", Name = "Arabic (Yemen)", Default = false, Active = true },
                    new Language { LanguageCode = "be-BY", Name = "Belarusian (Belarus)", Default = false, Active = true },
                    new Language { LanguageCode = "bg-BG", Name = "Bulgarian", Default = false, Active = true },
                    new Language { LanguageCode = "ca-ES", Name = "Catalan", Default = false, Active = true },
                    new Language { LanguageCode = "cs-CZ", Name = "Czech", Default = false, Active = true },
                    new Language { LanguageCode = "da-DK", Name = "Danish (Denmark)", Default = false, Active = true },
                    new Language { LanguageCode = "de-AT", Name = "German (Austria)", Default = false, Active = true },
                    new Language { LanguageCode = "de-CH", Name = "German (Switzerland)", Default = false, Active = true },
                    new Language { LanguageCode = "de-DE", Name = "German", Default = false, Active = true },
                    new Language { LanguageCode = "de-LI", Name = "German (Lechtenstein)", Default = false, Active = true },
                    new Language { LanguageCode = "de-LU", Name = "German (Luxembourg)", Default = false, Active = true },
                    new Language { LanguageCode = "el-GR", Name = "Greek", Default = false, Active = true },
                    new Language { LanguageCode = "en-AU", Name = "English (Australia)", Default = false, Active = true },
                    new Language { LanguageCode = "en-CA", Name = "English (Canada)", Default = false, Active = true },
                    new Language { LanguageCode = "en-CB", Name = "English (Caribbean)", Default = false, Active = true },
                    new Language { LanguageCode = "en-GB", Name = "English (UK)", Default = false, Active = true },
                    new Language { LanguageCode = "en-IE", Name = "English (Ireland)", Default = false, Active = true },
                    new Language { LanguageCode = "en-JM", Name = "English (Jamaica)", Default = false, Active = true },
                    new Language { LanguageCode = "en-NZ", Name = "English (New Zealand)", Default = false, Active = true },
                    new Language { LanguageCode = "en-PH", Name = "English (Philippines)", Default = false, Active = true },
                    new Language { LanguageCode = "en-TT", Name = "English (Trinidad & Tobago)", Default = false, Active = true },
                    new Language { LanguageCode = "en-US", Name = "English", Default = true, Active = true, AvailableLocale = true },
                    new Language { LanguageCode = "en-ZA", Name = "English (South Africa)", Default = false, Active = true },
                    new Language { LanguageCode = "en-ZW", Name = "English (Zimbabwe)", Default = false, Active = true },
                    new Language { LanguageCode = "es-AR", Name = "Spanish (Argentina)", Default = false, Active = true },
                    new Language { LanguageCode = "es-BO", Name = "Spanish (Bolivia)", Default = false, Active = true },
                    new Language { LanguageCode = "es-CL", Name = "Spanish (Chile)", Default = false, Active = true },
                    new Language { LanguageCode = "es-CO", Name = "Spanish (Columbia)", Default = false, Active = true },
                    new Language { LanguageCode = "es-CR", Name = "Spanish (Costa Rica)", Default = false, Active = true },
                    new Language { LanguageCode = "es-DO", Name = "Spanish (Dominican Republic)", Default = false, Active = true },
                    new Language { LanguageCode = "es-EC", Name = "Spanish (Ecuador)", Default = false, Active = true },
                    new Language { LanguageCode = "es-ES", Name = "Spanish", Default = false, Active = true },
                    new Language { LanguageCode = "es-GT", Name = "Spanish (Guatemala)", Default = false, Active = true },
                    new Language { LanguageCode = "es-HN", Name = "Spanish (Hondorus)", Default = false, Active = true },
                    new Language { LanguageCode = "es-MX", Name = "Spanish (Mexico)", Default = false, Active = true },
                    new Language { LanguageCode = "es-NI", Name = "Spanish (Nicaragua)", Default = false, Active = true },
                    new Language { LanguageCode = "es-PA", Name = "Spanish (Panama)", Default = false, Active = true },
                    new Language { LanguageCode = "es-PE", Name = "Spanish (Peru)", Default = false, Active = true },
                    new Language { LanguageCode = "es-PR", Name = "Spanish (Puerto Rico)", Default = false, Active = true },
                    new Language { LanguageCode = "es-PY", Name = "Spanish (Paraguay)", Default = false, Active = true },
                    new Language { LanguageCode = "es-SV", Name = "Spanish (El Salvador)", Default = false, Active = true },
                    new Language { LanguageCode = "es-UY", Name = "Spanish (Uruguay)", Default = false, Active = true },
                    new Language { LanguageCode = "es-VE", Name = "Spanish (Venezuela)", Default = false, Active = true },
                    new Language { LanguageCode = "et-EE", Name = "Estonian", Default = false, Active = true },
                    new Language { LanguageCode = "eu-ES", Name = "Basque", Default = false, Active = true },
                    new Language { LanguageCode = "fa-IR", Name = "Farsi (Iran)", Default = false, Active = true },
                    new Language { LanguageCode = "fi-FI", Name = "Finish (Finland)", Default = false, Active = true },
                    new Language { LanguageCode = "fo-FO", Name = "Faroese", Default = false, Active = true },
                    new Language { LanguageCode = "fr-BE", Name = "French (Belgium)", Default = false, Active = true },
                    new Language { LanguageCode = "fr-CA", Name = "French (Canada)", Default = false, Active = true },
                    new Language { LanguageCode = "fr-CH", Name = "French (Switzerland)", Default = false, Active = true },
                    new Language { LanguageCode = "fr-FR", Name = "French", Default = false, Active = true },
                    new Language { LanguageCode = "fr-LU", Name = "French (Luxembourg)", Default = false, Active = true },
                    new Language { LanguageCode = "fr-MC", Name = "French (Manaco)", Default = false, Active = true },
                    new Language { LanguageCode = "gl-ES", Name = "Galician", Default = false, Active = true },
                    new Language { LanguageCode = "gu-IN", Name = "Gujarati (India)", Default = false, Active = true },
                    new Language { LanguageCode = "he-IL", Name = "Hebrew (Israel)", Default = false, Active = true },
                    new Language { LanguageCode = "hi-IN", Name = "Hindi (India)", Default = false, Active = true },
                    new Language { LanguageCode = "hr-HR", Name = "Croatian", Default = false, Active = true },
                    new Language { LanguageCode = "hu-HU", Name = "Hungarian", Default = false, Active = true },
                    new Language { LanguageCode = "hy-AM", Name = "Armenian (Armenia)", Default = false, Active = true },
                    new Language { LanguageCode = "id-ID", Name = "Indonesian", Default = false, Active = true },
                    new Language { LanguageCode = "is-IS", Name = "Icelandic", Default = false, Active = true },
                    new Language { LanguageCode = "it-CH", Name = "Italian (Switzerland)", Default = false, Active = true },
                    new Language { LanguageCode = "it-IT", Name = "Italian", Default = false, Active = true },
                    new Language { LanguageCode = "ja-JP", Name = "Japanese", Default = false, Active = true },
                    new Language { LanguageCode = "ka-GE", Name = "Georgian", Default = false, Active = true },
                    new Language { LanguageCode = "kk-KZ", Name = "Kazakh", Default = false, Active = true },
                    new Language { LanguageCode = "kn-IN", Name = "Kannada - India", Default = false, Active = true },
                    new Language { LanguageCode = "ko-KR", Name = "Korean (Korea)", Default = false, Active = true },
                    new Language { LanguageCode = "ky-KZ", Name = "Kyrgyz", Default = false, Active = true },
                    new Language { LanguageCode = "lt-LT", Name = "Lithuanian", Default = false, Active = true },
                    new Language { LanguageCode = "lv-LV", Name = "Latvian", Default = false, Active = true },
                    new Language { LanguageCode = "mk-MK", Name = "Macedonian", Default = false, Active = true },
                    new Language { LanguageCode = "mn-MN", Name = "Mongolian", Default = false, Active = true },
                    new Language { LanguageCode = "mr-IN", Name = "Marathi (India)", Default = false, Active = true },
                    new Language { LanguageCode = "ms-BN", Name = "Malay (Brunei)", Default = false, Active = true },
                    new Language { LanguageCode = "ms-MY", Name = "Malay (Malaysia)", Default = false, Active = true },
                    new Language { LanguageCode = "nb-NO", Name = "Norwegian (Bokmal)", Default = false, Active = true },
                    new Language { LanguageCode = "nl-BE", Name = "Dutch (Belgium)", Default = false, Active = true },
                    new Language { LanguageCode = "nl-NL", Name = "Dutch (Standard)", Default = false, Active = true },
                    new Language { LanguageCode = "nn-NO", Name = "Norwegian (Nynorsk)", Default = false, Active = true },
                    new Language { LanguageCode = "pa-IN", Name = "Punjabi (India)", Default = false, Active = true },
                    new Language { LanguageCode = "pl-PL", Name = "Polish", Default = false, Active = true },
                    new Language { LanguageCode = "pt-BR", Name = "Portuguese (Brazil)", Default = false, Active = true },
                    new Language { LanguageCode = "pt-PT", Name = "Portuguese (Portugal)", Default = false, Active = true },
                    new Language { LanguageCode = "ro-RO", Name = "Romanian", Default = false, Active = true },
                    new Language { LanguageCode = "ru-RU", Name = "Russian", Default = false, Active = true },
                    new Language { LanguageCode = "sa-IN", Name = "Sanskrit (India)", Default = false, Active = true },
                    new Language { LanguageCode = "sk-SK", Name = "Slovak", Default = false, Active = true },
                    new Language { LanguageCode = "sl-SI", Name = "Slovenian", Default = false, Active = true },
                    new Language { LanguageCode = "sq-AL", Name = "Albania", Default = false, Active = true },
                    new Language { LanguageCode = "sv-FI", Name = "Swedish (Finland)", Default = false, Active = true },
                    new Language { LanguageCode = "sv-SE", Name = "Swedish (Sweden)", Default = false, Active = true },
                    new Language { LanguageCode = "sw-KE", Name = "Swahili (Kenya)", Default = false, Active = true },
                    new Language { LanguageCode = "sy-SY", Name = "Syriac", Default = false, Active = true },
                    new Language { LanguageCode = "ta-IN", Name = "Tamil (India)", Default = false, Active = true },
                    new Language { LanguageCode = "te-IN", Name = "Telugu (India)", Default = false, Active = true },
                    new Language { LanguageCode = "th-TH", Name = "Thai (Thailand)", Default = false, Active = true },
                    new Language { LanguageCode = "tr-TR", Name = "Turkish", Default = false, Active = true },
                    new Language { LanguageCode = "tt-RU", Name = "Tatar (Russia)", Default = false, Active = true },
                    new Language { LanguageCode = "tx-TX", Name = "Texas", Default = false, Active = true },
                    new Language { LanguageCode = "uk-UA", Name = "Ukrainian", Default = false, Active = true },
                    new Language { LanguageCode = "ur-PK", Name = "Urdu (Pakistan)", Default = false, Active = true },
                    new Language { LanguageCode = "vi-VN", Name = "Vietnamese", Default = false, Active = true },
                    new Language { LanguageCode = "zh-CN", Name = "Chinese (PRC)", Default = false, Active = true },
                    new Language { LanguageCode = "zh-HK", Name = "Chinese (Hon Kong SAR)", Default = false, Active = true },
                    new Language { LanguageCode = "zh-MO", Name = "Chinese (Macau SAR)", Default = false, Active = true },
                    new Language { LanguageCode = "zh-SG", Name = "Chinese (Singapore)", Default = false, Active = true },
                    new Language { LanguageCode = "zh-TW", Name = "Chinese (Taiwan)", Default = false, Active = true });

            // add timezones
            builder.Entity<TimeZone>().HasData(
                    new Core.Data.Entities.TimeZone { ShortName = "Dateline Standard Time", LongName = "(UTC-12:00) International Date Line West", Offset = -12, TimeZoneId = "Etc/GMT+12", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "UTC-11", LongName = "(UTC-11:00) Coordinated Universal Time-11", Offset = -11, TimeZoneId = "Etc/GMT+11", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Aleutian Standard Time", LongName = "(UTC-10:00) Aleutian Islands", Offset = -10, TimeZoneId = "America/Adak", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Hawaiian Standard Time", LongName = "(UTC-10:00) Hawaii", Offset = -10, TimeZoneId = "Pacific/Honolulu", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Marquesas Standard Time", LongName = "(UTC-09:30) Marquesas Islands", Offset = -9.5, TimeZoneId = "Pacific/Marquesas", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Alaskan Standard Time", LongName = "(UTC-09:00) Alaska", Offset = -9, TimeZoneId = "America/Anchorage", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "UTC-09", LongName = "(UTC-09:00) Coordinated Universal Time-09", Offset = -9, TimeZoneId = "Etc/GMT+9", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Pacific Standard Time (Mexico)", LongName = "(UTC-08:00) Baja California", Offset = -8, TimeZoneId = "America/Tijuana", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "UTC-08", LongName = "(UTC-08:00) Coordinated Universal Time-08", Offset = -8, TimeZoneId = "Etc/GMT+8", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Pacific Standard Time", LongName = "(UTC-08:00) Pacific Time (US & Canada)", Offset = -8, TimeZoneId = "America/Los_Angeles", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "US Mountain Standard Time", LongName = "(UTC-07:00) Arizona", Offset = -7, TimeZoneId = "America/Phoenix", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Mountain Standard Time (Mexico)", LongName = "(UTC-07:00) Chihuahua, La Paz, Mazatlan", Offset = -7, TimeZoneId = "America/Chihuahua", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Mountain Standard Time", LongName = "(UTC-07:00) Mountain Time (US & Canada)", Offset = -7, TimeZoneId = "America/Denver", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central America Standard Time", LongName = "(UTC-06:00) Central America", Offset = -6, TimeZoneId = "America/Guatemala", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central Standard Time", LongName = "(UTC-06:00) Central Time (US & Canada)", Offset = -6, TimeZoneId = "America/Chicago", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Easter Island Standard Time", LongName = "(UTC-06:00) Easter Island", Offset = -6, TimeZoneId = "Pacific/Easter", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central Standard Time (Mexico)", LongName = "(UTC-06:00) Guadalajara, Mexico City, Monterrey", Offset = -6, TimeZoneId = "America/Mexico_City", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Canada Central Standard Time", LongName = "(UTC-06:00) Saskatchewan", Offset = -6, TimeZoneId = "America/Regina", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "SA Pacific Standard Time", LongName = "(UTC-05:00) Bogota, Lima, Quito, Rio Branco", Offset = -5, TimeZoneId = "America/Bogota", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Eastern Standard Time (Mexico)", LongName = "(UTC-05:00) Chetumal", Offset = -5, TimeZoneId = "America/Cancun", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Eastern Standard Time", LongName = "(UTC-05:00) Eastern Time (US & Canada)", Offset = -5, TimeZoneId = "America/New_York", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Haiti Standard Time", LongName = "(UTC-05:00) Haiti", Offset = -5, TimeZoneId = "America/Port-au-Prince", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Cuba Standard Time", LongName = "(UTC-05:00) Havana", Offset = -5, TimeZoneId = "America/Havana", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "US Eastern Standard Time", LongName = "(UTC-05:00) Indiana (East)", Offset = -5, TimeZoneId = "America/Indiana/Indianapolis", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Paraguay Standard Time", LongName = "(UTC-04:00) Asuncion", Offset = -4, TimeZoneId = "America/Asuncion", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Atlantic Standard Time", LongName = "(UTC-04:00) Atlantic Time (Canada)", Offset = -4, TimeZoneId = "America/Halifax", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Venezuela Standard Time", LongName = "(UTC-04:00) Caracas", Offset = -4, TimeZoneId = "America/Caracas", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central Brazilian Standard Time", LongName = "(UTC-04:00) Cuiaba", Offset = -4, TimeZoneId = "America/Cuiaba", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "SA Western Standard Time", LongName = "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan", Offset = -4, TimeZoneId = "America/La_Paz", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Pacific SA Standard Time", LongName = "(UTC-04:00) Santiago", Offset = -4, TimeZoneId = "America/Santiago", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Turks And Caicos Standard Time", LongName = "(UTC-04:00) Turks and Caicos", Offset = -4, TimeZoneId = "America/Grand_Turk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Newfoundland Standard Time", LongName = "(UTC-03:30) Newfoundland", Offset = -3.5, TimeZoneId = "America/St_Johns", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Tocantins Standard Time", LongName = "(UTC-03:00) Araguaina", Offset = -3, TimeZoneId = "America/Araguaina", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "E. South America Standard Time", LongName = "(UTC-03:00) Brasilia", Offset = -3, TimeZoneId = "America/Sao_Paulo", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "SA Eastern Standard Time", LongName = "(UTC-03:00) Cayenne, Fortaleza", Offset = -3, TimeZoneId = "America/Cayenne", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Argentina Standard Time", LongName = "(UTC-03:00) City of Buenos Aires", Offset = -3, TimeZoneId = "America/Argentina/Buenos_Aires", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Greenland Standard Time", LongName = "(UTC-03:00) Greenland", Offset = -3, TimeZoneId = "America/Godthab", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Montevideo Standard Time", LongName = "(UTC-03:00) Montevideo", Offset = -3, TimeZoneId = "America/Montevideo", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Saint Pierre Standard Time", LongName = "(UTC-03:00) Saint Pierre and Miquelon", Offset = -3, TimeZoneId = "America/Miquelon", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Bahia Standard Time", LongName = "(UTC-03:00) Salvador", Offset = -3, TimeZoneId = "America/Bahia", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "UTC-02", LongName = "(UTC-02:00) Coordinated Universal Time-02", Offset = -2, TimeZoneId = "Etc/GMT+2", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Azores Standard Time", LongName = "(UTC-01:00) Azores", Offset = -1, TimeZoneId = "Atlantic/Azores", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Cape Verde Standard Time", LongName = "(UTC-01:00) Cabo Verde Is.", Offset = -1, TimeZoneId = "Atlantic/Cape_Verde", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "UTC", LongName = "(UTC) Coordinated Universal Time", Offset = 0, TimeZoneId = "Etc/UTC", Default = true, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Morocco Standard Time", LongName = "(UTC+00:00) Casablanca", Offset = 0, TimeZoneId = "Africa/Casablanca", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "GMT Standard Time", LongName = "(UTC+00:00) Dublin, Edinburgh, Lisbon, London", Offset = 0, TimeZoneId = "Europe/London", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Greenwich Standard Time", LongName = "(UTC+00:00) Monrovia, Reykjavik", Offset = 0, TimeZoneId = "Atlantic/Reykjavik", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "W. Europe Standard Time", LongName = "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna", Offset = 1, TimeZoneId = "Europe/Berlin", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central Europe Standard Time", LongName = "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague", Offset = 1, TimeZoneId = "Europe/Budapest", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Romance Standard Time", LongName = "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris", Offset = 1, TimeZoneId = "Europe/Paris", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central European Standard Time", LongName = "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb", Offset = 1, TimeZoneId = "Europe/Warsaw", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "W. Central Africa Standard Time", LongName = "(UTC+01:00) West Central Africa", Offset = 1, TimeZoneId = "Africa/Lagos", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Namibia Standard Time", LongName = "(UTC+01:00) Windhoek", Offset = 1, TimeZoneId = "Africa/Windhoek", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Jordan Standard Time", LongName = "(UTC+02:00) Amman", Offset = 2, TimeZoneId = "Asia/Amman", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "GTB Standard Time", LongName = "(UTC+02:00) Athens, Bucharest", Offset = 2, TimeZoneId = "Europe/Bucharest", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Middle East Standard Time", LongName = "(UTC+02:00) Beirut", Offset = 2, TimeZoneId = "Asia/Beirut", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Egypt Standard Time", LongName = "(UTC+02:00) Cairo", Offset = 2, TimeZoneId = "Africa/Cairo", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "E. Europe Standard Time", LongName = "(UTC+02:00) Chisinau", Offset = 2, TimeZoneId = "Europe/Chisinau", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Syria Standard Time", LongName = "(UTC+02:00) Damascus", Offset = 2, TimeZoneId = "Asia/Damascus", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "West Bank Standard Time", LongName = "(UTC+02:00) Gaza, Hebron", Offset = 2, TimeZoneId = "Asia/Hebron", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "South Africa Standard Time", LongName = "(UTC+02:00) Harare, Pretoria", Offset = 2, TimeZoneId = "Africa/Johannesburg", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "FLE Standard Time", LongName = "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius", Offset = 2, TimeZoneId = "Europe/Kiev", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Turkey Standard Time", LongName = "(UTC+02:00) Istanbul", Offset = 2, TimeZoneId = "Europe/Istanbul", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Israel Standard Time", LongName = "(UTC+02:00) Jerusalem", Offset = 2, TimeZoneId = "Asia/Jerusalem", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Kaliningrad Standard Time", LongName = "(UTC+02:00) Kaliningrad", Offset = 2, TimeZoneId = "Europe/Kaliningrad", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Libya Standard Time", LongName = "(UTC+02:00) Tripoli", Offset = 2, TimeZoneId = "Africa/Tripoli", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Arabic Standard Time", LongName = "(UTC+03:00) Baghdad", Offset = 3, TimeZoneId = "Asia/Baghdad", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Arab Standard Time", LongName = "(UTC+03:00) Kuwait, Riyadh", Offset = 3, TimeZoneId = "Asia/Riyadh", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Belarus Standard Time", LongName = "(UTC+03:00) Minsk", Offset = 3, TimeZoneId = "Europe/Minsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Russian Standard Time", LongName = "(UTC+03:00) Moscow, St. Petersburg, Volgograd", Offset = 3, TimeZoneId = "Europe/Moscow", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "E. Africa Standard Time", LongName = "(UTC+03:00) Nairobi", Offset = 3, TimeZoneId = "Africa/Nairobi", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Iran Standard Time", LongName = "(UTC+03:30) Tehran", Offset = 3.5, TimeZoneId = "Asia/Tehran", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Arabian Standard Time", LongName = "(UTC+04:00) Abu Dhabi, Muscat", Offset = 4, TimeZoneId = "Asia/Dubai", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Astrakhan Standard Time", LongName = "(UTC+04:00) Astrakhan, Ulyanovsk", Offset = 4, TimeZoneId = "Europe/Astrakhan", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Azerbaijan Standard Time", LongName = "(UTC+04:00) Baku", Offset = 4, TimeZoneId = "Asia/Baku", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Russia Time Zone 3", LongName = "(UTC+04:00) Izhevsk, Samara", Offset = 4, TimeZoneId = "Europe/Samara", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Mauritius Standard Time", LongName = "(UTC+04:00) Port Louis", Offset = 4, TimeZoneId = "Indian/Mauritius", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Georgian Standard Time", LongName = "(UTC+04:00) Tbilisi", Offset = 4, TimeZoneId = "Asia/Tbilisi", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Caucasus Standard Time", LongName = "(UTC+04:00) Yerevan", Offset = 4, TimeZoneId = "Asia/Yerevan", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Afghanistan Standard Time", LongName = "(UTC+04:30) Kabul", Offset = 4.5, TimeZoneId = "Asia/Kabul", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "West Asia Standard Time", LongName = "(UTC+05:00) Ashgabat, Tashkent", Offset = 5, TimeZoneId = "Asia/Tashkent", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Ekaterinburg Standard Time", LongName = "(UTC+05:00) Ekaterinburg", Offset = 5, TimeZoneId = "Asia/Yekaterinburg", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Pakistan Standard Time", LongName = "(UTC+05:00) Islamabad, Karachi", Offset = 5, TimeZoneId = "Asia/Karachi", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "India Standard Time", LongName = "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi", Offset = 5.5, TimeZoneId = "Asia/Kolkata", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Sri Lanka Standard Time", LongName = "(UTC+05:30) Sri Jayawardenepura", Offset = 5.5, TimeZoneId = "Asia/Colombo", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Nepal Standard Time", LongName = "(UTC+05:45) Kathmandu", Offset = 5.75, TimeZoneId = "Asia/Kathmandu", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central Asia Standard Time", LongName = "(UTC+06:00) Astana", Offset = 6, TimeZoneId = "Asia/Almaty", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Bangladesh Standard Time", LongName = "(UTC+06:00) Dhaka", Offset = 6, TimeZoneId = "Asia/Dhaka", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Omsk Standard Time", LongName = "(UTC+06:00) Omsk", Offset = 6, TimeZoneId = "Asia/Omsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Myanmar Standard Time", LongName = "(UTC+06:30) Yangon (Rangoon)", Offset = 6.5, TimeZoneId = "Asia/Yangon", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "SE Asia Standard Time", LongName = "(UTC+07:00) Bangkok, Hanoi, Jakarta", Offset = 7, TimeZoneId = "Asia/Bangkok", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Altai Standard Time", LongName = "(UTC+07:00) Barnaul, Gorno-Altaysk", Offset = 7, TimeZoneId = "Asia/Barnaul", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "W. Mongolia Standard Time", LongName = "(UTC+07:00) Hovd", Offset = 7, TimeZoneId = "Asia/Hovd", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "North Asia Standard Time", LongName = "(UTC+07:00) Krasnoyarsk", Offset = 7, TimeZoneId = "Asia/Krasnoyarsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "N. Central Asia Standard Time", LongName = "(UTC+07:00) Novosibirsk", Offset = 7, TimeZoneId = "Asia/Novosibirsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Tomsk Standard Time", LongName = "(UTC+07:00) Tomsk", Offset = 7, TimeZoneId = "Asia/Tomsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "China Standard Time", LongName = "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi", Offset = 8, TimeZoneId = "Asia/Shanghai", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "North Asia East Standard Time", LongName = "(UTC+08:00) Irkutsk", Offset = 8, TimeZoneId = "Asia/Irkutsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Singapore Standard Time", LongName = "(UTC+08:00) Kuala Lumpur, Singapore", Offset = 8, TimeZoneId = "Asia/Singapore", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "W. Australia Standard Time", LongName = "(UTC+08:00) Perth", Offset = 8, TimeZoneId = "Australia/Perth", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Taipei Standard Time", LongName = "(UTC+08:00) Taipei", Offset = 8, TimeZoneId = "Asia/Taipei", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Ulaanbaatar Standard Time", LongName = "(UTC+08:00) Ulaanbaatar", Offset = 8, TimeZoneId = "Asia/Ulaanbaatar", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "North Korea Standard Time", LongName = "(UTC+08:30) Pyongyang", Offset = 8.5, TimeZoneId = "Asia/Pyongyang", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Aus Central W. Standard Time", LongName = "(UTC+08:45) Eucla", Offset = 8.75, TimeZoneId = "Australia/Eucla", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Transbaikal Standard Time", LongName = "(UTC+09:00) Chita", Offset = 9, TimeZoneId = "Asia/Chita", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Tokyo Standard Time", LongName = "(UTC+09:00) Osaka, Sapporo, Tokyo", Offset = 9, TimeZoneId = "Asia/Tokyo", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Korea Standard Time", LongName = "(UTC+09:00) Seoul", Offset = 9, TimeZoneId = "Asia/Seoul", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Yakutsk Standard Time", LongName = "(UTC+09:00) Yakutsk", Offset = 9, TimeZoneId = "Asia/Yakutsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Cen. Australia Standard Time", LongName = "(UTC+09:30) Adelaide", Offset = 9.5, TimeZoneId = "Australia/Adelaide", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "AUS Central Standard Time", LongName = "(UTC+09:30) Darwin", Offset = 9.5, TimeZoneId = "Australia/Darwin", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "E. Australia Standard Time", LongName = "(UTC+10:00) Brisbane", Offset = 10, TimeZoneId = "Australia/Brisbane", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "AUS Eastern Standard Time", LongName = "(UTC+10:00) Canberra, Melbourne, Sydney", Offset = 10, TimeZoneId = "Australia/Sydney", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "West Pacific Standard Time", LongName = "(UTC+10:00) Guam, Port Moresby", Offset = 10, TimeZoneId = "Pacific/Port_Moresby", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Tasmania Standard Time", LongName = "(UTC+10:00) Hobart", Offset = 10, TimeZoneId = "Australia/Hobart", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Vladivostok Standard Time", LongName = "(UTC+10:00) Vladivostok", Offset = 10, TimeZoneId = "Asia/Vladivostok", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Lord Howe Standard Time", LongName = "(UTC+10:30) Lord Howe Island", Offset = 10.5, TimeZoneId = "Australia/Lord_Howe", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Bougainville Standard Time", LongName = "(UTC+11:00) Bougainville Island", Offset = 11, TimeZoneId = "Pacific/Bougainville", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Russia Time Zone 10", LongName = "(UTC+11:00) Chokurdakh", Offset = 11, TimeZoneId = "Asia/Srednekolymsk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Magadan Standard Time", LongName = "(UTC+11:00) Magadan", Offset = 11, TimeZoneId = "Asia/Magadan", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Norfolk Standard Time", LongName = "(UTC+11:00) Norfolk Island", Offset = 11, TimeZoneId = "Pacific/Norfolk", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Sakhalin Standard Time", LongName = "(UTC+11:00) Sakhalin", Offset = 11, TimeZoneId = "Asia/Sakhalin", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Central Pacific Standard Time", LongName = "(UTC+11:00) Solomon Is., New Caledonia", Offset = 11, TimeZoneId = "Pacific/Guadalcanal", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "New Zealand Standard Time", LongName = "(UTC+12:00) Auckland, Wellington", Offset = 12, TimeZoneId = "Pacific/Auckland", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "UTC+12", LongName = "(UTC+12:00) Coordinated Universal Time+12", Offset = 12, TimeZoneId = "Etc/GMT-12", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Fiji Standard Time", LongName = "(UTC+12:00) Fiji", Offset = 12, TimeZoneId = "Pacific/Fiji", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Kamchatka Standard Time", LongName = "(UTC+12:00) Petropavlovsk-Kamchatsky - Old", Offset = 12, TimeZoneId = "Asia/Kamchatka", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Chatham Islands Standard Time", LongName = "(UTC+12:45) Chatham Islands", Offset = 12.75, TimeZoneId = "Pacific/Chatham", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Tonga Standard Time", LongName = "(UTC+13:00) Nuku'alofa", Offset = 13, TimeZoneId = "Pacific/Tongatapu", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Samoa Standard Time", LongName = "(UTC+13:00) Samoa", Offset = 13, TimeZoneId = "Pacific/Apia", Default = false, Active = true },
                    new Core.Data.Entities.TimeZone { ShortName = "Line Islands Standard Time", LongName = "(UTC+14:00) Kiritimati Island", Offset = 14, TimeZoneId = "Pacific/Kiritimati", Default = false, Active = true });

            // add base templates
            builder.Entity<Template>().HasData(
                    new Template { TemplateKey = SecurityDefaults.ResetPasswordTemplateName + "_en-US_txt", LanguageCode = LocaleExtensions.DefaultLanguageCode, TemplateType = TemplateType.Message, Content = Resources.reset_password_en_txt },
                    new Template { TemplateKey = SecurityDefaults.ResetPasswordTemplateName + "_en-US_htm", LanguageCode = LocaleExtensions.DefaultLanguageCode, TemplateType = TemplateType.Message, Content = Resources.reset_password_en_htm },
                    new Template { TemplateKey = SecurityDefaults.VerifyAccountTemplateName + "_en-US_txt", LanguageCode = LocaleExtensions.DefaultLanguageCode, TemplateType = TemplateType.Message, Content = Resources.verify_account_en_txt },
                    new Template { TemplateKey = SecurityDefaults.VerifyAccountTemplateName + "_en-US_htm", LanguageCode = LocaleExtensions.DefaultLanguageCode, TemplateType = TemplateType.Message, Content = Resources.verify_account_en_htm }
                );
        }

        #endregion
    }
}