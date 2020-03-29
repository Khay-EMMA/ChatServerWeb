using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerWeb.Api.Migrations
{
    public partial class ThirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "ChatGroups",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "ChatApplicationUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_AppId",
                table: "ChatGroups",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatApplicationUsers_GroupId",
                table: "ChatApplicationUsers",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatApplicationUsers_ChatGroups_GroupId",
                table: "ChatApplicationUsers",
                column: "GroupId",
                principalTable: "ChatGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatGroups_ChatApplications_AppId",
                table: "ChatGroups",
                column: "AppId",
                principalTable: "ChatApplications",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatApplicationUsers_ChatGroups_GroupId",
                table: "ChatApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatGroups_ChatApplications_AppId",
                table: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_AppId",
                table: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_ChatApplicationUsers_GroupId",
                table: "ChatApplicationUsers");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "ChatGroups");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ChatApplicationUsers");
        }
    }
}
