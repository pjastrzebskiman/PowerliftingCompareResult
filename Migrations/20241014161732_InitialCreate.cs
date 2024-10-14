using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerliftingCompareResult.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiftResults",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: true),
                    EQ = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<float>(type: "real", nullable: true),
                    AgeClass = table.Column<string>(type: "text", nullable: true),
                    BodyWeight = table.Column<float>(type: "real", nullable: true),
                    WeightClass = table.Column<string>(type: "text", nullable: true),
                    Squat1 = table.Column<float>(type: "real", nullable: true),
                    Squat2 = table.Column<float>(type: "real", nullable: true),
                    Squat3 = table.Column<float>(type: "real", nullable: true),
                    Bench1 = table.Column<float>(type: "real", nullable: true),
                    Bench2 = table.Column<float>(type: "real", nullable: true),
                    Bench3 = table.Column<float>(type: "real", nullable: true),
                    Deadlift1 = table.Column<float>(type: "real", nullable: true),
                    Deadlift2 = table.Column<float>(type: "real", nullable: true),
                    Deadlift3 = table.Column<float>(type: "real", nullable: true),
                    Squat = table.Column<float>(type: "real", nullable: true),
                    Bench = table.Column<float>(type: "real", nullable: true),
                    Deadlift = table.Column<float>(type: "real", nullable: true),
                    Total = table.Column<float>(type: "real", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Federation = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MeetCountry = table.Column<string>(type: "text", nullable: true),
                    MeetName = table.Column<string>(type: "text", nullable: true)
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
