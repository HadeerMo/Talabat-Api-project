using Microsoft.EntityFrameworkCore.Migrations;

namespace Talabat.repository.Data.Migrations
{
    public partial class EditSubtotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupTotal",
                table: "Orders",
                newName: "SubTotal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "Orders",
                newName: "SupTotal");
        }
    }
}
