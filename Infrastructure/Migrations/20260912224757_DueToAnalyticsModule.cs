using Microsoft.EntityFrameworkCore.Migrations;


namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DueToAnalyticsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("c5e5be59-f9fa-4ace-ad3e-9e0c8616061c"));

            migrationBuilder.CreateTable(
                name: "AnalyticsEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    SubjectType = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ActorUserId = table.Column<string>(type: "varchar(255)", nullable: true),
                    ReferrerSource = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalyticsEvent_Users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 12, 22, 47, 56, 455, DateTimeKind.Utc).AddTicks(1015));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("11ef98cb-df77-423d-806c-89833b448aca"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 12, 22, 47, 56, 456, DateTimeKind.Utc).AddTicks(821), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 12, 22, 47, 56, 375, DateTimeKind.Utc).AddTicks(5408), "AQAAAAIAAYagAAAAENJd4BwyHr4b4MBK4J/dfA+f2h7lOqN85UFfKPvfRikFM3ijY4NxmFZXUvv5Vd2MQg==" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvent_ActorUserId_DateCreated",
                table: "AnalyticsEvent",
                columns: new[] { "ActorUserId", "DateCreated" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvent_SubjectType_SubjectId_DateCreated",
                table: "AnalyticsEvent",
                columns: new[] { "SubjectType", "SubjectId", "DateCreated" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsEvent");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("11ef98cb-df77-423d-806c-89833b448aca"));

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 8, 10, 47, 37, 735, DateTimeKind.Utc).AddTicks(3117));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("c5e5be59-f9fa-4ace-ad3e-9e0c8616061c"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 8, 10, 47, 37, 736, DateTimeKind.Utc).AddTicks(3210), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 8, 10, 47, 37, 635, DateTimeKind.Utc).AddTicks(1061), "AQAAAAIAAYagAAAAELD7zodPPy+OKGhzmCnWKvdtp7RseDuMNic8c6H52JuznGSxEPwLpys7+riFzSpxAQ==" });
        }
    }
}
