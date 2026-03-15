using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeSense.Api.Migrations
{
    /// <inheritdoc />
    public partial class fix1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_Devices_DeviceId",
                table: "SensorReadings");

            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_DeviceId_RecordedAtUtc",
                table: "SensorReadings");

            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceCode",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "InfraredDetected",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "ReceivedAtUtc",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "RecordedAtUtc",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "TemperatureC",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "VibrationDetected",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "DeviceCode",
                table: "Devices");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "SensorReadings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "BatchId",
                table: "SensorReadings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "RawPayload",
                table: "SensorReadings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SensorType",
                table: "SensorReadings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "SensorReadings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "SensorReadings",
                type: "decimal(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Devices",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "MacAddress",
                table: "Devices",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DeviceThresholds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceThresholds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceThresholds_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadingBatches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TriggeredByThreshold = table.Column<bool>(type: "bit", nullable: false),
                    TriggerSensorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TriggerValue = table.Column<decimal>(type: "decimal(8,3)", precision: 8, scale: 3, nullable: true),
                    TriggerRule = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadingBatches_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<long>(type: "bigint", nullable: false),
                    SensorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TriggerValue = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    Rule = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alerts_ReadingBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "ReadingBatches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_BatchId_SensorType",
                table: "SensorReadings",
                columns: new[] { "BatchId", "SensorType" });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MacAddress",
                table: "Devices",
                column: "MacAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_BatchId",
                table: "Alerts",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DeviceId",
                table: "Alerts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceThresholds_DeviceId_SensorType",
                table: "DeviceThresholds",
                columns: new[] { "DeviceId", "SensorType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReadingBatches_BatchKey",
                table: "ReadingBatches",
                column: "BatchKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReadingBatches_DeviceId",
                table: "ReadingBatches",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_ReadingBatches_BatchId",
                table: "SensorReadings",
                column: "BatchId",
                principalTable: "ReadingBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_ReadingBatches_BatchId",
                table: "SensorReadings");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "DeviceThresholds");

            migrationBuilder.DropTable(
                name: "ReadingBatches");

            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_BatchId_SensorType",
                table: "SensorReadings");

            migrationBuilder.DropIndex(
                name: "IX_Devices_MacAddress",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "RawPayload",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "SensorType",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "MacAddress",
                table: "Devices");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "SensorReadings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "SensorReadings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Humidity",
                table: "SensorReadings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InfraredDetected",
                table: "SensorReadings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedAtUtc",
                table: "SensorReadings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordedAtUtc",
                table: "SensorReadings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TemperatureC",
                table: "SensorReadings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VibrationDetected",
                table: "SensorReadings",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Devices",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "DeviceCode",
                table: "Devices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_DeviceId_RecordedAtUtc",
                table: "SensorReadings",
                columns: new[] { "DeviceId", "RecordedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceCode",
                table: "Devices",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_Devices_DeviceId",
                table: "SensorReadings",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
