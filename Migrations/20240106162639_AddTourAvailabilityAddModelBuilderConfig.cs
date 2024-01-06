using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_net_core_web_app_authentication_authorisation.Migrations
{
    /// <inheritdoc />
    public partial class AddTourAvailabilityAddModelBuilderConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TourAvailabilities",
                columns: table => new
                {
                    TourAvailabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvailableTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourAvailabilities", x => x.TourAvailabilityId);
                    table.ForeignKey(
                        name: "FK_TourAvailabilities_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourAvailabilities_TourId",
                table: "TourAvailabilities",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourAvailabilities");
        }
    }
}
