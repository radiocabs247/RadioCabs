using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Radiocab.Migrations
{
    /// <inheritdoc />
    public partial class RemovePaymentForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Advertisements_EntityId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Drivers_EntityId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Listings_EntityId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_EntityId",
                table: "Payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payments_EntityId",
                table: "Payments",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Advertisements_EntityId",
                table: "Payments",
                column: "EntityId",
                principalTable: "Advertisements",
                principalColumn: "AdvertiseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Drivers_EntityId",
                table: "Payments",
                column: "EntityId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Listings_EntityId",
                table: "Payments",
                column: "EntityId",
                principalTable: "Listings",
                principalColumn: "ListingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
