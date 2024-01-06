using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspnetcorewebappauthenticationauthorisation.Migrations
{
    /// <inheritdoc />
    public partial class AddPassportNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "AspNetUsers");
        }
    }
}
