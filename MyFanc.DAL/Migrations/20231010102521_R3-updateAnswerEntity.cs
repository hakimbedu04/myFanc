using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFanc.DAL.Migrations
{
    /// <inheritdoc />
    public partial class R3updateAnswerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "Answers");

            migrationBuilder.CreateTable(
                name: "AnswersDetailsTranslations",
                columns: table => new
                {
                    AnswersDetailsId = table.Column<int>(type: "int", nullable: false),
                    DetailsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswersDetailsTranslations", x => new { x.AnswersDetailsId, x.DetailsId });
                    table.ForeignKey(
                        name: "FK_AnswersDetailsTranslations_Answers_AnswersDetailsId",
                        column: x => x.AnswersDetailsId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswersDetailsTranslations_Translations_DetailsId",
                        column: x => x.DetailsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswersLinksTranslations",
                columns: table => new
                {
                    AnswersLinksId = table.Column<int>(type: "int", nullable: false),
                    LinksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswersLinksTranslations", x => new { x.AnswersLinksId, x.LinksId });
                    table.ForeignKey(
                        name: "FK_AnswersLinksTranslations_Answers_AnswersLinksId",
                        column: x => x.AnswersLinksId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswersLinksTranslations_Translations_LinksId",
                        column: x => x.LinksId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswersDetailsTranslations_DetailsId",
                table: "AnswersDetailsTranslations",
                column: "DetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswersLinksTranslations_LinksId",
                table: "AnswersLinksTranslations",
                column: "LinksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswersDetailsTranslations");

            migrationBuilder.DropTable(
                name: "AnswersLinksTranslations");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
