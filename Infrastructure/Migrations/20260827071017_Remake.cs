using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("f5a2280a-fa4f-4326-8ea6-23cfe041c270"));

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "ConversationParticipant",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                table: "ConversationParticipant",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "ConversationParticipant",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("d11d07d1-d188-4555-af1f-35c75efd9516"));

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "ConversationParticipant");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                table: "ConversationParticipant");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "ConversationParticipant");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 26, 6, 51, 46, 861, DateTimeKind.Utc).AddTicks(2165));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("f5a2280a-fa4f-4326-8ea6-23cfe041c270"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, new DateTime(2026, 8, 26, 6, 51, 46, 862, DateTimeKind.Utc).AddTicks(4005), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 26, 6, 51, 46, 781, DateTimeKind.Utc).AddTicks(4191), "AQAAAAIAAYagAAAAEHEb8ziOux8oJQ/C04XrPVLgKNT6kS5WE7eMjLjAQ4+xiE9c5KFQR3OML7A0ldbE/A==" });
        }
    }
}
