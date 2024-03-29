﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TweetBook.Data.Migrations
{
	public partial class AddPosts : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Posts",
				columns: table => new
				{
					Id = table.Column<Guid>(nullable: false),
					Name = table.Column<string>(maxLength: 250, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Posts", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Posts");
		}
	}
}
