using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connect.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceNormalizedAccountUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                newName: "ux_account_username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ux_account_username",
                table: "AspNetUsers",
                newName: "UserNameIndex");
        }
    }
}
