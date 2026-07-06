using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAiDraftReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiDraftReply",
                table: "Tickets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiDraftReply",
                table: "Tickets");
        }
    }
}
