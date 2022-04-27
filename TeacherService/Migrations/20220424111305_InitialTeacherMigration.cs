using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherService.Migrations
{
    public partial class InitialTeacherMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeacherAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherQuizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClipId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherQuizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAssignmentActivityTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    TeacherAssignmentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityStatus = table.Column<int>(type: "int", nullable: false),
                    ReplyComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReplyAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAssignmentActivityTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAssignmentActivityTrackers_TeacherAssignments_TeacherAssignmentId",
                        column: x => x.TeacherAssignmentId,
                        principalTable: "TeacherAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassEnrollees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassEnrollees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassEnrollees_TeacherClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "TeacherClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherClassTeacherAssignment",
                columns: table => new
                {
                    TeacherAssignmentsId = table.Column<int>(type: "int", nullable: false),
                    TeacherClassesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClassTeacherAssignment", x => new { x.TeacherAssignmentsId, x.TeacherClassesId });
                    table.ForeignKey(
                        name: "FK_TeacherClassTeacherAssignment_TeacherAssignments_TeacherAssignmentsId",
                        column: x => x.TeacherAssignmentsId,
                        principalTable: "TeacherAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherClassTeacherAssignment_TeacherClasses_TeacherClassesId",
                        column: x => x.TeacherClassesId,
                        principalTable: "TeacherClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherClassTeacherQuiz",
                columns: table => new
                {
                    TeacherClassesId = table.Column<int>(type: "int", nullable: false),
                    TeacherQuizsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClassTeacherQuiz", x => new { x.TeacherClassesId, x.TeacherQuizsId });
                    table.ForeignKey(
                        name: "FK_TeacherClassTeacherQuiz_TeacherClasses_TeacherClassesId",
                        column: x => x.TeacherClassesId,
                        principalTable: "TeacherClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherClassTeacherQuiz_TeacherQuizzes_TeacherQuizsId",
                        column: x => x.TeacherQuizsId,
                        principalTable: "TeacherQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherQuizActivityTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    TeacherQuizId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherQuizActivityTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherQuizActivityTrackers_TeacherQuizzes_TeacherQuizId",
                        column: x => x.TeacherQuizId,
                        principalTable: "TeacherQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassEnrollees_ClassId",
                table: "ClassEnrollees",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignmentActivityTrackers_TeacherAssignmentId",
                table: "TeacherAssignmentActivityTrackers",
                column: "TeacherAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassTeacherAssignment_TeacherClassesId",
                table: "TeacherClassTeacherAssignment",
                column: "TeacherClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassTeacherQuiz_TeacherQuizsId",
                table: "TeacherClassTeacherQuiz",
                column: "TeacherQuizsId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherQuizActivityTrackers_TeacherQuizId",
                table: "TeacherQuizActivityTrackers",
                column: "TeacherQuizId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassEnrollees");

            migrationBuilder.DropTable(
                name: "TeacherAssignmentActivityTrackers");

            migrationBuilder.DropTable(
                name: "TeacherClassTeacherAssignment");

            migrationBuilder.DropTable(
                name: "TeacherClassTeacherQuiz");

            migrationBuilder.DropTable(
                name: "TeacherQuizActivityTrackers");

            migrationBuilder.DropTable(
                name: "TeacherSubjects");

            migrationBuilder.DropTable(
                name: "TeacherAssignments");

            migrationBuilder.DropTable(
                name: "TeacherClasses");

            migrationBuilder.DropTable(
                name: "TeacherQuizzes");
        }
    }
}
