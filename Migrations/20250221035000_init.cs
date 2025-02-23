using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using OfficeFoodAPI.Model;

#nullable disable

namespace OfficeFoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "usertype_mstr",
                columns: table => new
                {
                    usertypeid = table.Column<Guid>(type: "uuid", nullable: false),
                    usertype = table.Column<string>(type: "text", nullable: false),
                    permission = table.Column<int>(type: "integer", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    upatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usertype_mstr", x => x.usertypeid);
                });

            migrationBuilder.CreateTable(
                name: "vendor_mstr",
                columns: table => new
                {
                    vendorid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    serviceareas = table.Column<List<string>>(type: "text[]", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    upatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor_mstr", x => x.vendorid);
                });

            migrationBuilder.CreateTable(
                name: "company_mstr",
                columns: table => new
                {
                    companyid = table.Column<Guid>(type: "uuid", nullable: false),
                    companyname = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    subsidyperplate = table.Column<double>(type: "double precision", nullable: false),
                    vendorid = table.Column<Guid>(type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    upatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_mstr", x => x.companyid);
                    table.ForeignKey(
                        name: "FK_company_mstr_vendor_mstr_vendorid",
                        column: x => x.vendorid,
                        principalTable: "vendor_mstr",
                        principalColumn: "vendorid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "menuitem_mstr",
                columns: table => new
                {
                    menuitemid = table.Column<Guid>(type: "uuid", nullable: false),
                    itemname = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    upatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vendorid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menuitem_mstr", x => x.menuitemid);
                    table.ForeignKey(
                        name: "FK_menuitem_mstr_vendor_mstr_vendorid",
                        column: x => x.vendorid,
                        principalTable: "vendor_mstr",
                        principalColumn: "vendorid");
                });

            migrationBuilder.CreateTable(
                name: "user_mstr",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    companyid = table.Column<Guid>(type: "uuid", nullable: false),
                    coordinate = table.Column<Point>(type: "geometry(Point, 4326)", nullable: true),
                    usertypeid = table.Column<Guid>(type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    upatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_mstr", x => x.userid);
                    table.ForeignKey(
                        name: "FK_user_mstr_company_mstr_companyid",
                        column: x => x.companyid,
                        principalTable: "company_mstr",
                        principalColumn: "companyid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_mstr_usertype_mstr_usertypeid",
                        column: x => x.usertypeid,
                        principalTable: "usertype_mstr",
                        principalColumn: "usertypeid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_history_mstr",
                columns: table => new
                {
                    employeeid = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    companyid = table.Column<Guid>(type: "uuid", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    month = table.Column<int>(type: "integer", nullable: false),
                    day1 = table.Column<int>(type: "integer", nullable: false),
                    day1logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day2 = table.Column<int>(type: "integer", nullable: false),
                    day2logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day3 = table.Column<int>(type: "integer", nullable: false),
                    day3logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day4 = table.Column<int>(type: "integer", nullable: false),
                    day4logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day5 = table.Column<int>(type: "integer", nullable: false),
                    day5logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day6 = table.Column<int>(type: "integer", nullable: false),
                    day6logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day7 = table.Column<int>(type: "integer", nullable: false),
                    day7logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day8 = table.Column<int>(type: "integer", nullable: false),
                    day8logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day9 = table.Column<int>(type: "integer", nullable: false),
                    day9logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day10 = table.Column<int>(type: "integer", nullable: false),
                    day10logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day11 = table.Column<int>(type: "integer", nullable: false),
                    day11logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day12 = table.Column<int>(type: "integer", nullable: false),
                    day12logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day13 = table.Column<int>(type: "integer", nullable: false),
                    day13logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day14 = table.Column<int>(type: "integer", nullable: false),
                    day14logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day15 = table.Column<int>(type: "integer", nullable: false),
                    day15logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day16 = table.Column<int>(type: "integer", nullable: false),
                    day16logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day17 = table.Column<int>(type: "integer", nullable: false),
                    day17logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day18 = table.Column<int>(type: "integer", nullable: false),
                    day18logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day19 = table.Column<int>(type: "integer", nullable: false),
                    day19logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day20 = table.Column<int>(type: "integer", nullable: false),
                    day20logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day21 = table.Column<int>(type: "integer", nullable: false),
                    day21logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day22 = table.Column<int>(type: "integer", nullable: false),
                    day22logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day23 = table.Column<int>(type: "integer", nullable: false),
                    day23logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day24 = table.Column<int>(type: "integer", nullable: false),
                    day24logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day25 = table.Column<int>(type: "integer", nullable: false),
                    day25logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day26 = table.Column<int>(type: "integer", nullable: false),
                    day26logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day27 = table.Column<int>(type: "integer", nullable: false),
                    day27logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day28 = table.Column<int>(type: "integer", nullable: false),
                    day28logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day29 = table.Column<int>(type: "integer", nullable: false),
                    day29logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day30 = table.Column<int>(type: "integer", nullable: false),
                    day30logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    day31 = table.Column<int>(type: "integer", nullable: false),
                    day31logs = table.Column<List<Log>>(type: "jsonb", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    upatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_history_mstr", x => x.employeeid);
                    table.ForeignKey(
                        name: "FK_employee_history_mstr_company_mstr_companyid",
                        column: x => x.companyid,
                        principalTable: "company_mstr",
                        principalColumn: "companyid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_employee_history_mstr_user_mstr_userid",
                        column: x => x.userid,
                        principalTable: "user_mstr",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userauth_mstr",
                columns: table => new
                {
                    userauthid = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    passwordHash = table.Column<string>(type: "text", nullable: false),
                    refreshToken = table.Column<string>(type: "text", nullable: true),
                    refreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userauth_mstr", x => x.userauthid);
                    table.ForeignKey(
                        name: "FK_userauth_mstr_user_mstr_userid",
                        column: x => x.userid,
                        principalTable: "user_mstr",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_company_mstr_vendorid",
                table: "company_mstr",
                column: "vendorid");

            migrationBuilder.CreateIndex(
                name: "IX_employee_history_mstr_companyid",
                table: "employee_history_mstr",
                column: "companyid");

            migrationBuilder.CreateIndex(
                name: "IX_employee_history_mstr_userid",
                table: "employee_history_mstr",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_menuitem_mstr_vendorid",
                table: "menuitem_mstr",
                column: "vendorid");

            migrationBuilder.CreateIndex(
                name: "IX_user_mstr_companyid",
                table: "user_mstr",
                column: "companyid");

            migrationBuilder.CreateIndex(
                name: "IX_user_mstr_usertypeid",
                table: "user_mstr",
                column: "usertypeid");

            migrationBuilder.CreateIndex(
                name: "IX_userauth_mstr_userid",
                table: "userauth_mstr",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_history_mstr");

            migrationBuilder.DropTable(
                name: "menuitem_mstr");

            migrationBuilder.DropTable(
                name: "userauth_mstr");

            migrationBuilder.DropTable(
                name: "user_mstr");

            migrationBuilder.DropTable(
                name: "company_mstr");

            migrationBuilder.DropTable(
                name: "usertype_mstr");

            migrationBuilder.DropTable(
                name: "vendor_mstr");
        }
    }
}
