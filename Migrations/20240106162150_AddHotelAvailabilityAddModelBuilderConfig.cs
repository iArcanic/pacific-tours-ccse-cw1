using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_net_core_web_app_authentication_authorisation.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelAvailabilityAddModelBuilderConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HotelAvailabilities",
                columns: table => new
                {
                    HotelAvailabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvailableTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelAvailabilities", x => x.HotelAvailabilityId);
                    table.ForeignKey(
                        name: "FK_HotelAvailabilities_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "HotelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotelAvailabilities_HotelId",
                table: "HotelAvailabilities",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelAvailabilities");
        }
    }
}
