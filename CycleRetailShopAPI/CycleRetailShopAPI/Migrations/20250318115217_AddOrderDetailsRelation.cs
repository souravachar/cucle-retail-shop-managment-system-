using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CycleRetailShopAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDetailsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderID1",
                table: "OrderDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderID1",
                table: "OrderDetails",
                column: "OrderID1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderID1",
                table: "OrderDetails",
                column: "OrderID1",
                principalTable: "Orders",
                principalColumn: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderID1",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderID1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderID1",
                table: "OrderDetails");
        }
    }
}
