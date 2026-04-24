using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FertileNotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriberServiceAccountFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsServiceAccount",
                table: "Subscribers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "McpApiKey",
                table: "Subscribers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scopes",
                table: "ApiKeys",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsServiceAccount",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "McpApiKey",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "Scopes",
                table: "ApiKeys");
        }
    }
}
