using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Applicant = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExpectedReturn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActualReturn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceBookings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceBookings");
        }
    }
}
