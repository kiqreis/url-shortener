﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "Plan",
                table: "ApplicationUsers");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUsers_NormalizedUserName",
                table: "ApplicationUsers",
                newName: "UserNameIndex");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUsers_NormalizedEmail",
                table: "ApplicationUsers",
                newName: "EmailIndex");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    Plan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShortUrl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UrlStatus = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortUrl_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrlClick",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClickedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Referrer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortUrlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlClick", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrlClick_ShortUrl_ShortUrlId",
                        column: x => x.ShortUrlId,
                        principalTable: "ShortUrl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRoles_RoleId",
                table: "ApplicationUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRoleClaims_RoleId",
                table: "ApplicationUserRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShortUrl_UserId",
                table: "ShortUrl",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UrlClick_ShortUrlId",
                table: "UrlClick",
                column: "ShortUrlId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRoleClaims_AspNetRoles_RoleId",
                table: "ApplicationUserRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserRoles_AspNetRoles_RoleId",
                table: "ApplicationUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRoleClaims_AspNetRoles_RoleId",
                table: "ApplicationUserRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserRoles_AspNetRoles_RoleId",
                table: "ApplicationUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "UrlClick");

            migrationBuilder.DropTable(
                name: "ShortUrl");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserRoles_RoleId",
                table: "ApplicationUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserRoleClaims_RoleId",
                table: "ApplicationUserRoleClaims");

            migrationBuilder.RenameIndex(
                name: "UserNameIndex",
                table: "ApplicationUsers",
                newName: "IX_ApplicationUsers_NormalizedUserName");

            migrationBuilder.RenameIndex(
                name: "EmailIndex",
                table: "ApplicationUsers",
                newName: "IX_ApplicationUsers_NormalizedEmail");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ApplicationUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "ApplicationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Plan",
                table: "ApplicationUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
