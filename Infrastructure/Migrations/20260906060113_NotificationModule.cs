using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the composite index FIRST — since it starts with UserId,
            // it can back the existing FK constraint the moment it's created,
            // which is what lets the old single-column index be dropped next
            // without MySQL complaining the FK has nothing to reference.
            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId_IsDeleted_Status_DateCreated",
                table: "Notification",
                columns: new[] { "UserId", "IsDeleted", "Status", "DateCreated" });

            migrationBuilder.DropIndex(
                name: "IX_Notification_UserId",
                table: "Notification");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("06035c74-acbd-42e2-924d-f63b65e3d542"));

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notification");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Notification",
                type: "varchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<string>(
                name: "ActionUrl",
                table: "Notification",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActorAvatarUrl",
                table: "Notification",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActorName",
                table: "Notification",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActorUserId",
                table: "Notification",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Notification",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRead",
                table: "Notification",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceEntityId",
                table: "Notification",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceEntityType",
                table: "Notification",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    CompanyId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ReviewerId = table.Column<string>(type: "varchar(255)", nullable: false),
                    OverallRating = table.Column<int>(type: "int", nullable: false),
                    WorkLifeBalanceRating = table.Column<int>(type: "int", nullable: false),
                    CompensationRating = table.Column<int>(type: "int", nullable: false),
                    JobSecurityRating = table.Column<int>(type: "int", nullable: false),
                    ManagementRating = table.Column<int>(type: "int", nullable: false),
                    CultureRating = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false),
                    Content = table.Column<string>(type: "longtext", nullable: false),
                    JobTitle = table.Column<string>(type: "longtext", nullable: false),
                    Location = table.Column<string>(type: "longtext", nullable: true),
                    IsCurrentEmployee = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    YearsAtCompany = table.Column<int>(type: "int", nullable: true),
                    HelpfulCount = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyReviews_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyReviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SavedJobSearch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProfessionalProfileId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Keyword = table.Column<string>(type: "longtext", nullable: true),
                    Location = table.Column<string>(type: "longtext", nullable: true),
                    JobCategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    EmploymentType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    WorkPlaceType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ExperienceLevel = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmailNotificationsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LastNotifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedJobSearch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedJobSearch_JobCategory_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SavedJobSearch_ProfessionalProfiles_ProfessionalProfileId",
                        column: x => x.ProfessionalProfileId,
                        principalTable: "ProfessionalProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 6, 6, 1, 11, 646, DateTimeKind.Utc).AddTicks(4968));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("48af85db-eaf3-429f-8e66-f4f6b2ca8f30"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 6, 6, 1, 11, 647, DateTimeKind.Utc).AddTicks(4592), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 6, 6, 1, 11, 569, DateTimeKind.Utc).AddTicks(8701), "AQAAAAIAAYagAAAAEL4y+wYdQBs8L+AdUXKU5yBt30NeCU4AUyXN6gBLjzPuOJiy7fgKr2v2wiZgrcLbAw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ActorUserId",
                table: "Notification",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviews_CompanyId",
                table: "CompanyReviews",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviews_ReviewerId",
                table: "CompanyReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobSearch_JobCategoryId",
                table: "SavedJobSearch",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobSearch_ProfessionalProfileId",
                table: "SavedJobSearch",
                column: "ProfessionalProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_ActorUserId",
                table: "Notification",
                column: "ActorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_ActorUserId",
                table: "Notification");

            migrationBuilder.DropTable(
                name: "CompanyReviews");

            migrationBuilder.DropTable(
                name: "SavedJobSearch");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ActorUserId",
                table: "Notification");

            // Recreate the simple UserId index FIRST, so the FK on UserId has
            // something to fall back to before the composite index (which is
            // currently backing that same FK) gets dropped below.
            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.DropIndex(
                name: "IX_Notification_UserId_IsDeleted_Status_DateCreated",
                table: "Notification");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("48af85db-eaf3-429f-8e66-f4f6b2ca8f30"));

            migrationBuilder.DropColumn(
                name: "ActionUrl",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ActorAvatarUrl",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ActorName",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ActorUserId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "DateRead",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "SourceEntityId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "SourceEntityType",
                table: "Notification");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Notification",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Notification",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notification",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Conversation",
                keyColumn: "Id",
                keyValue: new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"),
                column: "DateCreated",
                value: new DateTime(2026, 9, 2, 7, 39, 38, 617, DateTimeKind.Utc).AddTicks(1459));

            migrationBuilder.InsertData(
                table: "ConversationParticipant",
                columns: new[] { "Id", "ConversationId", "CreatedBy", "IsDeleted", "IsHidden", "IsMuted", "IsPinned", "JoinedAt", "LastReadAt", "UserId" },
                values: new object[] { new Guid("06035c74-acbd-42e2-924d-f63b65e3d542"), new Guid("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2"), "c117635d-96e0-409b-9fae-72976ec9c42a", false, false, false, false, new DateTime(2026, 9, 2, 7, 39, 38, 618, DateTimeKind.Utc).AddTicks(2443), null, "c117635d-96e0-409b-9fae-72976ec9c42a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "c117635d-96e0-409b-9fae-72976ec9c42a",
                columns: new[] { "DateCreated", "HashedPassword" },
                values: new object[] { new DateTime(2026, 9, 2, 7, 39, 38, 537, DateTimeKind.Utc).AddTicks(8168), "AQAAAAIAAYagAAAAECDk60CS+cyYwPNZ1YLGVk2l59BddGXBa7XOt7E0IfhHcqlj4xukN/3lZdEuIXQjMA==" });
        }
    }
}