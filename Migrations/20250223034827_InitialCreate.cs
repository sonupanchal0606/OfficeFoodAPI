using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeFoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_menuitem_mstr_vendor_mstr_vendorid",
                table: "menuitem_mstr");

            migrationBuilder.AlterColumn<Guid>(
                name: "vendorid",
                table: "menuitem_mstr",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_menuitem_mstr_vendor_mstr_vendorid",
                table: "menuitem_mstr",
                column: "vendorid",
                principalTable: "vendor_mstr",
                principalColumn: "vendorid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_menuitem_mstr_vendor_mstr_vendorid",
                table: "menuitem_mstr");

            migrationBuilder.AlterColumn<Guid>(
                name: "vendorid",
                table: "menuitem_mstr",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_menuitem_mstr_vendor_mstr_vendorid",
                table: "menuitem_mstr",
                column: "vendorid",
                principalTable: "vendor_mstr",
                principalColumn: "vendorid");
        }
    }
}
