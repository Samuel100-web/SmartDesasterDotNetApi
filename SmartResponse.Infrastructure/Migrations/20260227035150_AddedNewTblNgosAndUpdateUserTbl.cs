using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartResponse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewTblNgosAndUpdateUserTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CNIC",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyContact",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CNIC",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "BloodGroup",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NgoId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ngos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadOfficeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ngos", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f1e2d3c4-b5a6-4f7e-8d9c-0b1a2c3d4e5f"),
                columns: new[] { "Designation", "NgoId" },
                values: new object[] { "", null });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CNIC",
                table: "Users",
                column: "CNIC",
                unique: true,
                filter: "[CNIC] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NgoId",
                table: "Users",
                column: "NgoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Ngos_NgoId",
                table: "Users",
                column: "NgoId",
                principalTable: "Ngos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Ngos_NgoId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Ngos");

            migrationBuilder.DropIndex(
                name: "IX_Users_CNIC",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_NgoId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NgoId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyContact",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CNIC",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BloodGroup",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CNIC",
                table: "Users",
                column: "CNIC",
                unique: true);
        }
    }
}
