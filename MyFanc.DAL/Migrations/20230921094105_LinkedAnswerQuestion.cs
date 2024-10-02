using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFanc.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LinkedAnswerQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Answers_LinkedQuestionId",
                table: "Answers",
                column: "LinkedQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Question_LinkedQuestionId",
                table: "Answers",
                column: "LinkedQuestionId",
                principalTable: "Question",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Question_LinkedQuestionId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_LinkedQuestionId",
                table: "Answers");
        }
    }
}
