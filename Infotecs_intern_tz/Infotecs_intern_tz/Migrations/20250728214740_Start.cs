using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infotecs_intern_tz.Migrations
{
    /// <inheritdoc />
    public partial class Start : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "results",
                columns: table => new
                {
                    fileName = table.Column<string>(type: "text", nullable: false),
                    result_deltaTime = table.Column<long>(type: "bigint", nullable: false),
                    result_minDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    result_averageExecutionTime = table.Column<long>(type: "bigint", nullable: false),
                    result_averageValue = table.Column<float>(type: "real", nullable: false),
                    result_medianValue = table.Column<float>(type: "real", nullable: false),
                    result_maxValue = table.Column<float>(type: "real", nullable: false),
                    result_minValue = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_results", x => x.fileName);
                });

            migrationBuilder.CreateTable(
                name: "values",
                columns: table => new
                {
                    fileName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_values", x => x.fileName);
                });

            migrationBuilder.CreateTable(
                name: "ValueSchema",
                columns: table => new
                {
                    ValueEntryfileName = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    executionTime = table.Column<long>(type: "bigint", nullable: false),
                    value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueSchema", x => new { x.ValueEntryfileName, x.Id });
                    table.ForeignKey(
                        name: "FK_ValueSchema_values_ValueEntryfileName",
                        column: x => x.ValueEntryfileName,
                        principalTable: "values",
                        principalColumn: "fileName",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "results");

            migrationBuilder.DropTable(
                name: "ValueSchema");

            migrationBuilder.DropTable(
                name: "values");
        }
    }
}
