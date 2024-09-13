using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRISAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_relation_to_Table_LeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_workflow_sequence_requiredrole",
                table: "workflow_sequence",
                column: "requiredrole",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeID",
                table: "LeaveRequests",
                column: "EmployeeID",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_ProcessId",
                table: "LeaveRequests",
                column: "ProcessId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeID",
                table: "LeaveRequests",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_process_ProcessId",
                table: "LeaveRequests",
                column: "ProcessId",
                principalTable: "process",
                principalColumn: "id_process",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workflow_sequence_AspNetRoles_requiredrole",
                table: "workflow_sequence",
                column: "requiredrole",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Employees_EmployeeID",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_process_ProcessId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_workflow_sequence_AspNetRoles_requiredrole",
                table: "workflow_sequence");

            migrationBuilder.DropIndex(
                name: "IX_workflow_sequence_requiredrole",
                table: "workflow_sequence");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_EmployeeID",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_ProcessId",
                table: "LeaveRequests");
        }
    }
}
