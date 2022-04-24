using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurriculumService.Migrations
{
    public partial class InitialCurriculumMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectLanguage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectLanguage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Term = table.Column<int>(type: "int", nullable: false),
                    IsAppShow = table.Column<bool>(type: "bit", nullable: true),
                    RewardPoints = table.Column<int>(type: "int", nullable: true),
                    TeacherGuide = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullyQualifiedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectLanguageId = table.Column<int>(type: "int", nullable: true),
                    SubjectGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_SubjectGroup_SubjectGroupId",
                        column: x => x.SubjectGroupId,
                        principalTable: "SubjectGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subjects_SubjectLanguage_SubjectLanguageId",
                        column: x => x.SubjectLanguageId,
                        principalTable: "SubjectLanguage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: true),
                    FullyQualifiedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShow = table.Column<bool>(type: "bit", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    UnitNumber = table.Column<int>(type: "int", nullable: true),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubjectId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartUnit = table.Column<int>(type: "int", nullable: true),
                    EndUnit = table.Column<int>(type: "int", nullable: true),
                    IsShow = table.Column<bool>(type: "bit", nullable: true),
                    Ponits = table.Column<int>(type: "int", nullable: true),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sort = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageNo = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KNLDBank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMedu = table.Column<bool>(type: "bit", nullable: true),
                    Usability = table.Column<int>(type: "int", nullable: true),
                    Points = table.Column<int>(type: "int", nullable: true),
                    Orientation = table.Column<int>(type: "int", nullable: true),
                    IsPremium = table.Column<bool>(type: "bit", nullable: true),
                    LessonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clips_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    Creator = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quizzes_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quizzes_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MCQ",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationInSec = table.Column<int>(type: "int", nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    ClipId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCQ", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MCQ_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MCQ_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuizForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DurationInSec = table.Column<int>(type: "int", nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClipId = table.Column<int>(type: "int", nullable: true),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizForms_Clips_ClipId",
                        column: x => x.ClipId,
                        principalTable: "Clips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizForms_QuizQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizForms_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MCQAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    MCQId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCQAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MCQAnswer_MCQ_MCQId",
                        column: x => x.MCQId,
                        principalTable: "MCQ",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MCQQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MCQId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCQQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MCQQuestion_MCQ_MCQId",
                        column: x => x.MCQId,
                        principalTable: "MCQ",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuizFormId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_QuizForms_QuizFormId",
                        column: x => x.QuizFormId,
                        principalTable: "QuizForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizFormId = table.Column<int>(type: "int", nullable: true),
                    UserAnswerId = table.Column<int>(type: "int", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_QuizAnswers_UserAnswerId",
                        column: x => x.UserAnswerId,
                        principalTable: "QuizAnswers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizAttempts_QuizForms_QuizFormId",
                        column: x => x.QuizFormId,
                        principalTable: "QuizForms",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "SubjectGroup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "KG1" },
                    { 2, "KG2" },
                    { 3, "الصف الأول الإبتدائى" },
                    { 4, "الصف الثانى الإبتدائى" },
                    { 5, "الصف الثالث الإبتدائى" },
                    { 6, "الصف الرابع الإبتدائى" },
                    { 7, "الصف الخامس الإبتدائى" },
                    { 8, "الصف السادس الإبتدائى" },
                    { 9, "الصف الأول الإعدادى" },
                    { 10, "الصف الثانى الإعدادى" },
                    { 11, "الصف الثالث الإعدادى" },
                    { 12, "الصف الأول الثانوى" },
                    { 13, "الصف الثانى الثانوى" },
                    { 14, "الثانوية العامة" }
                });

            migrationBuilder.InsertData(
                table: "SubjectLanguage",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "عربى" },
                    { 2, "لغات" },
                    { 3, "الكل" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clips_LessonId",
                table: "Clips",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_UnitId",
                table: "Lessons",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MCQ_ClipId",
                table: "MCQ",
                column: "ClipId");

            migrationBuilder.CreateIndex(
                name: "IX_MCQ_LessonId",
                table: "MCQ",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_MCQAnswer_MCQId",
                table: "MCQAnswer",
                column: "MCQId");

            migrationBuilder.CreateIndex(
                name: "IX_MCQQuestion_MCQId",
                table: "MCQQuestion",
                column: "MCQId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuizFormId",
                table: "QuizAnswers",
                column: "QuizFormId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizFormId",
                table: "QuizAttempts",
                column: "QuizFormId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserAnswerId",
                table: "QuizAttempts",
                column: "UserAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizForms_ClipId",
                table: "QuizForms",
                column: "ClipId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizForms_QuestionId",
                table: "QuizForms",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizForms_QuizId",
                table: "QuizForms",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_LessonId",
                table: "Quizzes",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_SubjectId",
                table: "Quizzes",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UnitId",
                table: "Quizzes",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectGroupId",
                table: "Subjects",
                column: "SubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectLanguageId",
                table: "Subjects",
                column: "SubjectLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_SubjectId",
                table: "Units",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MCQAnswer");

            migrationBuilder.DropTable(
                name: "MCQQuestion");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "MCQ");

            migrationBuilder.DropTable(
                name: "QuizAnswers");

            migrationBuilder.DropTable(
                name: "QuizForms");

            migrationBuilder.DropTable(
                name: "Clips");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectGroup");

            migrationBuilder.DropTable(
                name: "SubjectLanguage");
        }
    }
}
