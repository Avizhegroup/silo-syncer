using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiloSync.Service.Data.Local.Migrations
{
    /// <inheritdoc />
    public partial class InitialSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SentFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GalleryId = table.Column<long>(type: "INTEGER", nullable: false),
                    UsageId = table.Column<string>(type: "TEXT", nullable: true),
                    MediaName = table.Column<string>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RemoteUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastGalleryId = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncStates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SentFiles_GalleryId",
                table: "SentFiles",
                column: "GalleryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SentFiles_UsageId",
                table: "SentFiles",
                column: "UsageId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncStates_Id",
                table: "SyncStates",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SentFiles");

            migrationBuilder.DropTable(
                name: "SyncStates");
        }
    }
}
