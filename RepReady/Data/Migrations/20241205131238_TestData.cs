using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RepReady.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Strength" },
                    { 2, "Cardio" },
                    { 3, "Flexibility" }
                });

            migrationBuilder.InsertData(
                table: "Workouts",
                columns: new[] { "Id", "CategoryId", "Date", "Description", "Duration", "Name", "OrganizerId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Workout focusing on upper body strength", 60, "Upper Body Strength", "organizer1" },
                    { 2, 2, new DateTime(2024, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Morning cardio workout to get your heart pumping", 45, "Morning Cardio", "organizer2" }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Description", "Finish", "Reps", "Sets", "Start", "Status", "Title", "WorkoutId" },
                values: new object[,]
                {
                    { 1, "A standard push-up exercise for chest and triceps.", new DateTime(2024, 12, 5, 9, 30, 0, 0, DateTimeKind.Unspecified), 10, 3, new DateTime(2024, 12, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), true, "Push-Up", 1 },
                    { 2, "A 5km run for cardio endurance.", new DateTime(2024, 12, 6, 7, 45, 0, 0, DateTimeKind.Unspecified), 1, 1, new DateTime(2024, 12, 6, 7, 0, 0, 0, DateTimeKind.Unspecified), true, "Running", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Workouts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Workouts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
