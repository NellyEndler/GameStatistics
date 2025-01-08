using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStatistics.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDataTypeInWorkshop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Lägg till en ny kolumn med rätt datatyp (float)
            migrationBuilder.AddColumn<double>(
                name: "TotalWorkshopTimeInSeconds_New",
                table: "Workshops",
                nullable: false,
                defaultValue: 0.0);

            // Om du behöver kopiera data från den gamla kolumnen, kan du här lägga till SQL för att göra det
            // T.ex. om du har DateTime-värden som behöver konverteras till sekunder eller liknande
            migrationBuilder.Sql("UPDATE Workshops SET TotalWorkshopTimeInSeconds_New = DATEDIFF(SECOND, '1900-01-01', TotalWorkshopTimeInSeconds)");

            // Ta bort den gamla kolumnen
            migrationBuilder.DropColumn(
                name: "TotalWorkshopTimeInSeconds",
                table: "Workshops");

            // Byt namn på den nya kolumnen till den gamla kolumnens namn
            migrationBuilder.RenameColumn(
                name: "TotalWorkshopTimeInSeconds_New",
                table: "Workshops",
                newName: "TotalWorkshopTimeInSeconds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Återställ till den gamla datatypen om migrationen rullas tillbaka
            migrationBuilder.AddColumn<DateTime>(
                name: "TotalWorkshopTimeInSeconds",
                table: "Workshops",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime());

            // Ta bort den nya kolumnen
            migrationBuilder.DropColumn(
                name: "TotalWorkshopTimeInSeconds_New",
                table: "Workshops");
        }
    }
}

