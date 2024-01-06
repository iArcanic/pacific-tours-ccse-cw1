using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspnetcorewebappauthenticationauthorisation.Migrations
{
    /// <inheritdoc />
    public partial class FixHotelModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Hotels",
                newName: "CheckOutDate");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Hotels",
                newName: "RoomType");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Hotels",
                newName: "CheckInDate");

            migrationBuilder.AlterColumn<Guid>(
                name: "HotelId",
                table: "Hotels",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoomType",
                table: "Hotels",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "CheckOutDate",
                table: "Hotels",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "CheckInDate",
                table: "Hotels",
                newName: "EndDate");

            migrationBuilder.AlterColumn<int>(
                name: "HotelId",
                table: "Hotels",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
