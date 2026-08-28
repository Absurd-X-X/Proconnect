using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("d20b0dbc-3329-4813-be4d-eca3f1cc455f"));

            migrationBuilder.AddColumn<int>(
                name: "ReactionType",
                table: "PostLike",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalPostId",
                table: "Post",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "FileUpload",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "FileUpload",
                type: "char(36)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Post_OriginalPostId",
                table: "Post",
                column: "OriginalPostId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUpload_PostId",
                table: "FileUpload",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUpload_Post_PostId",
                table: "FileUpload",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_OriginalPostId",
                table: "Post",
                column: "OriginalPostId",
                principalTable: "Post",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUpload_Post_PostId",
                table: "FileUpload");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_OriginalPostId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_OriginalPostId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_FileUpload_PostId",
                table: "FileUpload");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("afc1cf0e-7d0b-4ea6-95a5-490fa91453cc"));

            migrationBuilder.DropColumn(
                name: "ReactionType",
                table: "PostLike");

            migrationBuilder.DropColumn(
                name: "OriginalPostId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "FileUpload");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "FileUpload");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 8, 19, 19, 55, 33, 58, DateTimeKind.Utc).AddTicks(421));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "JoinedAt", "UserId" },
                values: new object[] { new Guid("d20b0dbc-3329-4813-be4d-eca3f1cc455f"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, new DateTime(2026, 8, 19, 19, 55, 33, 59, DateTimeKind.Utc).AddTicks(752), "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 8, 19, 19, 55, 32, 979, DateTimeKind.Utc).AddTicks(3327), "AQAAAAIAAYagAAAAEEHVRxXbWiglFrdXffIbMP5bOTIKqQ3AVhLF5FlpotsodgcoYr7x0udaSKv2P5FMWQ==" });
        }
    }
}
