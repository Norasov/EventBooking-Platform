using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_EventId",
                table: "Reviews");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketTypes_Quantity_NonNegative",
                table: "TicketTypes",
                sql: "\"Quantity\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TicketTypes_SoldCount_LessOrEqualQuantity",
                table: "TicketTypes",
                sql: "\"SoldCount\" <= \"Quantity\"");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Code",
                table: "Tickets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EventId_AttendeeId",
                table: "Reviews",
                columns: new[] { "EventId", "AttendeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_Status_StartDate",
                table: "Events",
                columns: new[] { "Status", "StartDate" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Events_EndDate_After_StartDate",
                table: "Events",
                sql: "\"EndDate\" > \"StartDate\"");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_Email",
                table: "Attendees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketTypes_Quantity_NonNegative",
                table: "TicketTypes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TicketTypes_SoldCount_LessOrEqualQuantity",
                table: "TicketTypes");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Code",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_EventId_AttendeeId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Events_Status_StartDate",
                table: "Events");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Events_EndDate_After_StartDate",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Attendees_Email",
                table: "Attendees");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EventId",
                table: "Reviews",
                column: "EventId");
        }
    }
}
