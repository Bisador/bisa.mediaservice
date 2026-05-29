using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "media_items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MediaLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaLinks_media_items_MediaId",
                        column: x => x.MediaId,
                        principalTable: "media_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaLinks_MediaId",
                table: "MediaLinks",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaLinks_OwnerType_OwnerId",
                table: "MediaLinks",
                columns: new[] { "OwnerType", "OwnerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaLinks");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "media_items");
        }
    }
}
