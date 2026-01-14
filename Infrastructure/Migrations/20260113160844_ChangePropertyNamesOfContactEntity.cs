using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropertyNamesOfContactEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactUserId",
                table: "Contacts",
                newName: "AddresseeId");

            migrationBuilder.RenameColumn(
                name: "OwnerUserId",
                table: "Contacts",
                newName: "RequesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddresseeId",
                table: "Contacts",
                newName: "ContactUserId");

            migrationBuilder.RenameColumn(
                name: "RequesterId",
                table: "Contacts",
                newName: "OwnerUserId");
        }
    }
}
