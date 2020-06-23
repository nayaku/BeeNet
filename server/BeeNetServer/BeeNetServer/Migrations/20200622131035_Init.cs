using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeeNetServer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Color = table.Column<uint>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    EditTime = table.Column<DateTime>(nullable: false),
                    Num = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Index = table.Column<ushort>(nullable: false),
                    Context = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddTime = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now')"),
                    EditTime = table.Column<DateTime>(nullable: false, defaultValueSql: "datetime('now')"),
                    Path = table.Column<string>(nullable: true),
                    Height = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false),
                    MD5 = table.Column<string>(nullable: true),
                    PriHash = table.Column<byte[]>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    WorkspaceName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pictures_Workspaces_WorkspaceName",
                        column: x => x.WorkspaceName,
                        principalTable: "Workspaces",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PictureLabels",
                columns: table => new
                {
                    PictureId = table.Column<uint>(nullable: false),
                    LabelName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictureLabels", x => new { x.PictureId, x.LabelName });
                    table.ForeignKey(
                        name: "FK_PictureLabels_Labels_LabelName",
                        column: x => x.LabelName,
                        principalTable: "Labels",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PictureLabels_Pictures_PictureId",
                        column: x => x.PictureId,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PictureLabels_LabelName",
                table: "PictureLabels",
                column: "LabelName");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_MD5",
                table: "Pictures",
                column: "MD5",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_WorkspaceName",
                table: "Pictures",
                column: "WorkspaceName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PictureLabels");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropTable(
                name: "Workspaces");
        }
    }
}
