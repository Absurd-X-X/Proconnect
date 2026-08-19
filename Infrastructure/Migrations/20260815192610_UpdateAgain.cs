using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("21a6dfbc-5c10-4c45-8c1c-56d5adfd252c"));

            migrationBuilder.AddColumn<string>(
                name: "AvailabilityStatus",
                table: "ProfessionalProfiles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "ProfessionalProfiles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("1bc84b7e-febe-4ce5-a6c6-e90fcabf5bb7"));

            migrationBuilder.DropColumn(
                name: "AvailabilityStatus",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "ProfessionalProfiles");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 13, 5, 47, 49, 656, DateTimeKind.Utc).AddTicks(4945));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "JoinedAt", "UserId" },
                values: new object[] { new Guid("21a6dfbc-5c10-4c45-8c1c-56d5adfd252c"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, new DateTime(2026, 8, 13, 5, 47, 49, 657, DateTimeKind.Utc).AddTicks(4704), "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 13, 5, 47, 49, 575, DateTimeKind.Utc).AddTicks(8459), "AQAAAAIAAYagAAAAEPNEImM+64fT1vZnS2O3YRM05aOKeFuso+Tazydf8qTEq/kI4mtTRnv1FmeDuWthnA==" });
        }
    }
}
