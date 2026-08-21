using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveLearn.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CourseTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionTask_sections_SectionId",
                table: "SectionTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SectionTask",
                table: "SectionTask");

            migrationBuilder.RenameTable(
                name: "SectionTask",
                newName: "section_tasks");

            migrationBuilder.RenameIndex(
                name: "IX_SectionTask_SectionId",
                table: "section_tasks",
                newName: "IX_section_tasks_SectionId");

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "section_tasks",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "AssignedAt",
                table: "section_tasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_section_tasks",
                table: "section_tasks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_section_tasks_CourseId",
                table: "section_tasks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_section_tasks_TaskId",
                table: "section_tasks",
                column: "TaskId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_section_tasks_courses_CourseId",
                table: "section_tasks",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_section_tasks_sections_SectionId",
                table: "section_tasks",
                column: "SectionId",
                principalTable: "sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_section_tasks_courses_CourseId",
                table: "section_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_section_tasks_sections_SectionId",
                table: "section_tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_section_tasks",
                table: "section_tasks");

            migrationBuilder.DropIndex(
                name: "IX_section_tasks_CourseId",
                table: "section_tasks");

            migrationBuilder.DropIndex(
                name: "IX_section_tasks_TaskId",
                table: "section_tasks");

            migrationBuilder.RenameTable(
                name: "section_tasks",
                newName: "SectionTask");

            migrationBuilder.RenameIndex(
                name: "IX_section_tasks_SectionId",
                table: "SectionTask",
                newName: "IX_SectionTask_SectionId");

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "SectionTask",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "AssignedAt",
                table: "SectionTask",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SectionTask",
                table: "SectionTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionTask_sections_SectionId",
                table: "SectionTask",
                column: "SectionId",
                principalTable: "sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
