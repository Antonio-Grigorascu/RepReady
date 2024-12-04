using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepReady.Data.Migrations
{
    /// <inheritdoc />
    public partial class FaraAsociativa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_AspNetUsers_OrganizerId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "ApplicationUserWorkouts");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_OrganizerId",
                table: "Workouts");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizerId",
                table: "Workouts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUserWorkout",
                columns: table => new
                {
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkoutsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserWorkout", x => new { x.UsersId, x.WorkoutsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserWorkout_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserWorkout_Workouts_WorkoutsId",
                        column: x => x.WorkoutsId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserWorkout_WorkoutsId",
                table: "ApplicationUserWorkout",
                column: "WorkoutsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserWorkout");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizerId",
                table: "Workouts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUserWorkouts",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkoutId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserWorkouts", x => new { x.ApplicationUserId, x.WorkoutId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserWorkouts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserWorkouts_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_OrganizerId",
                table: "Workouts",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserWorkouts_WorkoutId",
                table: "ApplicationUserWorkouts",
                column: "WorkoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_AspNetUsers_OrganizerId",
                table: "Workouts",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
