using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPersonalDiary.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFieldNameToDeletionRequestedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeletionScheduledAt",
                table: "Users",
                newName: "DeletionRequestedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeletionRequestedAt",
                table: "Users",
                newName: "DeletionScheduledAt");
        }
    }
}
