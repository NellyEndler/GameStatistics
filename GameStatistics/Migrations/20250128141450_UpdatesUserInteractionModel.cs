using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStatistics.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesUserInteractionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeSTamp",
                table: "UserInteractions",
                newName: "TimeStamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "UserInteractions",
                newName: "TimeSTamp");
        }
    }
}
