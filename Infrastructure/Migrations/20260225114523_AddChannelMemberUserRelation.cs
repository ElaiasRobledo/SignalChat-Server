using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelMemberUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMembers_Users_UserId",
                table: "ChannelMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "ChannelMembers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembers_UserId1",
                table: "ChannelMembers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_Users_UserId",
                table: "ChannelMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_Users_UserId1",
                table: "ChannelMembers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMembers_Users_UserId",
                table: "ChannelMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMembers_Users_UserId1",
                table: "ChannelMembers");

            migrationBuilder.DropIndex(
                name: "IX_ChannelMembers_UserId1",
                table: "ChannelMembers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ChannelMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_Users_UserId",
                table: "ChannelMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
