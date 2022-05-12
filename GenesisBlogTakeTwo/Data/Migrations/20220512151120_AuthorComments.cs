using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenesisBlogTakeTwo.Data.Migrations
{
    public partial class AuthorComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "BlogPostComment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostComment_AuthorId",
                table: "BlogPostComment",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostComment_AspNetUsers_AuthorId",
                table: "BlogPostComment",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostComment_AspNetUsers_AuthorId",
                table: "BlogPostComment");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostComment_AuthorId",
                table: "BlogPostComment");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "BlogPostComment");
        }
    }
}
