using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedModuleSeven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("e5a5939a-347c-4179-babe-8cfc6c99bbe8"));

            migrationBuilder.AddColumn<string>(
                name: "ApplicantCurrentJobTitle",
                table: "JobApplication",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantLinkedInUrl",
                table: "JobApplication",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantLocation",
                table: "JobApplication",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantPhone",
                table: "JobApplication",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantYearsOfExperience",
                table: "JobApplication",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobSkill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    JobId = table.Column<Guid>(type: "char(36)", nullable: false),
                    SkillId = table.Column<Guid>(type: "char(36)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSkill_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_JobId_SkillId",
                table: "JobSkill",
                columns: new[] { "JobId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_SkillId",
                table: "JobSkill",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSkill");

            migrationBuilder.DeleteData(
                table: "ConversationParticipant",
                keyColumn: "Id",
                keyValue: new Guid("06035c74-acbd-42e2-924d-f63b65e3d542"));

            migrationBuilder.DropColumn(
                name: "ApplicantCurrentJobTitle",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "ApplicantLinkedInUrl",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "ApplicantLocation",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "ApplicantPhone",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "ApplicantYearsOfExperience",
                table: "JobApplication");

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
    }
}
