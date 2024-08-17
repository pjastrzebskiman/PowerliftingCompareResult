using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerliftingCompareResult.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiftResults",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EQ = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<float>(type: "real", nullable: false),
                    AgeClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyWeight = table.Column<float>(type: "real", nullable: false),
                    WeightClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Squat1 = table.Column<float>(type: "real", nullable: false),
                    Squat2 = table.Column<float>(type: "real", nullable: false),
                    Squat3 = table.Column<float>(type: "real", nullable: false),
                    Bench1 = table.Column<float>(type: "real", nullable: false),
                    Bench2 = table.Column<float>(type: "real", nullable: false),
                    Bench3 = table.Column<float>(type: "real", nullable: false),
                    Deadlift1 = table.Column<float>(type: "real", nullable: false),
                    Deadlift2 = table.Column<float>(type: "real", nullable: false),
                    Deadlift3 = table.Column<float>(type: "real", nullable: false),
                    Squat = table.Column<float>(type: "real", nullable: false),
                    Bench = table.Column<float>(type: "real", nullable: false),
                    Deadlift = table.Column<float>(type: "real", nullable: false),
                    Total = table.Column<float>(type: "real", nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Federation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeetName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiftResults");
        }
    }
}
