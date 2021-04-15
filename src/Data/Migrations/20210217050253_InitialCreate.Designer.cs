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
    using Bastille.Id.Core.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class implements the Bastille ID Server data schema model.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Migrations.Migration" />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210217050253_InitialCreate")]
    partial class InitialCreate
    {
        /// <summary>
        /// Implemented to builds the <see cref="P:Microsoft.EntityFrameworkCore.Migrations.Migration.TargetModel" />.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="T:Microsoft.EntityFrameworkCore.ModelBuilder" /> to use to build the model.</param>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.AuditLog", b =>
                {
                    b.Property<long>("AuditLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("ClientAddress")
                        .HasColumnType("nvarchar(30)")
                        .HasMaxLength(30);

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<DateTime>("EventDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("Request")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30);

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(450);

                    b.HasKey("AuditLogId");

                    b.HasIndex("UserId");

                    b.ToTable("AuditLogs");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Group", b =>
                {
                    b.Property<Guid>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid?>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OwnerUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ParentGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.HasKey("GroupId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentGroupId");

                    b.HasIndex("UpdatedByUserId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.GroupUser", b =>
                {
                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(450);

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupUsers");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Language", b =>
                {
                    b.Property<string>("LanguageCode")
                        .HasColumnType("char(5)")
                        .HasMaxLength(5);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<bool>("AvailableLocale")
                        .HasColumnType("bit");

                    b.Property<bool>("Default")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("LanguageCode");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Languages");

                    b.HasData(
                        new
                        {
                            LanguageCode = "af-ZA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Afrikaans (South Africa)"
                        },
                        new
                        {
                            LanguageCode = "ar-AE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (U.A.E.)"
                        },
                        new
                        {
                            LanguageCode = "ar-AR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic"
                        },
                        new
                        {
                            LanguageCode = "ar-BH",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Bahrain)"
                        },
                        new
                        {
                            LanguageCode = "ar-DZ",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Algeria)"
                        },
                        new
                        {
                            LanguageCode = "ar-EG",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Egypt)"
                        },
                        new
                        {
                            LanguageCode = "ar-IQ",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Iraq)"
                        },
                        new
                        {
                            LanguageCode = "ar-JO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Jordan)"
                        },
                        new
                        {
                            LanguageCode = "ar-KW",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Kuwait)"
                        },
                        new
                        {
                            LanguageCode = "ar-LB",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Lebanon)"
                        },
                        new
                        {
                            LanguageCode = "ar-LY",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Libya)"
                        },
                        new
                        {
                            LanguageCode = "ar-MA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Morocco)"
                        },
                        new
                        {
                            LanguageCode = "ar-OM",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Oman)"
                        },
                        new
                        {
                            LanguageCode = "ar-QA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Qatar)"
                        },
                        new
                        {
                            LanguageCode = "ar-SA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Saudi Arabia)"
                        },
                        new
                        {
                            LanguageCode = "ar-SY",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Syria)"
                        },
                        new
                        {
                            LanguageCode = "ar-TN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Tunisia)"
                        },
                        new
                        {
                            LanguageCode = "ar-YE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Arabic (Yemen)"
                        },
                        new
                        {
                            LanguageCode = "be-BY",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Belarusian (Belarus)"
                        },
                        new
                        {
                            LanguageCode = "bg-BG",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Bulgarian"
                        },
                        new
                        {
                            LanguageCode = "ca-ES",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Catalan"
                        },
                        new
                        {
                            LanguageCode = "cs-CZ",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Czech"
                        },
                        new
                        {
                            LanguageCode = "da-DK",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Danish (Denmark)"
                        },
                        new
                        {
                            LanguageCode = "de-AT",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "German (Austria)"
                        },
                        new
                        {
                            LanguageCode = "de-CH",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "German (Switzerland)"
                        },
                        new
                        {
                            LanguageCode = "de-DE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "German"
                        },
                        new
                        {
                            LanguageCode = "de-LI",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "German (Lechtenstein)"
                        },
                        new
                        {
                            LanguageCode = "de-LU",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "German (Luxembourg)"
                        },
                        new
                        {
                            LanguageCode = "el-GR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Greek"
                        },
                        new
                        {
                            LanguageCode = "en-AU",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Australia)"
                        },
                        new
                        {
                            LanguageCode = "en-CA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Canada)"
                        },
                        new
                        {
                            LanguageCode = "en-CB",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Caribbean)"
                        },
                        new
                        {
                            LanguageCode = "en-GB",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (UK)"
                        },
                        new
                        {
                            LanguageCode = "en-IE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Ireland)"
                        },
                        new
                        {
                            LanguageCode = "en-JM",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Jamaica)"
                        },
                        new
                        {
                            LanguageCode = "en-NZ",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (New Zealand)"
                        },
                        new
                        {
                            LanguageCode = "en-PH",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Philippines)"
                        },
                        new
                        {
                            LanguageCode = "en-TT",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Trinidad & Tobago)"
                        },
                        new
                        {
                            LanguageCode = "en-US",
                            Active = true,
                            AvailableLocale = true,
                            Default = true,
                            Name = "English"
                        },
                        new
                        {
                            LanguageCode = "en-ZA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (South Africa)"
                        },
                        new
                        {
                            LanguageCode = "en-ZW",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "English (Zimbabwe)"
                        },
                        new
                        {
                            LanguageCode = "es-AR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Argentina)"
                        },
                        new
                        {
                            LanguageCode = "es-BO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Bolivia)"
                        },
                        new
                        {
                            LanguageCode = "es-CL",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Chile)"
                        },
                        new
                        {
                            LanguageCode = "es-CO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Columbia)"
                        },
                        new
                        {
                            LanguageCode = "es-CR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Costa Rica)"
                        },
                        new
                        {
                            LanguageCode = "es-DO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Dominican Republic)"
                        },
                        new
                        {
                            LanguageCode = "es-EC",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Ecuador)"
                        },
                        new
                        {
                            LanguageCode = "es-ES",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish"
                        },
                        new
                        {
                            LanguageCode = "es-GT",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Guatemala)"
                        },
                        new
                        {
                            LanguageCode = "es-HN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Hondorus)"
                        },
                        new
                        {
                            LanguageCode = "es-MX",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Mexico)"
                        },
                        new
                        {
                            LanguageCode = "es-NI",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Nicaragua)"
                        },
                        new
                        {
                            LanguageCode = "es-PA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Panama)"
                        },
                        new
                        {
                            LanguageCode = "es-PE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Peru)"
                        },
                        new
                        {
                            LanguageCode = "es-PR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Puerto Rico)"
                        },
                        new
                        {
                            LanguageCode = "es-PY",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Paraguay)"
                        },
                        new
                        {
                            LanguageCode = "es-SV",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (El Salvador)"
                        },
                        new
                        {
                            LanguageCode = "es-UY",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Uruguay)"
                        },
                        new
                        {
                            LanguageCode = "es-VE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Spanish (Venezuela)"
                        },
                        new
                        {
                            LanguageCode = "et-EE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Estonian"
                        },
                        new
                        {
                            LanguageCode = "eu-ES",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Basque"
                        },
                        new
                        {
                            LanguageCode = "fa-IR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Farsi (Iran)"
                        },
                        new
                        {
                            LanguageCode = "fi-FI",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Finish (Finland)"
                        },
                        new
                        {
                            LanguageCode = "fo-FO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Faroese"
                        },
                        new
                        {
                            LanguageCode = "fr-BE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "French (Belgium)"
                        },
                        new
                        {
                            LanguageCode = "fr-CA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "French (Canada)"
                        },
                        new
                        {
                            LanguageCode = "fr-CH",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "French (Switzerland)"
                        },
                        new
                        {
                            LanguageCode = "fr-FR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "French"
                        },
                        new
                        {
                            LanguageCode = "fr-LU",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "French (Luxembourg)"
                        },
                        new
                        {
                            LanguageCode = "fr-MC",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "French (Manaco)"
                        },
                        new
                        {
                            LanguageCode = "gl-ES",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Galician"
                        },
                        new
                        {
                            LanguageCode = "gu-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Gujarati (India)"
                        },
                        new
                        {
                            LanguageCode = "he-IL",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Hebrew (Israel)"
                        },
                        new
                        {
                            LanguageCode = "hi-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Hindi (India)"
                        },
                        new
                        {
                            LanguageCode = "hr-HR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Croatian"
                        },
                        new
                        {
                            LanguageCode = "hu-HU",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Hungarian"
                        },
                        new
                        {
                            LanguageCode = "hy-AM",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Armenian (Armenia)"
                        },
                        new
                        {
                            LanguageCode = "id-ID",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Indonesian"
                        },
                        new
                        {
                            LanguageCode = "is-IS",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Icelandic"
                        },
                        new
                        {
                            LanguageCode = "it-CH",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Italian (Switzerland)"
                        },
                        new
                        {
                            LanguageCode = "it-IT",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Italian"
                        },
                        new
                        {
                            LanguageCode = "ja-JP",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Japanese"
                        },
                        new
                        {
                            LanguageCode = "ka-GE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Georgian"
                        },
                        new
                        {
                            LanguageCode = "kk-KZ",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Kazakh"
                        },
                        new
                        {
                            LanguageCode = "kn-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Kannada - India"
                        },
                        new
                        {
                            LanguageCode = "ko-KR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Korean (Korea)"
                        },
                        new
                        {
                            LanguageCode = "ky-KZ",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Kyrgyz"
                        },
                        new
                        {
                            LanguageCode = "lt-LT",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Lithuanian"
                        },
                        new
                        {
                            LanguageCode = "lv-LV",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Latvian"
                        },
                        new
                        {
                            LanguageCode = "mk-MK",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Macedonian"
                        },
                        new
                        {
                            LanguageCode = "mn-MN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Mongolian"
                        },
                        new
                        {
                            LanguageCode = "mr-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Marathi (India)"
                        },
                        new
                        {
                            LanguageCode = "ms-BN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Malay (Brunei)"
                        },
                        new
                        {
                            LanguageCode = "ms-MY",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Malay (Malaysia)"
                        },
                        new
                        {
                            LanguageCode = "nb-NO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Norwegian (Bokmal)"
                        },
                        new
                        {
                            LanguageCode = "nl-BE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Dutch (Belgium)"
                        },
                        new
                        {
                            LanguageCode = "nl-NL",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Dutch (Standard)"
                        },
                        new
                        {
                            LanguageCode = "nn-NO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Norwegian (Nynorsk)"
                        },
                        new
                        {
                            LanguageCode = "pa-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Punjabi (India)"
                        },
                        new
                        {
                            LanguageCode = "pl-PL",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Polish"
                        },
                        new
                        {
                            LanguageCode = "pt-BR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Portuguese (Brazil)"
                        },
                        new
                        {
                            LanguageCode = "pt-PT",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Portuguese (Portugal)"
                        },
                        new
                        {
                            LanguageCode = "ro-RO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Romanian"
                        },
                        new
                        {
                            LanguageCode = "ru-RU",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Russian"
                        },
                        new
                        {
                            LanguageCode = "sa-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Sanskrit (India)"
                        },
                        new
                        {
                            LanguageCode = "sk-SK",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Slovak"
                        },
                        new
                        {
                            LanguageCode = "sl-SI",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Slovenian"
                        },
                        new
                        {
                            LanguageCode = "sq-AL",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Albania"
                        },
                        new
                        {
                            LanguageCode = "sv-FI",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Swedish (Finland)"
                        },
                        new
                        {
                            LanguageCode = "sv-SE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Swedish (Sweden)"
                        },
                        new
                        {
                            LanguageCode = "sw-KE",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Swahili (Kenya)"
                        },
                        new
                        {
                            LanguageCode = "sy-SY",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Syriac"
                        },
                        new
                        {
                            LanguageCode = "ta-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Tamil (India)"
                        },
                        new
                        {
                            LanguageCode = "te-IN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Telugu (India)"
                        },
                        new
                        {
                            LanguageCode = "th-TH",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Thai (Thailand)"
                        },
                        new
                        {
                            LanguageCode = "tr-TR",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Turkish"
                        },
                        new
                        {
                            LanguageCode = "tt-RU",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Tatar (Russia)"
                        },
                        new
                        {
                            LanguageCode = "tx-TX",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Texas"
                        },
                        new
                        {
                            LanguageCode = "uk-UA",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Ukrainian"
                        },
                        new
                        {
                            LanguageCode = "ur-PK",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Urdu (Pakistan)"
                        },
                        new
                        {
                            LanguageCode = "vi-VN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Vietnamese"
                        },
                        new
                        {
                            LanguageCode = "zh-CN",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Chinese (PRC)"
                        },
                        new
                        {
                            LanguageCode = "zh-HK",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Chinese (Hon Kong SAR)"
                        },
                        new
                        {
                            LanguageCode = "zh-MO",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Chinese (Macau SAR)"
                        },
                        new
                        {
                            LanguageCode = "zh-SG",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Chinese (Singapore)"
                        },
                        new
                        {
                            LanguageCode = "zh-TW",
                            Active = true,
                            AvailableLocale = false,
                            Default = false,
                            Name = "Chinese (Taiwan)"
                        });
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Organization", b =>
                {
                    b.Property<Guid>("OrganizationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Address1")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Address2")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<Guid>("OwnerUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Region")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("OrganizationId");

                    b.HasIndex("OwnerUserId");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.OrganizationAllowedDomain", b =>
                {
                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Domain")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.HasKey("OrganizationId", "Domain");

                    b.ToTable("OrganizationAllowedDomains");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RoleId")
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("RoleType")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<Guid?>("UpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.HasIndex("UpdatedByUserId");

                    b.ToTable("IdentityRoles");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Template", b =>
                {
                    b.Property<Guid>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("LanguageCode")
                        .HasColumnType("char(5)")
                        .HasMaxLength(5);

                    b.Property<Guid?>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TemplateKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("TemplateType")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<Guid?>("UpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TemplateId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("TemplateKey")
                        .IsUnique();

                    b.HasIndex("UpdatedByUserId");

                    b.ToTable("Templates");

                    b.HasData(
                        new
                        {
                            TemplateId = new Guid("41e1d669-53b1-431d-ba1d-7bafca4a9b50"),
                            Content = @"Forgot Your Password?
      
It happens. Click the link below to reset your password.

$LINK$

If you did not request a password reset, please contact us at $SUPPORTEMAIL$.",
                            CreatedDate = new DateTime(2021, 2, 17, 5, 2, 53, 456, DateTimeKind.Utc).AddTicks(1902),
                            LanguageCode = LocaleExtensions.DefaultLanguageCode,
                            TemplateKey = "reset_password_en-US_txt",
                            TemplateType = "Message"
                        },
                        new
                        {
                            TemplateId = new Guid("9a1cb149-67dd-4b7f-bf91-d3566d9a17bf"),
                            Content = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
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
                </html>",
                            CreatedDate = new DateTime(2021, 2, 17, 5, 2, 53, 457, DateTimeKind.Utc).AddTicks(934),
                            LanguageCode = LocaleExtensions.DefaultLanguageCode,
                            TemplateKey = "reset_password_en-US_htm",
                            TemplateType = "Message"
                        },
                        new
                        {
                            TemplateId = new Guid("9a151f65-7673-4e88-9f77-f2104a76de64"),
                            Content = @"You're almost there! Please click the link below to verify your e-mail address and activate your account.

                Please click the link below to verify your e-mail address and activate your account.

                $LINK$

                If you did not request an account from us, please contact us at $SUPPORTEMAIL$.",
                            CreatedDate = new DateTime(2021, 2, 17, 5, 2, 53, 457, DateTimeKind.Utc).AddTicks(1912),
                            LanguageCode = LocaleExtensions.DefaultLanguageCode,
                            TemplateKey = "verify_account_en-US_txt",
                            TemplateType = "Message"
                        },
                        new
                        {
                            TemplateId = new Guid("34c00483-ae50-465b-b8d1-4fb9c82dfd8e"),
                            Content = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
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
                </html>",
                            CreatedDate = new DateTime(2021, 2, 17, 5, 2, 53, 457, DateTimeKind.Utc).AddTicks(2614),
                            LanguageCode = LocaleExtensions.DefaultLanguageCode,
                            TemplateKey = "verify_account_en-US_htm",
                            TemplateType = "Message"
                        });
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.TenantConfig", b =>
                {
                    b.Property<Guid>("TenantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<Guid?>("CreatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("nvarchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TenantKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<Guid?>("UpdatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TenantId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("UpdatedByUserId");

                    b.ToTable("TenantConfigs");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.TimeZone", b =>
                {
                    b.Property<string>("TimeZoneId")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<bool>("Default")
                        .HasColumnType("bit");

                    b.Property<string>("LongName")
                        .HasColumnType("nvarchar(300)")
                        .HasMaxLength(300);

                    b.Property<double>("Offset")
                        .HasColumnType("float");

                    b.Property<string>("ShortName")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.HasKey("TimeZoneId");

                    b.HasIndex("ShortName")
                        .IsUnique()
                        .HasFilter("[ShortName] IS NOT NULL");

                    b.ToTable("TimeZones");

                    b.HasData(
                        new
                        {
                            TimeZoneId = "Etc/GMT+12",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-12:00) International Date Line West",
                            Offset = -12.0,
                            ShortName = "Dateline Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Etc/GMT+11",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-11:00) Coordinated Universal Time-11",
                            Offset = -11.0,
                            ShortName = "UTC-11"
                        },
                        new
                        {
                            TimeZoneId = "America/Adak",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-10:00) Aleutian Islands",
                            Offset = -10.0,
                            ShortName = "Aleutian Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Honolulu",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-10:00) Hawaii",
                            Offset = -10.0,
                            ShortName = "Hawaiian Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Marquesas",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-09:30) Marquesas Islands",
                            Offset = -9.5,
                            ShortName = "Marquesas Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Anchorage",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-09:00) Alaska",
                            Offset = -9.0,
                            ShortName = "Alaskan Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Etc/GMT+9",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-09:00) Coordinated Universal Time-09",
                            Offset = -9.0,
                            ShortName = "UTC-09"
                        },
                        new
                        {
                            TimeZoneId = "America/Tijuana",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-08:00) Baja California",
                            Offset = -8.0,
                            ShortName = "Pacific Standard Time (Mexico)"
                        },
                        new
                        {
                            TimeZoneId = "Etc/GMT+8",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-08:00) Coordinated Universal Time-08",
                            Offset = -8.0,
                            ShortName = "UTC-08"
                        },
                        new
                        {
                            TimeZoneId = "America/Los_Angeles",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-08:00) Pacific Time (US & Canada)",
                            Offset = -8.0,
                            ShortName = "Pacific Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Phoenix",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-07:00) Arizona",
                            Offset = -7.0,
                            ShortName = "US Mountain Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Chihuahua",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-07:00) Chihuahua, La Paz, Mazatlan",
                            Offset = -7.0,
                            ShortName = "Mountain Standard Time (Mexico)"
                        },
                        new
                        {
                            TimeZoneId = "America/Denver",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-07:00) Mountain Time (US & Canada)",
                            Offset = -7.0,
                            ShortName = "Mountain Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Guatemala",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-06:00) Central America",
                            Offset = -6.0,
                            ShortName = "Central America Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Chicago",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-06:00) Central Time (US & Canada)",
                            Offset = -6.0,
                            ShortName = "Central Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Easter",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-06:00) Easter Island",
                            Offset = -6.0,
                            ShortName = "Easter Island Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Mexico_City",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-06:00) Guadalajara, Mexico City, Monterrey",
                            Offset = -6.0,
                            ShortName = "Central Standard Time (Mexico)"
                        },
                        new
                        {
                            TimeZoneId = "America/Regina",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-06:00) Saskatchewan",
                            Offset = -6.0,
                            ShortName = "Canada Central Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Bogota",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-05:00) Bogota, Lima, Quito, Rio Branco",
                            Offset = -5.0,
                            ShortName = "SA Pacific Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Cancun",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-05:00) Chetumal",
                            Offset = -5.0,
                            ShortName = "Eastern Standard Time (Mexico)"
                        },
                        new
                        {
                            TimeZoneId = "America/New_York",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-05:00) Eastern Time (US & Canada)",
                            Offset = -5.0,
                            ShortName = "Eastern Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Port-au-Prince",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-05:00) Haiti",
                            Offset = -5.0,
                            ShortName = "Haiti Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Havana",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-05:00) Havana",
                            Offset = -5.0,
                            ShortName = "Cuba Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Indiana/Indianapolis",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-05:00) Indiana (East)",
                            Offset = -5.0,
                            ShortName = "US Eastern Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Asuncion",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-04:00) Asuncion",
                            Offset = -4.0,
                            ShortName = "Paraguay Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Halifax",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-04:00) Atlantic Time (Canada)",
                            Offset = -4.0,
                            ShortName = "Atlantic Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Caracas",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-04:00) Caracas",
                            Offset = -4.0,
                            ShortName = "Venezuela Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Cuiaba",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-04:00) Cuiaba",
                            Offset = -4.0,
                            ShortName = "Central Brazilian Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/La_Paz",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan",
                            Offset = -4.0,
                            ShortName = "SA Western Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Santiago",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-04:00) Santiago",
                            Offset = -4.0,
                            ShortName = "Pacific SA Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Grand_Turk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-04:00) Turks and Caicos",
                            Offset = -4.0,
                            ShortName = "Turks And Caicos Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/St_Johns",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:30) Newfoundland",
                            Offset = -3.5,
                            ShortName = "Newfoundland Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Araguaina",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) Araguaina",
                            Offset = -3.0,
                            ShortName = "Tocantins Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Sao_Paulo",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) Brasilia",
                            Offset = -3.0,
                            ShortName = "E. South America Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Cayenne",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) Cayenne, Fortaleza",
                            Offset = -3.0,
                            ShortName = "SA Eastern Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Argentina/Buenos_Aires",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) City of Buenos Aires",
                            Offset = -3.0,
                            ShortName = "Argentina Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Godthab",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) Greenland",
                            Offset = -3.0,
                            ShortName = "Greenland Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Montevideo",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) Montevideo",
                            Offset = -3.0,
                            ShortName = "Montevideo Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Miquelon",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) Saint Pierre and Miquelon",
                            Offset = -3.0,
                            ShortName = "Saint Pierre Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "America/Bahia",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-03:00) Salvador",
                            Offset = -3.0,
                            ShortName = "Bahia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Etc/GMT+2",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-02:00) Coordinated Universal Time-02",
                            Offset = -2.0,
                            ShortName = "UTC-02"
                        },
                        new
                        {
                            TimeZoneId = "Atlantic/Azores",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-01:00) Azores",
                            Offset = -1.0,
                            ShortName = "Azores Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Atlantic/Cape_Verde",
                            Active = true,
                            Default = false,
                            LongName = "(UTC-01:00) Cabo Verde Is.",
                            Offset = -1.0,
                            ShortName = "Cape Verde Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Etc/UTC",
                            Active = true,
                            Default = true,
                            LongName = "(UTC) Coordinated Universal Time",
                            Offset = 0.0,
                            ShortName = "UTC"
                        },
                        new
                        {
                            TimeZoneId = "Africa/Casablanca",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+00:00) Casablanca",
                            Offset = 0.0,
                            ShortName = "Morocco Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/London",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+00:00) Dublin, Edinburgh, Lisbon, London",
                            Offset = 0.0,
                            ShortName = "GMT Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Atlantic/Reykjavik",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+00:00) Monrovia, Reykjavik",
                            Offset = 0.0,
                            ShortName = "Greenwich Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Berlin",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna",
                            Offset = 1.0,
                            ShortName = "W. Europe Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Budapest",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague",
                            Offset = 1.0,
                            ShortName = "Central Europe Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Paris",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris",
                            Offset = 1.0,
                            ShortName = "Romance Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Warsaw",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb",
                            Offset = 1.0,
                            ShortName = "Central European Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Africa/Lagos",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+01:00) West Central Africa",
                            Offset = 1.0,
                            ShortName = "W. Central Africa Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Africa/Windhoek",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+01:00) Windhoek",
                            Offset = 1.0,
                            ShortName = "Namibia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Amman",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Amman",
                            Offset = 2.0,
                            ShortName = "Jordan Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Bucharest",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Athens, Bucharest",
                            Offset = 2.0,
                            ShortName = "GTB Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Beirut",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Beirut",
                            Offset = 2.0,
                            ShortName = "Middle East Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Africa/Cairo",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Cairo",
                            Offset = 2.0,
                            ShortName = "Egypt Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Chisinau",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Chisinau",
                            Offset = 2.0,
                            ShortName = "E. Europe Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Damascus",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Damascus",
                            Offset = 2.0,
                            ShortName = "Syria Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Hebron",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Gaza, Hebron",
                            Offset = 2.0,
                            ShortName = "West Bank Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Africa/Johannesburg",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Harare, Pretoria",
                            Offset = 2.0,
                            ShortName = "South Africa Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Kiev",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius",
                            Offset = 2.0,
                            ShortName = "FLE Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Istanbul",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Istanbul",
                            Offset = 2.0,
                            ShortName = "Turkey Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Jerusalem",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Jerusalem",
                            Offset = 2.0,
                            ShortName = "Israel Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Kaliningrad",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Kaliningrad",
                            Offset = 2.0,
                            ShortName = "Kaliningrad Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Africa/Tripoli",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+02:00) Tripoli",
                            Offset = 2.0,
                            ShortName = "Libya Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Baghdad",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+03:00) Baghdad",
                            Offset = 3.0,
                            ShortName = "Arabic Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Riyadh",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+03:00) Kuwait, Riyadh",
                            Offset = 3.0,
                            ShortName = "Arab Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Minsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+03:00) Minsk",
                            Offset = 3.0,
                            ShortName = "Belarus Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Moscow",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+03:00) Moscow, St. Petersburg, Volgograd",
                            Offset = 3.0,
                            ShortName = "Russian Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Africa/Nairobi",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+03:00) Nairobi",
                            Offset = 3.0,
                            ShortName = "E. Africa Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Tehran",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+03:30) Tehran",
                            Offset = 3.5,
                            ShortName = "Iran Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Dubai",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:00) Abu Dhabi, Muscat",
                            Offset = 4.0,
                            ShortName = "Arabian Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Astrakhan",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:00) Astrakhan, Ulyanovsk",
                            Offset = 4.0,
                            ShortName = "Astrakhan Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Baku",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:00) Baku",
                            Offset = 4.0,
                            ShortName = "Azerbaijan Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Europe/Samara",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:00) Izhevsk, Samara",
                            Offset = 4.0,
                            ShortName = "Russia Time Zone 3"
                        },
                        new
                        {
                            TimeZoneId = "Indian/Mauritius",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:00) Port Louis",
                            Offset = 4.0,
                            ShortName = "Mauritius Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Tbilisi",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:00) Tbilisi",
                            Offset = 4.0,
                            ShortName = "Georgian Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Yerevan",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:00) Yerevan",
                            Offset = 4.0,
                            ShortName = "Caucasus Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Kabul",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+04:30) Kabul",
                            Offset = 4.5,
                            ShortName = "Afghanistan Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Tashkent",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+05:00) Ashgabat, Tashkent",
                            Offset = 5.0,
                            ShortName = "West Asia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Yekaterinburg",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+05:00) Ekaterinburg",
                            Offset = 5.0,
                            ShortName = "Ekaterinburg Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Karachi",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+05:00) Islamabad, Karachi",
                            Offset = 5.0,
                            ShortName = "Pakistan Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Kolkata",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi",
                            Offset = 5.5,
                            ShortName = "India Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Colombo",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+05:30) Sri Jayawardenepura",
                            Offset = 5.5,
                            ShortName = "Sri Lanka Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Kathmandu",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+05:45) Kathmandu",
                            Offset = 5.75,
                            ShortName = "Nepal Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Almaty",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+06:00) Astana",
                            Offset = 6.0,
                            ShortName = "Central Asia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Dhaka",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+06:00) Dhaka",
                            Offset = 6.0,
                            ShortName = "Bangladesh Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Omsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+06:00) Omsk",
                            Offset = 6.0,
                            ShortName = "Omsk Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Yangon",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+06:30) Yangon (Rangoon)",
                            Offset = 6.5,
                            ShortName = "Myanmar Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Bangkok",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+07:00) Bangkok, Hanoi, Jakarta",
                            Offset = 7.0,
                            ShortName = "SE Asia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Barnaul",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+07:00) Barnaul, Gorno-Altaysk",
                            Offset = 7.0,
                            ShortName = "Altai Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Hovd",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+07:00) Hovd",
                            Offset = 7.0,
                            ShortName = "W. Mongolia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Krasnoyarsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+07:00) Krasnoyarsk",
                            Offset = 7.0,
                            ShortName = "North Asia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Novosibirsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+07:00) Novosibirsk",
                            Offset = 7.0,
                            ShortName = "N. Central Asia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Tomsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+07:00) Tomsk",
                            Offset = 7.0,
                            ShortName = "Tomsk Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Shanghai",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi",
                            Offset = 8.0,
                            ShortName = "China Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Irkutsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:00) Irkutsk",
                            Offset = 8.0,
                            ShortName = "North Asia East Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Singapore",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:00) Kuala Lumpur, Singapore",
                            Offset = 8.0,
                            ShortName = "Singapore Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Perth",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:00) Perth",
                            Offset = 8.0,
                            ShortName = "W. Australia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Taipei",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:00) Taipei",
                            Offset = 8.0,
                            ShortName = "Taipei Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Ulaanbaatar",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:00) Ulaanbaatar",
                            Offset = 8.0,
                            ShortName = "Ulaanbaatar Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Pyongyang",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:30) Pyongyang",
                            Offset = 8.5,
                            ShortName = "North Korea Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Eucla",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+08:45) Eucla",
                            Offset = 8.75,
                            ShortName = "Aus Central W. Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Chita",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+09:00) Chita",
                            Offset = 9.0,
                            ShortName = "Transbaikal Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Tokyo",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+09:00) Osaka, Sapporo, Tokyo",
                            Offset = 9.0,
                            ShortName = "Tokyo Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Seoul",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+09:00) Seoul",
                            Offset = 9.0,
                            ShortName = "Korea Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Yakutsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+09:00) Yakutsk",
                            Offset = 9.0,
                            ShortName = "Yakutsk Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Adelaide",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+09:30) Adelaide",
                            Offset = 9.5,
                            ShortName = "Cen. Australia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Darwin",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+09:30) Darwin",
                            Offset = 9.5,
                            ShortName = "AUS Central Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Brisbane",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+10:00) Brisbane",
                            Offset = 10.0,
                            ShortName = "E. Australia Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Sydney",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+10:00) Canberra, Melbourne, Sydney",
                            Offset = 10.0,
                            ShortName = "AUS Eastern Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Port_Moresby",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+10:00) Guam, Port Moresby",
                            Offset = 10.0,
                            ShortName = "West Pacific Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Hobart",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+10:00) Hobart",
                            Offset = 10.0,
                            ShortName = "Tasmania Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Vladivostok",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+10:00) Vladivostok",
                            Offset = 10.0,
                            ShortName = "Vladivostok Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Australia/Lord_Howe",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+10:30) Lord Howe Island",
                            Offset = 10.5,
                            ShortName = "Lord Howe Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Bougainville",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+11:00) Bougainville Island",
                            Offset = 11.0,
                            ShortName = "Bougainville Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Srednekolymsk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+11:00) Chokurdakh",
                            Offset = 11.0,
                            ShortName = "Russia Time Zone 10"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Magadan",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+11:00) Magadan",
                            Offset = 11.0,
                            ShortName = "Magadan Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Norfolk",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+11:00) Norfolk Island",
                            Offset = 11.0,
                            ShortName = "Norfolk Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Sakhalin",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+11:00) Sakhalin",
                            Offset = 11.0,
                            ShortName = "Sakhalin Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Guadalcanal",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+11:00) Solomon Is., New Caledonia",
                            Offset = 11.0,
                            ShortName = "Central Pacific Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Auckland",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+12:00) Auckland, Wellington",
                            Offset = 12.0,
                            ShortName = "New Zealand Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Etc/GMT-12",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+12:00) Coordinated Universal Time+12",
                            Offset = 12.0,
                            ShortName = "UTC+12"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Fiji",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+12:00) Fiji",
                            Offset = 12.0,
                            ShortName = "Fiji Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Asia/Kamchatka",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+12:00) Petropavlovsk-Kamchatsky - Old",
                            Offset = 12.0,
                            ShortName = "Kamchatka Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Chatham",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+12:45) Chatham Islands",
                            Offset = 12.75,
                            ShortName = "Chatham Islands Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Tongatapu",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+13:00) Nuku'alofa",
                            Offset = 13.0,
                            ShortName = "Tonga Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Apia",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+13:00) Samoa",
                            Offset = 13.0,
                            ShortName = "Samoa Standard Time"
                        },
                        new
                        {
                            TimeZoneId = "Pacific/Kiritimati",
                            Active = true,
                            Default = false,
                            LongName = "(UTC+14:00) Kiritimati Island",
                            Offset = 14.0,
                            ShortName = "Line Islands Standard Time"
                        });
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("newid()");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PasswordlessEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("IdentityUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RoleClaimId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("IdentityRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserClaimId")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId1")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("IdentityUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("IdentityUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("IdentityUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("IdentityUserTokens");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.AuditLog", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Group", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Bastille.Id.Core.Data.Entities.Organization", "Organization")
                        .WithMany("Groups")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("Bastille.Id.Core.Data.Entities.Group", "ParentGroup")
                        .WithMany()
                        .HasForeignKey("ParentGroupId");

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedByUserId");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.GroupUser", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "User")
                        .WithMany("Groups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Organization", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.OrganizationAllowedDomain", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Role", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedByUserId");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.Template", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Bastille.Id.Core.Data.Entities.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageCode");

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedByUserId");
                });

            modelBuilder.Entity("Bastille.Id.Core.Data.Entities.TenantConfig", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Bastille.Id.Core.Data.Entities.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedByUserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", null)
                        .WithMany("Claims")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bastille.Id.Core.Data.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Bastille.Id.Core.Data.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
