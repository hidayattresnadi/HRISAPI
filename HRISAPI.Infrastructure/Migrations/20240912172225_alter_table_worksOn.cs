using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HRISAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alter_table_worksOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorksOns",
                table: "WorksOns");

            migrationBuilder.AlterColumn<int>(
                name: "WorksOnId",
                table: "WorksOns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorksOns",
                table: "WorksOns",
                column: "WorksOnId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksOns_EmpNo",
                table: "WorksOns",
                column: "EmpNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorksOns",
                table: "WorksOns");

            migrationBuilder.DropIndex(
                name: "IX_WorksOns_EmpNo",
                table: "WorksOns");

            migrationBuilder.AlterColumn<int>(
                name: "WorksOnId",
                table: "WorksOns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorksOns",
                table: "WorksOns",
                columns: new[] { "EmpNo", "ProjNo" });
        }
    }
}
