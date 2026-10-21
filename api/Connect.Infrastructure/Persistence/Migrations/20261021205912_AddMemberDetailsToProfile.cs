using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connect.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberDetailsToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "age_band",
                table: "profile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "profile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "profile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "region",
                table: "profile",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "ck_profile_age_band",
                table: "profile",
                sql: "\"age_band\" IN ('Adolescent', 'YoungAdult', 'Adult')");

            migrationBuilder.AddCheckConstraint(
                name: "ck_profile_first_name_not_empty",
                table: "profile",
                sql: "length(\"first_name\") > 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_profile_region_not_empty",
                table: "profile",
                sql: "length(\"region\") > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_profile_age_band",
                table: "profile");

            migrationBuilder.DropCheckConstraint(
                name: "ck_profile_first_name_not_empty",
                table: "profile");

            migrationBuilder.DropCheckConstraint(
                name: "ck_profile_region_not_empty",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "age_band",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "bio",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "region",
                table: "profile");
        }
    }
}
