﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReslifeWorkorderManagement.Migrations
{
    /// <inheritdoc />
    public partial class addTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "WorkOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkOrder",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WorkOrder");
        }
    }
}
