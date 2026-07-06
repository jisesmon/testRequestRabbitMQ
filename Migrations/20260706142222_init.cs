using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testRequestRabbitMQ.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    EntryTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EntryUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Exception = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "TEXT", nullable: true),
                    StatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    OrdsEntryTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OrdsExitTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OrdsDurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    OrdsException = table.Column<string>(type: "TEXT", nullable: true),
                    OrdsExceptionMessage = table.Column<string>(type: "TEXT", nullable: true),
                    OrdsStatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    OrdsUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAuditLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestAuditLogs");
        }
    }
}
