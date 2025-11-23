using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Author_NormalizedFullName_BirthDate",
                table: "Authors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_Author_NormalizedFullName_BirthDate",
                table: "Authors",
                columns: new[] { "NormalizedFullName", "BirthDate" },
                unique: true,
                filter: "[BirthDate] IS NOT NULL");
        }
    }
}
