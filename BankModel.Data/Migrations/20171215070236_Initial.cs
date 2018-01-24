using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DataAccess.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name:"AllowOverdraw",
                table:"TemplateAccount",
                nullable: false,
                defaultValue:false
                );

            migrationBuilder.AddColumn<bool>(
                name: "AllowOverdraw",
                table: "Accounts",
                nullable: false,
                defaultValue: false
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name:"AllowOverdraw",
                table:"TemplateAccount"
                );

            migrationBuilder.DropColumn(
                name: "AllowOverdraw",
                table: "Accounts"
                );
        }
    }
}
