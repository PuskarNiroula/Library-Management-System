using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class RentLogMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentLogs_Incomes_IncomeId",
                table: "RentLogs");

            migrationBuilder.DropTable(
                name: "Incomes");

            migrationBuilder.DropColumn(
                name: "Cause",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Fines");

            migrationBuilder.RenameColumn(
                name: "IncomeId",
                table: "RentLogs",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_RentLogs_IncomeId",
                table: "RentLogs",
                newName: "IX_RentLogs_StudentId");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedById",
                table: "RentLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "RentLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentedDate",
                table: "RentLogs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedDate",
                table: "RentLogs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Fines",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Fines",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Fines",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LateDays",
                table: "Fines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                table: "Fines",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentLogId",
                table: "Fines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RentLogs_ApprovedById",
                table: "RentLogs",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_RentLogs_BookId",
                table: "RentLogs",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_RentLogId",
                table: "Fines",
                column: "RentLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fines_RentLogs_RentLogId",
                table: "Fines",
                column: "RentLogId",
                principalTable: "RentLogs",
                principalColumn: "RentLogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RentLogs_Books_BookId",
                table: "RentLogs",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RentLogs_Users_ApprovedById",
                table: "RentLogs",
                column: "ApprovedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RentLogs_Users_StudentId",
                table: "RentLogs",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fines_RentLogs_RentLogId",
                table: "Fines");

            migrationBuilder.DropForeignKey(
                name: "FK_RentLogs_Books_BookId",
                table: "RentLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_RentLogs_Users_ApprovedById",
                table: "RentLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_RentLogs_Users_StudentId",
                table: "RentLogs");

            migrationBuilder.DropIndex(
                name: "IX_RentLogs_ApprovedById",
                table: "RentLogs");

            migrationBuilder.DropIndex(
                name: "IX_RentLogs_BookId",
                table: "RentLogs");

            migrationBuilder.DropIndex(
                name: "IX_Fines_RentLogId",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "RentLogs");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "RentLogs");

            migrationBuilder.DropColumn(
                name: "RentedDate",
                table: "RentLogs");

            migrationBuilder.DropColumn(
                name: "ReturnedDate",
                table: "RentLogs");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "LateDays",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "Fines");

            migrationBuilder.DropColumn(
                name: "RentLogId",
                table: "Fines");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "RentLogs",
                newName: "IncomeId");

            migrationBuilder.RenameIndex(
                name: "IX_RentLogs_StudentId",
                table: "RentLogs",
                newName: "IX_RentLogs_IncomeId");

            migrationBuilder.AddColumn<string>(
                name: "Cause",
                table: "Fines",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Fines",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Incomes",
                columns: table => new
                {
                    IncomeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    FineId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<float>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.IncomeId);
                    table.ForeignKey(
                        name: "FK_Incomes_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Incomes_Fines_FineId",
                        column: x => x.FineId,
                        principalTable: "Fines",
                        principalColumn: "FineId");
                    table.ForeignKey(
                        name: "FK_Incomes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_BookId",
                table: "Incomes",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_FineId",
                table: "Incomes",
                column: "FineId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_UserId",
                table: "Incomes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentLogs_Incomes_IncomeId",
                table: "RentLogs",
                column: "IncomeId",
                principalTable: "Incomes",
                principalColumn: "IncomeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
