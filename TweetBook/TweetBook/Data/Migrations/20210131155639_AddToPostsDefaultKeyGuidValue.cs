using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TweetBook.Data.Migrations
{
	public partial class AddToPostsDefaultKeyGuidValue : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<Guid>(
				name: "Id",
				table: "Posts",
				nullable: false,
				defaultValueSql: "NEWID()",
				oldClrType: typeof(Guid),
				oldType: "uniqueidentifier");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<Guid>(
				name: "Id",
				table: "Posts",
				type: "uniqueidentifier",
				nullable: false,
				oldClrType: typeof(Guid),
				oldDefaultValueSql: "NEWID()");
		}
	}
}
