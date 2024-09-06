using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HRISAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_Tables_To_handle_LeaveRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "request",
                columns: table => new
                {
                    id_request = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    processname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    startdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    enddate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ISBN = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Publisher = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("request_pkey", x => x.id_request);
                });

            migrationBuilder.CreateTable(
                name: "workflow",
                columns: table => new
                {
                    id_workflow = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workflowname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("workflow_pkey", x => x.id_workflow);
                });

            migrationBuilder.CreateTable(
                name: "workflow_sequence",
                columns: table => new
                {
                    step_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_workflow = table.Column<int>(type: "integer", nullable: false),
                    steporder = table.Column<int>(type: "integer", nullable: false),
                    stepname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    requiredrole = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("workflow_sequence_pkey", x => x.step_id);
                    table.ForeignKey(
                        name: "workflow_sequence_id_workflow_fkey",
                        column: x => x.id_workflow,
                        principalTable: "workflow",
                        principalColumn: "id_workflow",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "next_step_rule",
                columns: table => new
                {
                    id_rule = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_currentstep = table.Column<int>(type: "integer", nullable: false),
                    id_nextstep = table.Column<int>(type: "integer", nullable: false),
                    conditiontype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    conditionvalue = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("next_step_rule_pkey", x => x.id_rule);
                    table.ForeignKey(
                        name: "next_step_rule_id_currentstep_fkey",
                        column: x => x.id_currentstep,
                        principalTable: "workflow_sequence",
                        principalColumn: "step_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "next_step_rule_id_nextstep_fkey",
                        column: x => x.id_nextstep,
                        principalTable: "workflow_sequence",
                        principalColumn: "step_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "process",
                columns: table => new
                {
                    id_process = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_workflow = table.Column<int>(type: "integer", nullable: false),
                    id_requester = table.Column<string>(type: "text", nullable: false),
                    request_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    id_current_step = table.Column<int>(type: "integer", nullable: false),
                    id_request = table.Column<int>(type: "integer", nullable: false),
                    request_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("process_pkey", x => x.id_process);
                    table.ForeignKey(
                        name: "FK_Process_Request",
                        column: x => x.id_request,
                        principalTable: "request",
                        principalColumn: "id_request",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Process_Requester",
                        column: x => x.id_requester,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Process_Workflow",
                        column: x => x.id_workflow,
                        principalTable: "workflow",
                        principalColumn: "id_workflow",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_process_workflow_sequence_id_current_step",
                        column: x => x.id_current_step,
                        principalTable: "workflow_sequence",
                        principalColumn: "step_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_action",
                columns: table => new
                {
                    id_action = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_proceess = table.Column<int>(type: "integer", nullable: false),
                    id_step = table.Column<int>(type: "integer", nullable: false),
                    id_actor = table.Column<string>(type: "text", nullable: false),
                    action = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    actiondate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    comments = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("workflow_action_pkey", x => x.id_action);
                    table.ForeignKey(
                        name: "FK_WorkflowAction_User",
                        column: x => x.id_actor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workflow_action_workflow_sequence_id_step",
                        column: x => x.id_step,
                        principalTable: "workflow_sequence",
                        principalColumn: "step_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "workflow_action_id_request_fkey",
                        column: x => x.id_proceess,
                        principalTable: "process",
                        principalColumn: "id_process",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_next_step_rule_id_currentstep",
                table: "next_step_rule",
                column: "id_currentstep");

            migrationBuilder.CreateIndex(
                name: "IX_next_step_rule_id_nextstep",
                table: "next_step_rule",
                column: "id_nextstep");

            migrationBuilder.CreateIndex(
                name: "IX_process_id_current_step",
                table: "process",
                column: "id_current_step");

            migrationBuilder.CreateIndex(
                name: "IX_process_id_request",
                table: "process",
                column: "id_request",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_process_id_requester",
                table: "process",
                column: "id_requester");

            migrationBuilder.CreateIndex(
                name: "IX_process_id_workflow",
                table: "process",
                column: "id_workflow");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_action_id_actor",
                table: "workflow_action",
                column: "id_actor");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_action_id_proceess",
                table: "workflow_action",
                column: "id_proceess");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_action_id_step",
                table: "workflow_action",
                column: "id_step");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_sequence_id_workflow",
                table: "workflow_sequence",
                column: "id_workflow");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "next_step_rule");

            migrationBuilder.DropTable(
                name: "workflow_action");

            migrationBuilder.DropTable(
                name: "process");

            migrationBuilder.DropTable(
                name: "request");

            migrationBuilder.DropTable(
                name: "workflow_sequence");

            migrationBuilder.DropTable(
                name: "workflow");
        }
    }
}
