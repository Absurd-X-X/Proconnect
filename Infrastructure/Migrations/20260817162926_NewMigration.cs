using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("1bc84b7e-febe-4ce5-a6c6-e90fcabf5bb7"));

            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "ProfessionalProfiles",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ResumeFileSizeBytes",
                table: "ProfessionalProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResumeUploadedAt",
                table: "ProfessionalProfiles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 17, 16, 29, 25, 148, DateTimeKind.Utc).AddTicks(3219));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "JoinedAt", "UserId" },
                values: new object[] { new Guid("82f0620f-aa87-4b89-a74b-8547f2ec174a"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, new DateTime(2026, 8, 17, 16, 29, 25, 149, DateTimeKind.Utc).AddTicks(3348), "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 17, 16, 29, 25, 67, DateTimeKind.Utc).AddTicks(4945), "AQAAAAIAAYagAAAAEBkOrxwU7w3CXHH54Ilsmm3+/6Dt0/64GHrWSP5SDOFcrrBSM43NJCjG8RLs+1IFSw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("82f0620f-aa87-4b89-a74b-8547f2ec174a"));

            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "ResumeFileSizeBytes",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "ResumeUploadedAt",
                table: "ProfessionalProfiles");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 15, 19, 26, 8, 603, DateTimeKind.Utc).AddTicks(1909));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "JoinedAt", "UserId" },
                values: new object[] { new Guid("1bc84b7e-febe-4ce5-a6c6-e90fcabf5bb7"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, new DateTime(2026, 8, 15, 19, 26, 8, 604, DateTimeKind.Utc).AddTicks(1922), "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 15, 19, 26, 8, 525, DateTimeKind.Utc).AddTicks(4778), "AQAAAAIAAYagAAAAEI1C7QcfRLyQjxaEFtgnBozKN/jF1WAFOhbizSKOqnBUIgNQKoVE5ug86M2Wh+jX/g==" });
        }
    }
}
