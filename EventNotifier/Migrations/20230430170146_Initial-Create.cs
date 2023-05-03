using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventNotifier.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
                migrationBuilder.CreateTable(
                    name: "Events",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "integer", nullable: false)
                            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                        Name = table.Column<string>(type: "text", nullable: false),
                        Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        Description = table.Column<string>(type: "text", nullable: false),
                        MaxSubscribers = table.Column<long>(type: "bigint", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Events", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "Users",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "integer", nullable: false)
                            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                        Username = table.Column<string>(type: "text", nullable: false),
                        Email = table.Column<string>(type: "text", nullable: false),
                        Password = table.Column<string>(type: "text", nullable: false),
                        Role = table.Column<int>(type: "integer", nullable: false),
                        ConfirmCode = table.Column<string>(type: "text", nullable: true),
                        EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Users", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "EventUser",
                    columns: table => new
                    {
                        EventSubscriptionsId = table.Column<int>(type: "integer", nullable: false),
                        SubscribersId = table.Column<int>(type: "integer", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_EventUser", x => new { x.EventSubscriptionsId, x.SubscribersId });
                        table.ForeignKey(
                            name: "FK_EventUser_Events_EventSubscriptionsId",
                            column: x => x.EventSubscriptionsId,
                            principalTable: "Events",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_EventUser_Users_SubscribersId",
                            column: x => x.SubscribersId,
                            principalTable: "Users",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.InsertData(
                    table: "Users",
                    columns: new[] { "Id", "ConfirmCode", "Email", "EmailConfirmed", "Password", "Role", "Username" },
                    values: new object[] { 1, null, "vlad.vlasov77@mail", true, "Aboba12345", 0, "Vladislav Vlasov" });

                migrationBuilder.CreateIndex(
                    name: "IX_EventUser_SubscribersId",
                    table: "EventUser",
                    column: "SubscribersId");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventUser");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
