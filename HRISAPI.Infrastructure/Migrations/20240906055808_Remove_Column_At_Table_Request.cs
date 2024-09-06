using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRISAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Column_At_Table_Request : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "request");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "request");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "request");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "request");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "request",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "request",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "request",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "request",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
