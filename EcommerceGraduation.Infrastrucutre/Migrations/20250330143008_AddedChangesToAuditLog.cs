using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceGraduation.Infrastrucutre.Migrations
{
    /// <inheritdoc />
    public partial class AddedChangesToAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Changes",
                table: "AuditLog",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Changes",
                table: "AuditLog");
        }
    }
}
