using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LessonCount",
                table: "Trainings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "LessonDurationHours",
                table: "Trainings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Trainings_DurationHours_Positive",
                table: "Trainings",
                sql: "\"DurationHours\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Trainings_LessonCount_Positive",
                table: "Trainings",
                sql: "\"LessonCount\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Trainings_LessonDurationHours_Range",
                table: "Trainings",
                sql: "\"LessonDurationHours\" > 0 AND \"LessonDurationHours\" <= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Trainings_LessonSchedule_WithinDuration",
                table: "Trainings",
                sql: "\"LessonCount\" * \"LessonDurationHours\" <= \"DurationHours\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Trainings_DurationHours_Positive",
                table: "Trainings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Trainings_LessonCount_Positive",
                table: "Trainings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Trainings_LessonDurationHours_Range",
                table: "Trainings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Trainings_LessonSchedule_WithinDuration",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "LessonCount",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "LessonDurationHours",
                table: "Trainings");
        }
    }
}
