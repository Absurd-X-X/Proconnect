using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NetworkingModuleFirstChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("82f0620f-aa87-4b89-a74b-8547f2ec174a"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Experience",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFollow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    FollowerId = table.Column<string>(type: "varchar(255)", nullable: false),
                    FollowingId = table.Column<string>(type: "varchar(255)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFollow_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollow_Users_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_Experience_CompanyId",
                table: "Experience",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollow_FollowerId_FollowingId",
                table: "UserFollow",
                columns: new[] { "FollowerId", "FollowingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFollow_FollowingId",
                table: "UserFollow",
                column: "FollowingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experience_Companies_CompanyId",
                table: "Experience",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experience_Companies_CompanyId",
                table: "Experience");

            migrationBuilder.DropTable(
                name: "UserFollow");

            migrationBuilder.DropIndex(
                name: "IX_Experience_CompanyId",
                table: "Experience");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("d20b0dbc-3329-4813-be4d-eca3f1cc455f"));

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Experience");

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
    }
}
