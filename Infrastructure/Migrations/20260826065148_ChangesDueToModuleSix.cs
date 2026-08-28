using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangesDueToModuleSix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("6e9f0211-3a81-4f3d-a390-a4b2d94ca21c"));

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "FileUpload",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadAt",
                table: "ConversationParticipant",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Conversation",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<string>(
                name: "GroupPhotoUrl",
                table: "Conversation",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Conversation",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                columns: new[] { "DateCreated", "GroupPhotoUrl", "IsGroup" },
                values: new object[] { new DateTime(2026, 8, 26, 6, 51, 46, 861, DateTimeKind.Utc).AddTicks(2165), null, false });

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

            migrationBuilder.CreateIndex(
                name: "IX_FileUpload_MessageId",
                table: "FileUpload",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUpload_Message_MessageId",
                table: "FileUpload",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUpload_Message_MessageId",
                table: "FileUpload");

            migrationBuilder.DropIndex(
                name: "IX_FileUpload_MessageId",
                table: "FileUpload");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("f5a2280a-fa4f-4326-8ea6-23cfe041c270"));

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "FileUpload");

            migrationBuilder.DropColumn(
                name: "LastReadAt",
                table: "ConversationParticipant");

            migrationBuilder.DropColumn(
                name: "GroupPhotoUrl",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Conversation");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Message",
                type: "longtext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Message",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Conversation",
                type: "longtext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

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
    }
}
