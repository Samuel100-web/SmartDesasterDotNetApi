using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartResponse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigraion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BloodGroup", "CNIC", "CreatedAt", "DeletedAt", "Email", "EmergencyContact", "FullName", "IsDeleted", "PasswordHash", "PhoneNumber", "RoleId", "TrustScore", "UpdatedAt" },
                values: new object[] { new Guid("f1e2d3c4-b5a6-4f7e-8d9c-0b1a2c3d4e5f"), "Admin Office, Islamabad", "O+", "3520100000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@smartresponse.com", "03007654321", "System Admin", false, "$2a$11$mC8m6b8m6b8m6b8m6b8m6eu8y3iR2/mU8u8u8u8u8u8u8u8u8u8u8", "03001234567", new Guid("8db2260d-405a-406a-8451-f2f171097223"), 100, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f1e2d3c4-b5a6-4f7e-8d9c-0b1a2c3d4e5f"));
        }
    }
}
