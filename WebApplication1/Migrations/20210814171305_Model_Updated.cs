using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class Model_Updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_AspNetUsers_AppUserId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Vacancies_VacancyId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobVacancies_AspNetUsers_AdminId",
                table: "JobVacancies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_VacancyAudits_VacancyAuditId",
                table: "Vacancies");

            migrationBuilder.DropTable(
                name: "VacancyAudits");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_VacancyAuditId",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "AppUser_HashPassword",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppUser_Key",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "JobVacancies",
                newName: "AppUserId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_AspNetUsers_AppUserId",
                table: "JobApplications",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Vacancies_VacancyId",
                table: "JobApplications",
                column: "VacancyId",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobVacancies_AspNetUsers_AppUserId",
                table: "JobVacancies",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_AspNetUsers_AppUserId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Vacancies_VacancyId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobVacancies_AspNetUsers_AppUserId",
                table: "JobVacancies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "JobVacancies",
                newName: "AdminId");

            migrationBuilder.AddColumn<byte[]>(
                name: "AppUser_HashPassword",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "AppUser_Key",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VacancyAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyAudits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_VacancyAuditId",
                table: "Vacancies",
                column: "VacancyAuditId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_AspNetUsers_AppUserId",
                table: "JobApplications",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Vacancies_VacancyId",
                table: "JobApplications",
                column: "VacancyId",
                principalTable: "Vacancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobVacancies_AspNetUsers_AdminId",
                table: "JobVacancies",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_VacancyAudits_VacancyAuditId",
                table: "Vacancies",
                column: "VacancyAuditId",
                principalTable: "VacancyAudits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
