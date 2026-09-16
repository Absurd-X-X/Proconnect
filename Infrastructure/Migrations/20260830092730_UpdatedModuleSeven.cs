using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModuleSeven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("d11d07d1-d188-4555-af1f-35c75efd9516"));

            migrationBuilder.AddColumn<string>(
                name: "AdditionalAnswers",
                table: "JobApplication",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkAuthorization",
                table: "JobApplication",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledPublishAt",
                table: "Job",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("a0cb7a15-9d72-4159-902f-cd4332f0cf5b"));

            migrationBuilder.DropColumn(
                name: "AdditionalAnswers",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "WorkAuthorization",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "ScheduledPublishAt",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Job");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 27, 7, 10, 15, 524, DateTimeKind.Utc).AddTicks(5275));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("d11d07d1-d188-4555-af1f-35c75efd9516"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 8, 27, 7, 10, 15, 525, DateTimeKind.Utc).AddTicks(5665), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 27, 7, 10, 15, 437, DateTimeKind.Utc).AddTicks(641), "AQAAAAIAAYagAAAAEGK4OZb3ScfdKCGJCje9esRPNXE3WJppQAqcegUmTw5Gkwdj5H/l0w1FUaFWPu+85g==" });
        }
    }
}
