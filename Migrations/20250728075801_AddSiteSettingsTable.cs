using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestP.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSettingsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteBanner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingPolicyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpressShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultTaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricingRulesNotes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettingsTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSettingsTable");
        }
    }
}
