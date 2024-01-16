using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_net_core_web_app_authentication_authorisation.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPaidPropertyToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "TourBookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "PackageBookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "HotelBookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "TourBookings");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "PackageBookings");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "HotelBookings");
        }
    }
}
