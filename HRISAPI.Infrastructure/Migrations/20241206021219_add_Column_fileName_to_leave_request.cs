using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRISAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_Column_fileName_to_leave_request : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "LeaveRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "LeaveRequests");
        }
    }
}
