﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcToDo.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToDoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(nullable: false),
                    Completed = table.Column<bool>(nullable: false, defaultValue: false),
                    Color = table.Column<string>(nullable: false, defaultValue: "#fff")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDoItems");
        }
    }
}
