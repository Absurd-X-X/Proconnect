using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReUpdateModuleSeven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("a0cb7a15-9d72-4159-902f-cd4332f0cf5b"));

            migrationBuilder.AddColumn<string>(
                name: "InterviewLocationOrLink",
                table: "JobApplication",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InterviewScheduledAt",
                table: "JobApplication",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterviewType",
                table: "JobApplication",
                type: "longtext",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 31, 11, 0, 59, 741, DateTimeKind.Utc).AddTicks(2248));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("e5a5939a-347c-4179-babe-8cfc6c99bbe8"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 8, 31, 11, 0, 59, 742, DateTimeKind.Utc).AddTicks(1655), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 31, 11, 0, 59, 664, DateTimeKind.Utc).AddTicks(3577), "AQAAAAIAAYagAAAAEPBGRRTWCMRpabGL+yCK6g9QsDlhNOgf9CydtDF40eUIBycPV1mbAU4DOcHSbcjc3g==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("e5a5939a-347c-4179-babe-8cfc6c99bbe8"));

            migrationBuilder.DropColumn(
                name: "InterviewLocationOrLink",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "InterviewScheduledAt",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "InterviewType",
                table: "JobApplication");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 30, 9, 27, 29, 47, DateTimeKind.Utc).AddTicks(1755));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("a0cb7a15-9d72-4159-902f-cd4332f0cf5b"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 8, 30, 9, 27, 29, 48, DateTimeKind.Utc).AddTicks(1499), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 30, 9, 27, 28, 966, DateTimeKind.Utc).AddTicks(4963), "AQAAAAIAAYagAAAAEAhYg3YCSRrrvvM9OW3mot62ykpE/rLJybl2ketNaREz0zC7RHZRJ8OmpzeHaXkr2A==" });
        }
    }
}
