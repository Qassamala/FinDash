using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinDash.Migrations
{
    /// <inheritdoc />
    public partial class AddedJunctionTableUserStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StaticStockDataId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStocks_StaticStockData_StaticStockDataId",
                        column: x => x.StaticStockDataId,
                        principalTable: "StaticStockData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserStocks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserStocks_StaticStockDataId",
                table: "UserStocks",
                column: "StaticStockDataId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStocks_UserId_StaticStockDataId",
                table: "UserStocks",
                columns: new[] { "UserId", "StaticStockDataId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStocks");
        }
    }
}
