using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radiocab.Migrations
{
    /// <inheritdoc />
    public partial class approvalstatusworkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Drivers");
        }
    }
}
