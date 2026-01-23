## Domain Model (current implementation)

### Enums
- `AccessType`: Internal, Public
- `QuestionType`: SingleSelect, MultiSelect, OpenText, FileUpload, Conditional
- `SurveyStatus`: Draft, Published (not used in EF)
- `AttachmentOwnerType`: Survey, Question, Option

### Core Entities & Relationships
- **Department**
  - Id (PK), Name, ExternalIdentifier (unique)
  - One-to-many Users; referenced by Surveys; UserRoles.

- **User**
  - Id (PK), Username, Email, DepartmentId (FK), PasswordHash?, IsSuperAdmin, CreatedAt, UpdatedAt
  - One-to-many RefreshTokens; one-to-many UserRoles
  - FK to Department (Restrict)

- **RefreshToken**
  - Id (PK), Token (unique), UserId (FK), ExpiresAt, CreatedAt

- **Role**
  - Id (PK), Name, Description
  - One-to-many RolePermissions

- **Permission**
  - Id (PK), Name, Description

- **RolePermission**
  - Composite PK (RoleId, PermissionId), FK to Role, FK to Permission

- **UserRole**
  - Composite PK (UserId, RoleId, DepartmentId)
  - FK to Role; FK to Department (Restrict)

- **Survey**
  - Id (PK), Title, Description?, IntroText?, ConsentText?, OutroText?, CreatedBy (string), DepartmentId (FK), CreatedAt, IsActive (default false), AccessType, StartDate?, EndDate?
  - One-to-many Questions (Cascade)
  - One-to-one Attachment (Cascade)
  - FK to Department (Restrict)

- **Question**
  - Id (PK), SurveyId (FK), Text, Description?, Order, Type (QuestionType), IsRequired, AllowedAttachmentContentTypes?
  - One-to-many QuestionOptions (Cascade)
  - One-to-one Attachment (Restrict)
  - One-to-many Answers (Cascade via Answer.Question FK)

- **QuestionOption**
  - Id (PK), QuestionId (FK), Text, Order, Value?
  - One-to-many DependentQuestions (ParentOption FK, Restrict)
  - One-to-one Attachment (Cascade)

- **DependentQuestion**
  - Id (PK), ParentQuestionOptionId (FK to QuestionOption, Restrict), ChildQuestionId (FK to Question, Restrict)
  - Enables conditional branching; order of children is via Question.Order

- **Attachment** (for survey/question/option definitions)
  - Id (PK), OwnerType (AttachmentOwnerType), DepartmentId, SurveyId?, QuestionId?, QuestionOptionId?, FileName, ContentType, SizeBytes, StoragePath, CreatedAt
  - Unique indexes on SurveyId, QuestionId, QuestionOptionId (each nullable)
  - Check constraint ensures exactly one owner FK is set
  - One-to-one with Survey (Cascade), Question (Restrict), QuestionOption (Restrict)

- **Participant**
  - Id (PK), ExternalId?, LdapUsername?, CreatedAt
  - Unique indexes on ExternalId (when not null) and LdapUsername (when not null)

- **Participation**
  - Id (PK), SurveyId (FK), ParticipantId (FK), StartedAt, CompletedAt?, IpAddress?
  - One-to-many Answers (Cascade)
  - FK to Participant (Cascade); FK to Survey (Restrict)

- **Answer**
  - Id (PK), ParticipationId (FK), QuestionId (FK), TextValue?
  - One-to-many AnswerOptions (Cascade)
  - One-to-one AnswerAttachment (Cascade)
  - FK to Question (Cascade)

- **AnswerOption**
  - Id (PK), AnswerId (FK), QuestionOptionId (FK Restrict)

- **AnswerAttachment** (uploads from respondents)
  - Id (PK), AnswerId (FK), SurveyId, DepartmentId, FileName, ContentType, SizeBytes, StoragePath, CreatedAt
  - Unique index on AnswerId
  - One-to-one with Answer (Cascade)

### Cascades / Delete behaviors (from EF configuration)
- Survey → Questions (Cascade)
- Question → Options, Answers (Cascade)
- QuestionOption → DependentQuestions (Restrict), Attachment (Cascade)
- Participation → Answers (Cascade)
- Answer → AnswerOptions (Cascade), AnswerAttachment (Cascade)
- Attachment: Survey (Cascade), Question (Restrict), QuestionOption (Restrict)
- UserRole Department FK (Restrict); Department FK on Survey/User/UserRole (Restrict)

### Key rules
- File upload questions can specify allowed content types (stored as comma string); non-file questions should not set allowed types.
- Attachments limited to allowed MIME set (png, jpg/jpeg, webp, pdf) and max 5MB (enforced in services).
- Conditional branching uses DependentQuestion linking parent option to child question.
