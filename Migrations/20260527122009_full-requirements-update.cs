using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radiocab.Migrations
{
    /// <inheritdoc />
    public partial class fullrequirementsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnitType",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MembershipType",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "Listings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DriverCode",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "Drivers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "Advertisements",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnitType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "MembershipType",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "DriverCode",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Advertisements");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
