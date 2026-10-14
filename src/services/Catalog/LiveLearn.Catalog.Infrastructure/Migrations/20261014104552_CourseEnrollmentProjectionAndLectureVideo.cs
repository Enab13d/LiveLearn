using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveLearn.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CourseEnrollmentProjectionAndLectureVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "lectures");

            migrationBuilder.AddColumn<Guid>(
                name: "VideoId",
                table: "lectures",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoStatus",
                table: "lectures",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "course_enrollments",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CourseRevision = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_enrollments", x => new { x.StudentId, x.CourseId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_enrollments");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "lectures");

            migrationBuilder.DropColumn(
                name: "VideoStatus",
                table: "lectures");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "lectures",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");
        }
    }
}
