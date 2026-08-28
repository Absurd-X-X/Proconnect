using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("afc1cf0e-7d0b-4ea6-95a5-490fa91453cc"));

            migrationBuilder.DropColumn(
                name: "PostContenetUrl",
                table: "Post");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 24, 0, 38, 33, 949, DateTimeKind.Utc).AddTicks(3212));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "JoinedAt", "UserId" },
                values: new object[] { new Guid("6e9f0211-3a81-4f3d-a390-a4b2d94ca21c"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, new DateTime(2026, 8, 24, 0, 38, 33, 950, DateTimeKind.Utc).AddTicks(3443), "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 24, 0, 38, 33, 864, DateTimeKind.Utc).AddTicks(955), "AQAAAAIAAYagAAAAENnDfOWjVHRkifUIGFY4T70qoNENWotQivtERgwDDPkKiCMD0gaEowdovqbw/vTOGA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("6e9f0211-3a81-4f3d-a390-a4b2d94ca21c"));

            migrationBuilder.AddColumn<string>(
                name: "PostContenetUrl",
                table: "Post",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 23, 18, 46, 41, 535, DateTimeKind.Utc).AddTicks(7474));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "JoinedAt", "UserId" },
                values: new object[] { new Guid("afc1cf0e-7d0b-4ea6-95a5-490fa91453cc"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, new DateTime(2026, 8, 23, 18, 46, 41, 537, DateTimeKind.Utc).AddTicks(2457), "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 23, 18, 46, 41, 455, DateTimeKind.Utc).AddTicks(7779), "AQAAAAIAAYagAAAAEAoJdrDbOpWBkWeccVtA6gz9vSeChXj5bzGuu/ZniuFjxcmqO8WGZSTnWPKXUs1xUw==" });
        }
    }
}
