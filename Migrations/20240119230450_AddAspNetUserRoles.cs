using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace asp_net_core_web_app_authentication_authorisation.Migrations
{
    /// <inheritdoc />
    public partial class AddAspNetUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "17e00bb7-18ec-4388-8d97-e55e25df387c", null, "client", "client" },
                    { "b8efa999-4141-49c6-a8c0-6fc0c95df4de", null, "manager", "manager" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "17e00bb7-18ec-4388-8d97-e55e25df387c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8efa999-4141-49c6-a8c0-6fc0c95df4de");
        }
    }
}
