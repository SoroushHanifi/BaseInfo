using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class __InitialMainTitle__ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MainTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScopeId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainTitles_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_DisplayOrder",
                table: "MainTitles",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_IsDelete",
                table: "MainTitles",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_Name",
                table: "MainTitles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_ScopeId",
                table: "MainTitles",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_ScopeId_IsDelete_DisplayOrder",
                table: "MainTitles",
                columns: new[] { "ScopeId", "IsDelete", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainTitles");
        }
    }
}
