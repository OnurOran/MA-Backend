using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBackend.Infrastructure.Migrations
{

    public partial class RenameTablesToPluralToSingular : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerAttachments_Answers_AnswerId",
                table: "AnswerAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_AnswerOptions_Answers_AnswerId",
                table: "AnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_AnswerOptions_QuestionOptions_QuestionOptionId",
                table: "AnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Participations_ParticipationId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_QuestionOptions_QuestionOptionId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Questions_QuestionId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Surveys_SurveyId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_DependentQuestions_QuestionOptions_ParentQuestionOptionId",
                table: "DependentQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_DependentQuestions_Questions_ChildQuestionId",
                table: "DependentQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Participations_Participants_ParticipantId",
                table: "Participations");

            migrationBuilder.DropForeignKey(
                name: "FK_Participations_Surveys_SurveyId",
                table: "Participations");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptions_Questions_QuestionId",
                table: "QuestionOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Parameters_TypeId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Surveys_SurveyId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_Departments_DepartmentId",
                table: "Surveys");

            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_Parameters_TypeId",
                table: "Surveys");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshTokens_Users_UserId",
                table: "UserRefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Departments_DepartmentId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshTokens",
                table: "UserRefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Surveys",
                table: "Surveys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participations",
                table: "Participations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participants",
                table: "Participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parameters",
                table: "Parameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DependentQuestions",
                table: "DependentQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Attachments_SingleOwner",
                table: "Attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerOptions",
                table: "AnswerOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerAttachments",
                table: "AnswerAttachments");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRole");

            migrationBuilder.RenameTable(
                name: "UserRefreshTokens",
                newName: "UserRefreshToken");

            migrationBuilder.RenameTable(
                name: "Surveys",
                newName: "Survey");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "RolePermission");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "Question");

            migrationBuilder.RenameTable(
                name: "QuestionOptions",
                newName: "QuestionOption");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permission");

            migrationBuilder.RenameTable(
                name: "Participations",
                newName: "Participation");

            migrationBuilder.RenameTable(
                name: "Participants",
                newName: "Participant");

            migrationBuilder.RenameTable(
                name: "Parameters",
                newName: "Parameter");

            migrationBuilder.RenameTable(
                name: "DependentQuestions",
                newName: "DependentQuestion");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Department");

            migrationBuilder.RenameTable(
                name: "Attachments",
                newName: "Attachment");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answer");

            migrationBuilder.RenameTable(
                name: "AnswerOptions",
                newName: "AnswerOption");

            migrationBuilder.RenameTable(
                name: "AnswerAttachments",
                newName: "AnswerAttachment");

            migrationBuilder.RenameIndex(
                name: "IX_Users_DepartmentId",
                table: "User",
                newName: "IX_User_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_DepartmentId",
                table: "UserRole",
                newName: "IX_UserRole_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRefreshTokens_UserId",
                table: "UserRefreshToken",
                newName: "IX_UserRefreshToken_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRefreshTokens_Token",
                table: "UserRefreshToken",
                newName: "IX_UserRefreshToken_Token");

            migrationBuilder.RenameIndex(
                name: "IX_Surveys_TypeId",
                table: "Survey",
                newName: "IX_Survey_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Surveys_DepartmentId",
                table: "Survey",
                newName: "IX_Survey_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermission",
                newName: "IX_RolePermission_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_TypeId",
                table: "Question",
                newName: "IX_Question_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_SurveyId",
                table: "Question",
                newName: "IX_Question_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOption",
                newName: "IX_QuestionOption_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Participations_SurveyId",
                table: "Participation",
                newName: "IX_Participation_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_Participations_ParticipantId",
                table: "Participation",
                newName: "IX_Participation_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Participants_LdapUsername",
                table: "Participant",
                newName: "IX_Participant_LdapUsername");

            migrationBuilder.RenameIndex(
                name: "IX_Participants_ExternalId",
                table: "Participant",
                newName: "IX_Participant_ExternalId");

            migrationBuilder.RenameIndex(
                name: "IX_DependentQuestions_ParentQuestionOptionId",
                table: "DependentQuestion",
                newName: "IX_DependentQuestion_ParentQuestionOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_DependentQuestions_ChildQuestionId",
                table: "DependentQuestion",
                newName: "IX_DependentQuestion_ChildQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_ExternalIdentifier",
                table: "Department",
                newName: "IX_Department_ExternalIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_SurveyId",
                table: "Attachment",
                newName: "IX_Attachment_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_QuestionOptionId",
                table: "Attachment",
                newName: "IX_Attachment_QuestionOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_QuestionId",
                table: "Attachment",
                newName: "IX_Attachment_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "Answer",
                newName: "IX_Answer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_ParticipationId",
                table: "Answer",
                newName: "IX_Answer_ParticipationId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerOptions_QuestionOptionId",
                table: "AnswerOption",
                newName: "IX_AnswerOption_QuestionOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerOptions_AnswerId",
                table: "AnswerOption",
                newName: "IX_AnswerOption_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerAttachments_AnswerId",
                table: "AnswerAttachment",
                newName: "IX_AnswerAttachment_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId", "DepartmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshToken",
                table: "UserRefreshToken",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Survey",
                table: "Survey",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Question",
                table: "Question",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionOption",
                table: "QuestionOption",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                table: "Permission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participation",
                table: "Participation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participant",
                table: "Participant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parameter",
                table: "Parameter",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DependentQuestion",
                table: "DependentQuestion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answer",
                table: "Answer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerOption",
                table: "AnswerOption",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerAttachment",
                table: "AnswerAttachment",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Attachment_SingleOwner",
                table: "Attachment",
                sql: "\n                (\n                    (CASE WHEN SurveyId IS NOT NULL THEN 1 ELSE 0 END) +\n                    (CASE WHEN QuestionId IS NOT NULL THEN 1 ELSE 0 END) +\n                    (CASE WHEN QuestionOptionId IS NOT NULL THEN 1 ELSE 0 END)\n                ) = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Participation_ParticipationId",
                table: "Answer",
                column: "ParticipationId",
                principalTable: "Participation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Question_QuestionId",
                table: "Answer",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerAttachment_Answer_AnswerId",
                table: "AnswerAttachment",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerOption_Answer_AnswerId",
                table: "AnswerOption",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerOption_QuestionOption_QuestionOptionId",
                table: "AnswerOption",
                column: "QuestionOptionId",
                principalTable: "QuestionOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_QuestionOption_QuestionOptionId",
                table: "Attachment",
                column: "QuestionOptionId",
                principalTable: "QuestionOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Question_QuestionId",
                table: "Attachment",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Survey_SurveyId",
                table: "Attachment",
                column: "SurveyId",
                principalTable: "Survey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DependentQuestion_QuestionOption_ParentQuestionOptionId",
                table: "DependentQuestion",
                column: "ParentQuestionOptionId",
                principalTable: "QuestionOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DependentQuestion_Question_ChildQuestionId",
                table: "DependentQuestion",
                column: "ChildQuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participation_Participant_ParticipantId",
                table: "Participation",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participation_Survey_SurveyId",
                table: "Participation",
                column: "SurveyId",
                principalTable: "Survey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Parameter_TypeId",
                table: "Question",
                column: "TypeId",
                principalTable: "Parameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Survey_SurveyId",
                table: "Question",
                column: "SurveyId",
                principalTable: "Survey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOption_Question_QuestionId",
                table: "QuestionOption",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                table: "RolePermission",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Survey_Department_DepartmentId",
                table: "Survey",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Survey_Parameter_TypeId",
                table: "Survey",
                column: "TypeId",
                principalTable: "Parameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Department_DepartmentId",
                table: "User",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshToken_User_UserId",
                table: "UserRefreshToken",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Department_DepartmentId",
                table: "UserRole",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Participation_ParticipationId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Question_QuestionId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_AnswerAttachment_Answer_AnswerId",
                table: "AnswerAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_AnswerOption_Answer_AnswerId",
                table: "AnswerOption");

            migrationBuilder.DropForeignKey(
                name: "FK_AnswerOption_QuestionOption_QuestionOptionId",
                table: "AnswerOption");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_QuestionOption_QuestionOptionId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Question_QuestionId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Survey_SurveyId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_DependentQuestion_QuestionOption_ParentQuestionOptionId",
                table: "DependentQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_DependentQuestion_Question_ChildQuestionId",
                table: "DependentQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_Participation_Participant_ParticipantId",
                table: "Participation");

            migrationBuilder.DropForeignKey(
                name: "FK_Participation_Survey_SurveyId",
                table: "Participation");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Parameter_TypeId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Survey_SurveyId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOption_Question_QuestionId",
                table: "QuestionOption");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Role_RoleId",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_Survey_Department_DepartmentId",
                table: "Survey");

            migrationBuilder.DropForeignKey(
                name: "FK_Survey_Parameter_TypeId",
                table: "Survey");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Department_DepartmentId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshToken_User_UserId",
                table: "UserRefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Department_DepartmentId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshToken",
                table: "UserRefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Survey",
                table: "Survey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionOption",
                table: "QuestionOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Question",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                table: "Permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participation",
                table: "Participation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Participant",
                table: "Participant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parameter",
                table: "Parameter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DependentQuestion",
                table: "DependentQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Attachment_SingleOwner",
                table: "Attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerOption",
                table: "AnswerOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerAttachment",
                table: "AnswerAttachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answer",
                table: "Answer");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserRefreshToken",
                newName: "UserRefreshTokens");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Survey",
                newName: "Surveys");

            migrationBuilder.RenameTable(
                name: "RolePermission",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "QuestionOption",
                newName: "QuestionOptions");

            migrationBuilder.RenameTable(
                name: "Question",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "Permission",
                newName: "Permissions");

            migrationBuilder.RenameTable(
                name: "Participation",
                newName: "Participations");

            migrationBuilder.RenameTable(
                name: "Participant",
                newName: "Participants");

            migrationBuilder.RenameTable(
                name: "Parameter",
                newName: "Parameters");

            migrationBuilder.RenameTable(
                name: "DependentQuestion",
                newName: "DependentQuestions");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "Departments");

            migrationBuilder.RenameTable(
                name: "Attachment",
                newName: "Attachments");

            migrationBuilder.RenameTable(
                name: "AnswerOption",
                newName: "AnswerOptions");

            migrationBuilder.RenameTable(
                name: "AnswerAttachment",
                newName: "AnswerAttachments");

            migrationBuilder.RenameTable(
                name: "Answer",
                newName: "Answers");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_DepartmentId",
                table: "UserRoles",
                newName: "IX_UserRoles_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRefreshToken_UserId",
                table: "UserRefreshTokens",
                newName: "IX_UserRefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRefreshToken_Token",
                table: "UserRefreshTokens",
                newName: "IX_UserRefreshTokens_Token");

            migrationBuilder.RenameIndex(
                name: "IX_User_DepartmentId",
                table: "Users",
                newName: "IX_Users_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Survey_TypeId",
                table: "Surveys",
                newName: "IX_Surveys_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Survey_DepartmentId",
                table: "Surveys",
                newName: "IX_Surveys_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionOption_QuestionId",
                table: "QuestionOptions",
                newName: "IX_QuestionOptions_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Question_TypeId",
                table: "Questions",
                newName: "IX_Questions_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Question_SurveyId",
                table: "Questions",
                newName: "IX_Questions_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_Participation_SurveyId",
                table: "Participations",
                newName: "IX_Participations_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_Participation_ParticipantId",
                table: "Participations",
                newName: "IX_Participations_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Participant_LdapUsername",
                table: "Participants",
                newName: "IX_Participants_LdapUsername");

            migrationBuilder.RenameIndex(
                name: "IX_Participant_ExternalId",
                table: "Participants",
                newName: "IX_Participants_ExternalId");

            migrationBuilder.RenameIndex(
                name: "IX_DependentQuestion_ParentQuestionOptionId",
                table: "DependentQuestions",
                newName: "IX_DependentQuestions_ParentQuestionOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_DependentQuestion_ChildQuestionId",
                table: "DependentQuestions",
                newName: "IX_DependentQuestions_ChildQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Department_ExternalIdentifier",
                table: "Departments",
                newName: "IX_Departments_ExternalIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_SurveyId",
                table: "Attachments",
                newName: "IX_Attachments_SurveyId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_QuestionOptionId",
                table: "Attachments",
                newName: "IX_Attachments_QuestionOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_QuestionId",
                table: "Attachments",
                newName: "IX_Attachments_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerOption_QuestionOptionId",
                table: "AnswerOptions",
                newName: "IX_AnswerOptions_QuestionOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerOption_AnswerId",
                table: "AnswerOptions",
                newName: "IX_AnswerOptions_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerAttachment_AnswerId",
                table: "AnswerAttachments",
                newName: "IX_AnswerAttachments_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_QuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_ParticipationId",
                table: "Answers",
                newName: "IX_Answers_ParticipationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId", "DepartmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshTokens",
                table: "UserRefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Surveys",
                table: "Surveys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participations",
                table: "Participations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Participants",
                table: "Participants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parameters",
                table: "Parameters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DependentQuestions",
                table: "DependentQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerOptions",
                table: "AnswerOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerAttachments",
                table: "AnswerAttachments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Attachments_SingleOwner",
                table: "Attachments",
                sql: "\n                (\n                    (CASE WHEN SurveyId IS NOT NULL THEN 1 ELSE 0 END) +\n                    (CASE WHEN QuestionId IS NOT NULL THEN 1 ELSE 0 END) +\n                    (CASE WHEN QuestionOptionId IS NOT NULL THEN 1 ELSE 0 END)\n                ) = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerAttachments_Answers_AnswerId",
                table: "AnswerAttachments",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerOptions_Answers_AnswerId",
                table: "AnswerOptions",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerOptions_QuestionOptions_QuestionOptionId",
                table: "AnswerOptions",
                column: "QuestionOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Participations_ParticipationId",
                table: "Answers",
                column: "ParticipationId",
                principalTable: "Participations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_QuestionOptions_QuestionOptionId",
                table: "Attachments",
                column: "QuestionOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Questions_QuestionId",
                table: "Attachments",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Surveys_SurveyId",
                table: "Attachments",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DependentQuestions_QuestionOptions_ParentQuestionOptionId",
                table: "DependentQuestions",
                column: "ParentQuestionOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DependentQuestions_Questions_ChildQuestionId",
                table: "DependentQuestions",
                column: "ChildQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participations_Participants_ParticipantId",
                table: "Participations",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participations_Surveys_SurveyId",
                table: "Participations",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptions_Questions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Parameters_TypeId",
                table: "Questions",
                column: "TypeId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Surveys_SurveyId",
                table: "Questions",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_Departments_DepartmentId",
                table: "Surveys",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_Parameters_TypeId",
                table: "Surveys",
                column: "TypeId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshTokens_Users_UserId",
                table: "UserRefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Departments_DepartmentId",
                table: "UserRoles",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
