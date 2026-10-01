using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class GroupsEnhanced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => new { x.Id, x.CourseId });
                    table.ForeignKey(
                        name: "FK_Groups_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupGrupperingUser",
                columns: table => new
                {
                    MembersId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    GroupCourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupGrupperingUser", x => new { x.MembersId, x.GroupId, x.GroupCourseId });
                    table.ForeignKey(
                        name: "FK_GroupGrupperingUser_Groups_GroupId_GroupCourseId",
                        columns: x => new { x.GroupId, x.GroupCourseId },
                        principalTable: "Groups",
                        principalColumns: new[] { "Id", "CourseId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupGrupperingUser_Users_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupGrupperingUser_GroupId_GroupCourseId",
                table: "GroupGrupperingUser",
                columns: new[] { "GroupId", "GroupCourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CourseId",
                table: "Groups",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupGrupperingUser");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
