using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepReady.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedStatusFromExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Exercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Exercises",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
