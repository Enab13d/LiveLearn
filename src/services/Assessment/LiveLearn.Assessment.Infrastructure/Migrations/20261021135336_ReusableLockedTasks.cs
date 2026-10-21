using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveLearn.Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReusableLockedTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_tasks",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_tasks", x => new { x.CourseId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_course_tasks_tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "published_courses",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_published_courses", x => x.CourseId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_tasks_TaskId",
                table: "course_tasks",
                column: "TaskId");

            // carry the US-AS01 assignment and lock data over before its columns go
            migrationBuilder.Sql("""
                INSERT INTO course_tasks ("CourseId", "TaskId")
                SELECT "CourseId", "Id" FROM tasks WHERE "CourseId" IS NOT NULL
                ON CONFLICT DO NOTHING;

                INSERT INTO published_courses ("CourseId")
                SELECT DISTINCT "CourseId" FROM locked_tasks
                ON CONFLICT DO NOTHING;
                """);

            migrationBuilder.DropPrimaryKey(
                name: "PK_locked_tasks",
                table: "locked_tasks");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "quiz_attempts");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "homework_submissions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ArchivedAt",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_locked_tasks",
                table: "locked_tasks",
                columns: new[] { "TaskId", "CourseId" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_tasks");

            migrationBuilder.DropTable(
                name: "published_courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_locked_tasks",
                table: "locked_tasks");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "quiz_attempts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "homework_submissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_locked_tasks",
                table: "locked_tasks",
                column: "TaskId");
        }
    }
}
