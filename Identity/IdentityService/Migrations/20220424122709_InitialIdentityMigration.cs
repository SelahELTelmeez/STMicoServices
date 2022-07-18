using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Migrations
{
    public partial class InitialIdentityMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avatars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    AvatarType = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avatars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Governorates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governorates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentitySchools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentitySchools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "NVARCHAR(36)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GradeId = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    Country = table.Column<int>(type: "int", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HopeToBe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferralCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEmailSubscribed = table.Column<bool>(type: "bit", nullable: true),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: true),
                    IsMobileVerified = table.Column<bool>(type: "bit", nullable: true),
                    IdentityRoleId = table.Column<int>(type: "int", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: true),
                    IdentitySchoolId = table.Column<int>(type: "int", nullable: true),
                    AvatarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUsers_Avatars_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Avatars",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUsers_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUsers_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityUsers_IdentityRoles_IdentityRoleId",
                        column: x => x.IdentityRoleId,
                        principalTable: "IdentityRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUsers_IdentitySchools_IdentitySchoolId",
                        column: x => x.IdentitySchoolId,
                        principalTable: "IdentitySchools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExternalIdentityProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifierkey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalIdentityProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalIdentityProviders_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityActivations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivationType = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityActivations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityActivations_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityReferralTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: false),
                    IdentityReferralUserId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityReferralTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityReferralTrackers_IdentityUsers_IdentityReferralUserId",
                        column: x => x.IdentityReferralUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityReferralTrackers_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRefreshTokens", x => x.Token);
                    table.ForeignKey(
                        name: "FK_IdentityRefreshTokens_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    PrimaryId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: true),
                    SecondaryId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRelations_IdentityUsers_PrimaryId",
                        column: x => x.PrimaryId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdentityRelations_IdentityUsers_SecondaryId",
                        column: x => x.SecondaryId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentitySubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentitySubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentitySubjects_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityTemporaryValueHolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "NVARCHAR(36)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityTemporaryValueHolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityTemporaryValueHolders_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Avatars",
                columns: new[] { "Id", "AvatarType", "ImageUrl" },
                values: new object[,]
                {
                    { 0, 0, "default.png" },
                    { 1, 1, "01.png" },
                    { 2, 1, "02.png" },
                    { 3, 1, "03.png" },
                    { 5, 1, "04.png" },
                    { 6, 1, "05.png" },
                    { 7, 1, "06.png" },
                    { 8, 1, "07.png" },
                    { 9, 1, "08.png" },
                    { 10, 2, "01.png" },
                    { 11, 2, "02.png" },
                    { 12, 2, "03.png" },
                    { 13, 2, "04.png" },
                    { 14, 2, "05.png" },
                    { 15, 2, "06.png" },
                    { 16, 2, "07.png" },
                    { 17, 2, "08.png" },
                    { 18, 3, "01.png" },
                    { 19, 3, "02.png" },
                    { 20, 3, "03.png" },
                    { 21, 3, "04.png" },
                    { 22, 3, "05.png" },
                    { 23, 3, "06.png" },
                    { 24, 3, "07.png" },
                    { 25, 3, "08.png" },
                    { 26, 1, "09.png" },
                    { 27, 1, "10.png" },
                    { 28, 1, "11.png" },
                    { 29, 1, "12.png" },
                    { 30, 1, "13.png" },
                    { 31, 1, "14.png" },
                    { 32, 1, "15.png" },
                    { 33, 1, "16.png" },
                    { 34, 3, "09.png" },
                    { 35, 3, "10.png" },
                    { 36, 3, "11.png" },
                    { 37, 3, "12.png" },
                    { 38, 3, "13.png" },
                    { 39, 3, "14.png" },
                    { 40, 3, "15.png" },
                    { 41, 3, "16.png" }
                });

            migrationBuilder.InsertData(
                table: "Governorates",
                columns: new[] { "Id", "IsEnabled", "Name" },
                values: new object[] { 1, true, "القاهرة" });

            migrationBuilder.InsertData(
                table: "Governorates",
                columns: new[] { "Id", "IsEnabled", "Name" },
                values: new object[,]
                {
                    { 2, true, "الجيزة" },
                    { 3, false, "حلوان" },
                    { 4, true, "الدقهلية" },
                    { 5, true, "المنوفية" },
                    { 6, true, "الاسكندرية" },
                    { 7, true, "الشرقية" },
                    { 8, true, "الغربية" },
                    { 9, true, "القليوبية" },
                    { 10, true, "بورسعيد" },
                    { 11, true, "اسوان" },
                    { 12, false, "6 أكتوبر" },
                    { 13, true, "اسيوط" },
                    { 14, true, "كفر الشيخ" },
                    { 15, true, "السويس" },
                    { 16, true, "بنى سويف" },
                    { 17, true, "الفيوم" },
                    { 18, true, "البحيرة" },
                    { 19, true, "المنيا" },
                    { 20, true, "سوهاج" },
                    { 21, true, "الاسماعيلية" },
                    { 22, true, "شمال سيناء" },
                    { 23, true, "دمياط" },
                    { 24, true, "الاقصر" },
                    { 25, true, "جنوب سيناء" },
                    { 26, true, "البحر الاحمر" },
                    { 27, true, "قنا" },
                    { 28, true, "الوادى الجديد" },
                    { 29, true, "مرسى مطروح" }
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "IsEnabled", "Name" },
                values: new object[,]
                {
                    { 1, false, "KG1" },
                    { 2, false, "KG2" },
                    { 3, true, "الصف الأول الإبتدائى" },
                    { 4, true, "الصف الثانى الإبتدائى" },
                    { 5, true, "الصف الثالث الإبتدائى" },
                    { 6, true, "الصف الرابع الإبتدائى" },
                    { 7, true, "الصف الخامس الإبتدائى" },
                    { 8, true, "الصف السادس الإبتدائى" },
                    { 9, false, "الصف الأول الإعدادى" },
                    { 10, false, "الصف الثانى الإعدادى" },
                    { 11, false, "الصف الثالث الإعدادى" },
                    { 12, false, "الصف الأول الثانوى" },
                    { 13, false, "الصف الثانى الثانوى" },
                    { 14, false, "الثانوية العامة" }
                });

            migrationBuilder.InsertData(
                table: "IdentityRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Student" });

            migrationBuilder.InsertData(
                table: "IdentityRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Parent" });

            migrationBuilder.InsertData(
                table: "IdentityRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Teacher" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentityProviders_IdentityUserId",
                table: "ExternalIdentityProviders",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityActivations_IdentityUserId",
                table: "IdentityActivations",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityReferralTrackers_IdentityReferralUserId",
                table: "IdentityReferralTrackers",
                column: "IdentityReferralUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityReferralTrackers_IdentityUserId",
                table: "IdentityReferralTrackers",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRefreshTokens_IdentityUserId",
                table: "IdentityRefreshTokens",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRelations_PrimaryId",
                table: "IdentityRelations",
                column: "PrimaryId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRelations_SecondaryId",
                table: "IdentityRelations",
                column: "SecondaryId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentitySubjects_IdentityUserId",
                table: "IdentitySubjects",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityTemporaryValueHolders_IdentityUserId",
                table: "IdentityTemporaryValueHolders",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_AvatarId",
                table: "IdentityUsers",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_GovernorateId",
                table: "IdentityUsers",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_GradeId",
                table: "IdentityUsers",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_IdentityRoleId",
                table: "IdentityUsers",
                column: "IdentityRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_IdentitySchoolId",
                table: "IdentityUsers",
                column: "IdentitySchoolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalIdentityProviders");

            migrationBuilder.DropTable(
                name: "IdentityActivations");

            migrationBuilder.DropTable(
                name: "IdentityReferralTrackers");

            migrationBuilder.DropTable(
                name: "IdentityRefreshTokens");

            migrationBuilder.DropTable(
                name: "IdentityRelations");

            migrationBuilder.DropTable(
                name: "IdentitySubjects");

            migrationBuilder.DropTable(
                name: "IdentityTemporaryValueHolders");

            migrationBuilder.DropTable(
                name: "IdentityUsers");

            migrationBuilder.DropTable(
                name: "Avatars");

            migrationBuilder.DropTable(
                name: "Governorates");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "IdentityRoles");

            migrationBuilder.DropTable(
                name: "IdentitySchools");
        }
    }
}
