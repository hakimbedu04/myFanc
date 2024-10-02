using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFanc.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReviewR1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Claims",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LastestUpdateUserId",
                table: "Users",
                newName: "LatestUpdateUserId");

            migrationBuilder.RenameColumn(
                name: "LastestUpdateTime",
                table: "Users",
                newName: "LatestUpdateTime");

            migrationBuilder.RenameColumn(
                name: "LastSychData",
                table: "Users",
                newName: "LatestSynchronization");

            migrationBuilder.RenameColumn(
                name: "LastConnected",
                table: "Users",
                newName: "LatestConnection");

            migrationBuilder.RenameColumn(
                name: "LastestUpdateUserId",
                table: "Dossiers",
                newName: "LatestUpdateUserId");

            migrationBuilder.RenameColumn(
                name: "LastestUpdateTime",
                table: "Dossiers",
                newName: "LatestUpdateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LatestUpdateUserId",
                table: "Users",
                newName: "LastestUpdateUserId");

            migrationBuilder.RenameColumn(
                name: "LatestUpdateTime",
                table: "Users",
                newName: "LastestUpdateTime");

            migrationBuilder.RenameColumn(
                name: "LatestSynchronization",
                table: "Users",
                newName: "LastSychData");

            migrationBuilder.RenameColumn(
                name: "LatestConnection",
                table: "Users",
                newName: "LastConnected");

            migrationBuilder.RenameColumn(
                name: "LatestUpdateUserId",
                table: "Dossiers",
                newName: "LastestUpdateUserId");

            migrationBuilder.RenameColumn(
                name: "LatestUpdateTime",
                table: "Dossiers",
                newName: "LastestUpdateTime");

            migrationBuilder.AddColumn<string>(
                name: "Claims",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
