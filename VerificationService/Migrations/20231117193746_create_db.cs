using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VerificationApp.Migrations
{
    /// <inheritdoc />
    public partial class create_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InspectingOrganizations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectingOrganizations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SmallBusinessEntities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmallBusinessEntities", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Verifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BusinessEntity = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Organization = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BeginPeriod = table.Column<DateOnly>(type: "date", nullable: false),
                    EndPeriod = table.Column<DateOnly>(type: "date", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "InspectingOrganizations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "10a0480d-fc87-40a3-a119-b0f8143f7c06", "Роскомнадзор" },
                    { "227adafb-f156-4d4c-8221-4bc41584f4ec", "Росприроднадзор " },
                    { "4b58cc46-df09-4180-916c-5ed5de7a1718", "Федеральная налоговая служба" },
                    { "c65eb449-4a72-4895-9077-25487bb6e736", "Росстандарт" },
                    { "ddddf500-0e36-4fb4-b98d-4515fc9b8cb5", "Роспотребнадзор" }
                });

            migrationBuilder.InsertData(
                table: "SmallBusinessEntities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "1d6cb852-c300-4e2a-8e0f-5c80e58c2da3", "Apple" },
                    { "3169ed79-36de-4644-b008-980791d2ddc0", "Microsoft" },
                    { "45292c75-e749-4927-87e3-5787414886b6", "Amazon" },
                    { "7ca5f558-336e-4a53-9bfb-b5c9c5132f17", "Google" },
                    { "943180b0-bbc5-41c6-b74f-09a45731a4d7", "Meta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectingOrganizations_Name",
                table: "InspectingOrganizations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmallBusinessEntities_Name",
                table: "SmallBusinessEntities",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectingOrganizations");

            migrationBuilder.DropTable(
                name: "SmallBusinessEntities");

            migrationBuilder.DropTable(
                name: "Verifications");
        }
    }
}
