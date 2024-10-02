using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFanc.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BeginR3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dossiers");

            migrationBuilder.DropTable(
                name: "Faqs");

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wizards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterUserId = table.Column<int>(type: "int", nullable: true),
                    LatestUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestUpdateUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wizards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WizardId = table.Column<int>(type: "int", nullable: false),
                    IsFirstQuestion = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterUserId = table.Column<int>(type: "int", nullable: true),
                    LatestUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestUpdateUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_Wizards_WizardId",
                        column: x => x.WizardId,
                        principalTable: "Wizards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationWizard",
                columns: table => new
                {
                    IntroductionTextsId = table.Column<int>(type: "int", nullable: false),
                    WizardsTextsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationWizard", x => new { x.IntroductionTextsId, x.WizardsTextsId });
                    table.ForeignKey(
                        name: "FK_TranslationWizard_Translations_IntroductionTextsId",
                        column: x => x.IntroductionTextsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationWizard_Wizards_WizardsTextsId",
                        column: x => x.WizardsTextsId,
                        principalTable: "Wizards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WizardTitlesTranslations",
                columns: table => new
                {
                    TitlesId = table.Column<int>(type: "int", nullable: false),
                    WizardsTitlesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WizardTitlesTranslations", x => new { x.TitlesId, x.WizardsTitlesId });
                    table.ForeignKey(
                        name: "FK_WizardTitlesTranslations_Translations_TitlesId",
                        column: x => x.TitlesId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WizardTitlesTranslations_Wizards_WizardsTitlesId",
                        column: x => x.WizardsTitlesId,
                        principalTable: "Wizards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkedQuestionId = table.Column<int>(type: "int", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterUserId = table.Column<int>(type: "int", nullable: true),
                    LatestUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestUpdateUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBreadcrumb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBreadcrumb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBreadcrumb_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionsTextsTranslations",
                columns: table => new
                {
                    QuestionsTextsId = table.Column<int>(type: "int", nullable: false),
                    TextsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsTextsTranslations", x => new { x.QuestionsTextsId, x.TextsId });
                    table.ForeignKey(
                        name: "FK_QuestionsTextsTranslations_Question_QuestionsTextsId",
                        column: x => x.QuestionsTextsId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionsTextsTranslations_Translations_TextsId",
                        column: x => x.TextsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionsTitlesTranslations",
                columns: table => new
                {
                    QuestionsTitlesId = table.Column<int>(type: "int", nullable: false),
                    TitlesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsTitlesTranslations", x => new { x.QuestionsTitlesId, x.TitlesId });
                    table.ForeignKey(
                        name: "FK_QuestionsTitlesTranslations_Question_QuestionsTitlesId",
                        column: x => x.QuestionsTitlesId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionsTitlesTranslations_Translations_TitlesId",
                        column: x => x.TitlesId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswersLabelsTranslations",
                columns: table => new
                {
                    AnswersLabelsId = table.Column<int>(type: "int", nullable: false),
                    LabelsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswersLabelsTranslations", x => new { x.AnswersLabelsId, x.LabelsId });
                    table.ForeignKey(
                        name: "FK_AnswersLabelsTranslations_Answers_AnswersLabelsId",
                        column: x => x.AnswersLabelsId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswersLabelsTranslations_Translations_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBreadcrumbItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreadcrumbId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsALoop = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBreadcrumbItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBreadcrumbItem_QuestionBreadcrumb_BreadcrumbId",
                        column: x => x.BreadcrumbId,
                        principalTable: "QuestionBreadcrumb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionBreadcrumbItem_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswersLabelsTranslations_LabelsId",
                table: "AnswersLabelsTranslations",
                column: "LabelsId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_WizardId",
                table: "Question",
                column: "WizardId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBreadcrumb_QuestionId",
                table: "QuestionBreadcrumb",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBreadcrumbItem_BreadcrumbId",
                table: "QuestionBreadcrumbItem",
                column: "BreadcrumbId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBreadcrumbItem_QuestionId",
                table: "QuestionBreadcrumbItem",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsTextsTranslations_TextsId",
                table: "QuestionsTextsTranslations",
                column: "TextsId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsTitlesTranslations_TitlesId",
                table: "QuestionsTitlesTranslations",
                column: "TitlesId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationWizard_WizardsTextsId",
                table: "TranslationWizard",
                column: "WizardsTextsId");

            migrationBuilder.CreateIndex(
                name: "IX_WizardTitlesTranslations_WizardsTitlesId",
                table: "WizardTitlesTranslations",
                column: "WizardsTitlesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswersLabelsTranslations");

            migrationBuilder.DropTable(
                name: "QuestionBreadcrumbItem");

            migrationBuilder.DropTable(
                name: "QuestionsTextsTranslations");

            migrationBuilder.DropTable(
                name: "QuestionsTitlesTranslations");

            migrationBuilder.DropTable(
                name: "TranslationWizard");

            migrationBuilder.DropTable(
                name: "WizardTitlesTranslations");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "QuestionBreadcrumb");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Wizards");

            migrationBuilder.CreateTable(
                name: "Dossiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<int>(type: "int", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterUserId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestUpdateUserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dossiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faqs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faqs", x => x.Id);
                });
        }
    }
}
