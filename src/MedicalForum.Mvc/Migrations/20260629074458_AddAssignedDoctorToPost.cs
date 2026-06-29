using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalForum.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedDoctorToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedDoctorId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AssignedDoctorId",
                table: "Posts",
                column: "AssignedDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_AssignedDoctorId",
                table: "Posts",
                column: "AssignedDoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_AssignedDoctorId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AssignedDoctorId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AssignedDoctorId",
                table: "Posts");
        }
    }
}
