using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketActivities_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketActivities_Users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketActivities_ActorUserId",
                table: "TicketActivities",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketActivities_TicketId_CreatedAt",
                table: "TicketActivities",
                columns: new[] { "TicketId", "CreatedAt" });

            migrationBuilder.Sql("""
                INSERT INTO "TicketActivities"
                    ("Id", "TicketId", "Type", "Message", "ActorUserId", "CreatedAt")
                SELECT
                    gen_random_uuid(),
                    "Id",
                    'Created',
                    'Ticket created.',
                    NULL,
                    "CreatedAt"
                FROM "Tickets";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TicketActivities"
                    ("Id", "TicketId", "Type", "Message", "ActorUserId", "CreatedAt")
                SELECT
                    gen_random_uuid(),
                    ticket."Id",
                    'Assigned',
                    CONCAT(
                        'Ticket assigned to ',
                        COALESCE(userAccount."Name", 'an agent'),
                        '.'),
                    ticket."AssignedToUserId",
                    COALESCE(ticket."AssignedAt", ticket."UpdatedAt")
                FROM "Tickets" AS ticket
                LEFT JOIN "Users" AS userAccount
                    ON userAccount."Id" = ticket."AssignedToUserId"
                WHERE ticket."AssignedToUserId" IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "TicketActivities"
                    ("Id", "TicketId", "Type", "Message", "ActorUserId", "CreatedAt")
                SELECT
                    gen_random_uuid(),
                    reply."TicketId",
                    'ReplySent',
                    CONCAT('Email reply sent to ', reply."RecipientEmail", '.'),
                    reply."SentByUserId",
                    reply."SentAt"
                FROM "TicketReplies" AS reply;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketActivities");
        }
    }
}
