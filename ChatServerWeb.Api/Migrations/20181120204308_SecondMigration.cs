using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerWeb.Api.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatApplications",
                columns: table => new
                {
                    AppId = table.Column<string>(nullable: false),
                    AppName = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatApplications", x => x.AppId);
                });

            migrationBuilder.CreateTable(
                name: "ChatSettings",
                columns: table => new
                {
                    SettingsId = table.Column<string>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSettings", x => x.SettingsId);
                });

            migrationBuilder.CreateTable(
                name: "ChatApplicationUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    AppId = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatApplicationUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_ChatApplicationUsers_ChatApplications_AppId",
                        column: x => x.AppId,
                        principalTable: "ChatApplications",
                        principalColumn: "AppId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    MessageId = table.Column<string>(nullable: false),
                    AppId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    GroupType = table.Column<int>(nullable: false),
                    GroupId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatApplications_AppId",
                        column: x => x.AppId,
                        principalTable: "ChatApplications",
                        principalColumn: "AppId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ChatGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ChatApplicationUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatApplicationUsers_AppId",
                table: "ChatApplicationUsers",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_AppId",
                table: "ChatMessages",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_GroupId",
                table: "ChatMessages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatSettings");

            migrationBuilder.DropTable(
                name: "ChatApplicationUsers");

            migrationBuilder.DropTable(
                name: "ChatApplications");
        }
    }
}
