using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class __InitialServiceFeature__ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                table: "Scopes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                table: "MainTitles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                table: "Departments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ServiceFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainTitleServiceFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainTitleId = table.Column<int>(type: "int", nullable: false),
                    ServiceFeatureId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainTitleServiceFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainTitleServiceFeatures_MainTitles_MainTitleId",
                        column: x => x.MainTitleId,
                        principalTable: "MainTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MainTitleServiceFeatures_ServiceFeatures_ServiceFeatureId",
                        column: x => x.ServiceFeatureId,
                        principalTable: "ServiceFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_CreateDate",
                table: "Scopes",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_CreateUserId",
                table: "Scopes",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_DepartmentId_IsDelete",
                table: "Scopes",
                columns: new[] { "DepartmentId", "IsDelete" });

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_IsDelete",
                table: "Scopes",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_CreateDate",
                table: "MainTitles",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_CreateUserId",
                table: "MainTitles",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitles_IsDelete_Amount",
                table: "MainTitles",
                columns: new[] { "IsDelete", "Amount" });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CreateDate",
                table: "Departments",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CreateUserId",
                table: "Departments",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsDelete",
                table: "Departments",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_CreateUserId",
                table: "MainTitleServiceFeatures",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_DisplayOrder",
                table: "MainTitleServiceFeatures",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_IsActive",
                table: "MainTitleServiceFeatures",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_IsDelete",
                table: "MainTitleServiceFeatures",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_MainTitleId",
                table: "MainTitleServiceFeatures",
                column: "MainTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_MainTitleId_IsDelete_IsActive",
                table: "MainTitleServiceFeatures",
                columns: new[] { "MainTitleId", "IsDelete", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_MainTitleId_ServiceFeatureId",
                table: "MainTitleServiceFeatures",
                columns: new[] { "MainTitleId", "ServiceFeatureId" },
                unique: true,
                filter: "[IsDelete] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_MainTitleId_ServiceFeatureId_IsDelete",
                table: "MainTitleServiceFeatures",
                columns: new[] { "MainTitleId", "ServiceFeatureId", "IsDelete" });

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_ServiceFeatureId",
                table: "MainTitleServiceFeatures",
                column: "ServiceFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_MainTitleServiceFeatures_ServiceFeatureId_IsDelete_IsActive",
                table: "MainTitleServiceFeatures",
                columns: new[] { "ServiceFeatureId", "IsDelete", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_Code",
                table: "ServiceFeatures",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_CreateUserId",
                table: "ServiceFeatures",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_DisplayOrder",
                table: "ServiceFeatures",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_IsActive",
                table: "ServiceFeatures",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_IsDelete",
                table: "ServiceFeatures",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_IsDelete_IsActive_DisplayOrder",
                table: "ServiceFeatures",
                columns: new[] { "IsDelete", "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_Name",
                table: "ServiceFeatures",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainTitleServiceFeatures");

            migrationBuilder.DropTable(
                name: "ServiceFeatures");

            migrationBuilder.DropIndex(
                name: "IX_Scopes_CreateDate",
                table: "Scopes");

            migrationBuilder.DropIndex(
                name: "IX_Scopes_CreateUserId",
                table: "Scopes");

            migrationBuilder.DropIndex(
                name: "IX_Scopes_DepartmentId_IsDelete",
                table: "Scopes");

            migrationBuilder.DropIndex(
                name: "IX_Scopes_IsDelete",
                table: "Scopes");

            migrationBuilder.DropIndex(
                name: "IX_MainTitles_CreateDate",
                table: "MainTitles");

            migrationBuilder.DropIndex(
                name: "IX_MainTitles_CreateUserId",
                table: "MainTitles");

            migrationBuilder.DropIndex(
                name: "IX_MainTitles_IsDelete_Amount",
                table: "MainTitles");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CreateDate",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CreateUserId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_IsDelete",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                table: "Scopes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                table: "MainTitles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
