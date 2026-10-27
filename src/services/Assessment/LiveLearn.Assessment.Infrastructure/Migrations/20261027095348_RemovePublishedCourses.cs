using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveLearn.Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePublishedCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "published_courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
