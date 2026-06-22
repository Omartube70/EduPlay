using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyConceptsSampleQuestionsChoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyConcepts",
                columns: table => new
                {
                    KeyConceptID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentAnalysisID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyConcepts", x => x.KeyConceptID);
                    table.ForeignKey(
                        name: "FK_KeyConcepts_DocumentAnalyses_DocumentAnalysisID",
                        column: x => x.DocumentAnalysisID,
                        principalTable: "DocumentAnalyses",
                        principalColumn: "DocumentAnalysisID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SampleQuestions",
                columns: table => new
                {
                    SampleQuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnswerIndex = table.Column<int>(type: "int", nullable: true),
                    DocumentAnalysisID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleQuestions", x => x.SampleQuestionID);
                    table.ForeignKey(
                        name: "FK_SampleQuestions_DocumentAnalyses_DocumentAnalysisID",
                        column: x => x.DocumentAnalysisID,
                        principalTable: "DocumentAnalyses",
                        principalColumn: "DocumentAnalysisID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionChoices",
                columns: table => new
                {
                    QuestionChoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    SampleQuestionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionChoices", x => x.QuestionChoiceID);
                    table.ForeignKey(
                        name: "FK_QuestionChoices_SampleQuestions_SampleQuestionID",
                        column: x => x.SampleQuestionID,
                        principalTable: "SampleQuestions",
                        principalColumn: "SampleQuestionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyConcepts_DocumentAnalysisID",
                table: "KeyConcepts",
                column: "DocumentAnalysisID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionChoices_SampleQuestionID",
                table: "QuestionChoices",
                column: "SampleQuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_SampleQuestions_DocumentAnalysisID",
                table: "SampleQuestions",
                column: "DocumentAnalysisID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyConcepts");

            migrationBuilder.DropTable(
                name: "QuestionChoices");

            migrationBuilder.DropTable(
                name: "SampleQuestions");
        }
    }
}
