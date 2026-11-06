using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveLearn.Enrollment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnrollmentOrderLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "course_enrollments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderLineId",
                table: "course_enrollments",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "course_enrollments");

            migrationBuilder.DropColumn(
                name: "OrderLineId",
                table: "course_enrollments");
        }
    }
}
