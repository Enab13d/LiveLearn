using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveLearn.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CourseRevisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_section_tasks_CourseId",
                table: "section_tasks");

            migrationBuilder.DropIndex(
                name: "IX_section_tasks_TaskId",
                table: "section_tasks");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "task_replicas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ArchivedRevision",
                table: "sections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedRevision",
                table: "sections",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ArchivedRevision",
                table: "section_tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedRevision",
                table: "section_tasks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ArchivedRevision",
                table: "lectures",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedRevision",
                table: "lectures",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Revision",
                table: "courses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_section_tasks_CourseId_TaskId",
                table: "section_tasks",
                columns: new[] { "CourseId", "TaskId" },
                unique: true,
                filter: "\"ArchivedRevision\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_section_tasks_CourseId_TaskId",
                table: "section_tasks");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "task_replicas");

            migrationBuilder.DropColumn(
                name: "ArchivedRevision",
                table: "sections");

            migrationBuilder.DropColumn(
                name: "CreatedRevision",
                table: "sections");

            migrationBuilder.DropColumn(
                name: "ArchivedRevision",
                table: "section_tasks");

            migrationBuilder.DropColumn(
                name: "CreatedRevision",
                table: "section_tasks");

            migrationBuilder.DropColumn(
                name: "ArchivedRevision",
                table: "lectures");

            migrationBuilder.DropColumn(
                name: "CreatedRevision",
                table: "lectures");

            migrationBuilder.DropColumn(
                name: "Revision",
                table: "courses");

            migrationBuilder.CreateIndex(
                name: "IX_section_tasks_CourseId",
                table: "section_tasks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_section_tasks_TaskId",
                table: "section_tasks",
                column: "TaskId",
                unique: true);
        }
    }
}
