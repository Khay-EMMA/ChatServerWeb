using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatServerWeb.Api.Migrations
{
    public partial class FifthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_ChatMessages_ChatApplicationUsers_UserId",
            //    table: "ChatMessages");


            //migrationBuilder.AddColumn<stri>(
            //    name: "UserId",
            //    table: "ChatMessages",
            //    newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ChatMessages",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "Username",
            //    table: "ChatMessages",
            //    newName: "UserId");

            //migrationBuilder.AlterColumn<string>(
            //    name: "UserId",
            //    table: "ChatMessages",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_ChatMessages_UserId",
            //    table: "ChatMessages",
            //    column: "UserId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_ChatMessages_ChatApplicationUsers_UserId",
            //    table: "ChatMessages",
            //    column: "UserId",
            //    principalTable: "ChatApplicationUsers",
            //    principalColumn: "UserId",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
