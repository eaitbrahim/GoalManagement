using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SothemaGoalManagement.API.Migrations
{
    public partial class SheetValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerComment",
                table: "EvaluationFileInstances",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OwnerValidationDateTime",
                table: "EvaluationFileInstances",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ValidatorComment",
                table: "EvaluationFileInstances",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidatorValidationDateTime",
                table: "EvaluationFileInstances",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerComment",
                table: "EvaluationFileInstances");

            migrationBuilder.DropColumn(
                name: "OwnerValidationDateTime",
                table: "EvaluationFileInstances");

            migrationBuilder.DropColumn(
                name: "ValidatorComment",
                table: "EvaluationFileInstances");

            migrationBuilder.DropColumn(
                name: "ValidatorValidationDateTime",
                table: "EvaluationFileInstances");
        }
    }
}
