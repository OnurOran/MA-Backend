IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [ExternalIdentifier] nvarchar(100) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Parameters] (
        [Id] int NOT NULL,
        [Code] nvarchar(20) NULL,
        [GroupName] nvarchar(100) NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [ParentId] int NOT NULL,
        [LevelNo] int NOT NULL,
        [Symbol] nvarchar(10) NULL,
        [OrderNo] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Parameters] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Participants] (
        [Id] int NOT NULL IDENTITY,
        [ExternalId] uniqueidentifier NULL,
        [LdapUsername] nvarchar(100) NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Participants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Permissions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(200) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(200) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [DepartmentId] int NOT NULL,
        [PasswordHash] nvarchar(512) NULL,
        [IsSuperAdmin] bit NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Surveys] (
        [Id] int NOT NULL IDENTITY,
        [Slug] nvarchar(100) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [IntroText] nvarchar(256) NULL,
        [ConsentText] nvarchar(1000) NULL,
        [OutroText] nvarchar(150) NULL,
        [DepartmentId] int NOT NULL,
        [IsPublished] bit NOT NULL DEFAULT CAST(0 AS bit),
        [AccessType] int NOT NULL,
        [StartDate] datetime2 NULL,
        [EndDate] datetime2 NULL,
        [TypeId] int NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Surveys] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Surveys_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Surveys_Parameters_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Parameters] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [RoleId] int NOT NULL,
        [PermissionId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [UserRefreshTokens] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Token] nvarchar(512) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_UserRefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserRefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        [DepartmentId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId], [DepartmentId]),
        CONSTRAINT [FK_UserRoles_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Participations] (
        [Id] int NOT NULL IDENTITY,
        [SurveyId] int NOT NULL,
        [ParticipantId] int NOT NULL,
        [StartedAt] datetime2 NOT NULL,
        [CompletedAt] datetime2 NULL,
        [IpAddress] nvarchar(50) NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Participations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Participations_Participants_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participants] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Participations_Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Questions] (
        [Id] int NOT NULL IDENTITY,
        [SurveyId] int NOT NULL,
        [Text] nvarchar(1000) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Order] int NOT NULL,
        [Type] int NOT NULL,
        [IsRequired] bit NOT NULL,
        [AllowedAttachmentContentTypes] nvarchar(512) NULL,
        [TypeId] int NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Questions_Parameters_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Parameters] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Questions_Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Answers] (
        [Id] int NOT NULL IDENTITY,
        [ParticipationId] int NOT NULL,
        [QuestionId] int NOT NULL,
        [TextValue] nvarchar(2000) NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Answers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Answers_Participations_ParticipationId] FOREIGN KEY ([ParticipationId]) REFERENCES [Participations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Answers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [QuestionOptions] (
        [Id] int NOT NULL IDENTITY,
        [QuestionId] int NOT NULL,
        [Text] nvarchar(256) NOT NULL,
        [Order] int NOT NULL,
        [Value] int NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_QuestionOptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionOptions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [AnswerAttachments] (
        [Id] int NOT NULL IDENTITY,
        [AnswerId] int NOT NULL,
        [SurveyId] int NOT NULL,
        [DepartmentId] int NOT NULL,
        [FileName] nvarchar(128) NOT NULL,
        [ContentType] nvarchar(256) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [StoragePath] nvarchar(1024) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_AnswerAttachments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnswerAttachments_Answers_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [AnswerOptions] (
        [Id] int NOT NULL IDENTITY,
        [AnswerId] int NOT NULL,
        [QuestionOptionId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_AnswerOptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnswerOptions_Answers_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AnswerOptions_QuestionOptions_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOptions] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Attachments] (
        [Id] int NOT NULL IDENTITY,
        [OwnerType] int NOT NULL,
        [DepartmentId] int NOT NULL,
        [SurveyId] int NULL,
        [QuestionId] int NULL,
        [QuestionOptionId] int NULL,
        [FileName] nvarchar(128) NOT NULL,
        [ContentType] nvarchar(256) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [StoragePath] nvarchar(1024) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Attachments] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Attachments_SingleOwner] CHECK (
                    (
                        (CASE WHEN SurveyId IS NOT NULL THEN 1 ELSE 0 END) +
                        (CASE WHEN QuestionId IS NOT NULL THEN 1 ELSE 0 END) +
                        (CASE WHEN QuestionOptionId IS NOT NULL THEN 1 ELSE 0 END)
                    ) = 1),
        CONSTRAINT [FK_Attachments_QuestionOptions_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOptions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Attachments_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Attachments_Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [DependentQuestions] (
        [Id] int NOT NULL IDENTITY,
        [ParentQuestionOptionId] int NOT NULL,
        [ChildQuestionId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_DependentQuestions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DependentQuestions_QuestionOptions_ParentQuestionOptionId] FOREIGN KEY ([ParentQuestionOptionId]) REFERENCES [QuestionOptions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DependentQuestions_Questions_ChildQuestionId] FOREIGN KEY ([ChildQuestionId]) REFERENCES [Questions] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreateDate', N'CreateEmployeeId', N'Description', N'DisplayName', N'GroupName', N'IsActive', N'IsDelete', N'LevelNo', N'Name', N'OrderNo', N'ParentId', N'Symbol', N'UpdateDate', N'UpdateEmployeeId') AND [object_id] = OBJECT_ID(N'[Parameters]'))
        SET IDENTITY_INSERT [Parameters] ON;
    EXEC(N'INSERT INTO [Parameters] ([Id], [Code], [CreateDate], [CreateEmployeeId], [Description], [DisplayName], [GroupName], [IsActive], [IsDelete], [LevelNo], [Name], [OrderNo], [ParentId], [Symbol], [UpdateDate], [UpdateEmployeeId])
    VALUES (1, NULL, ''0001-01-01T00:00:00.0000000'', NULL, NULL, N''Parameters'', N''Parameters'', CAST(1 AS bit), CAST(0 AS bit), 0, N''Parameters'', 0, 0, NULL, NULL, NULL),
    (2, NULL, ''0001-01-01T00:00:00.0000000'', NULL, N''Anket erişim tipi parametreleri'', N''Anket Erişim Türleri'', N''SurveyAccessTypes'', CAST(1 AS bit), CAST(0 AS bit), 1, N''SurveyAccessTypes'', 0, 1, NULL, NULL, NULL),
    (3, N''INTERNAL'', ''0001-01-01T00:00:00.0000000'', NULL, N''Sadece kurum içi erişim'', N''Dahili'', N''SurveyAccessTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''Internal'', 0, 2, NULL, NULL, NULL),
    (4, N''PUBLIC'', ''0001-01-01T00:00:00.0000000'', NULL, N''Herkese açık erişim'', N''Halka Açık'', N''SurveyAccessTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''Public'', 1, 2, NULL, NULL, NULL),
    (5, NULL, ''0001-01-01T00:00:00.0000000'', NULL, N''Anket soru tipi parametreleri'', N''Soru Türleri'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 1, N''QuestionTypes'', 1, 1, NULL, NULL, NULL),
    (6, N''SINGLE_SELECT'', ''0001-01-01T00:00:00.0000000'', NULL, N''Tek seçenek seçilebilen soru tipi'', N''Tekli Seçim'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''SingleSelect'', 0, 5, NULL, NULL, NULL),
    (7, N''MULTI_SELECT'', ''0001-01-01T00:00:00.0000000'', NULL, N''Birden fazla seçenek seçilebilen soru tipi'', N''Çoklu Seçim'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''MultiSelect'', 1, 5, NULL, NULL, NULL),
    (8, N''OPEN_TEXT'', ''0001-01-01T00:00:00.0000000'', NULL, N''Serbest metin girişi yapılabilen soru tipi'', N''Açık Metin'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''OpenText'', 2, 5, NULL, NULL, NULL),
    (9, N''FILE_UPLOAD'', ''0001-01-01T00:00:00.0000000'', NULL, N''Dosya yüklemesi yapılabilen soru tipi'', N''Dosya Yükleme'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''FileUpload'', 3, 5, NULL, NULL, NULL),
    (10, N''CONDITIONAL'', ''0001-01-01T00:00:00.0000000'', NULL, N''Koşula bağlı olarak gösterilen soru tipi'', N''Koşullu'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''Conditional'', 4, 5, NULL, NULL, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreateDate', N'CreateEmployeeId', N'Description', N'DisplayName', N'GroupName', N'IsActive', N'IsDelete', N'LevelNo', N'Name', N'OrderNo', N'ParentId', N'Symbol', N'UpdateDate', N'UpdateEmployeeId') AND [object_id] = OBJECT_ID(N'[Parameters]'))
        SET IDENTITY_INSERT [Parameters] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AnswerAttachments_AnswerId] ON [AnswerAttachments] ([AnswerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_AnswerOptions_AnswerId] ON [AnswerOptions] ([AnswerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_AnswerOptions_QuestionOptionId] ON [AnswerOptions] ([QuestionOptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Answers_ParticipationId] ON [Answers] ([ParticipationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Answers_QuestionId] ON [Answers] ([QuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Attachments_QuestionId] ON [Attachments] ([QuestionId]) WHERE [QuestionId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Attachments_QuestionOptionId] ON [Attachments] ([QuestionOptionId]) WHERE [QuestionOptionId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Attachments_SurveyId] ON [Attachments] ([SurveyId]) WHERE [SurveyId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Departments_ExternalIdentifier] ON [Departments] ([ExternalIdentifier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_DependentQuestions_ChildQuestionId] ON [DependentQuestions] ([ChildQuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_DependentQuestions_ParentQuestionOptionId] ON [DependentQuestions] ([ParentQuestionOptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Participants_ExternalId] ON [Participants] ([ExternalId]) WHERE [ExternalId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Participants_LdapUsername] ON [Participants] ([LdapUsername]) WHERE [LdapUsername] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Participations_ParticipantId] ON [Participations] ([ParticipantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Participations_SurveyId] ON [Participations] ([SurveyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_QuestionOptions_QuestionId] ON [QuestionOptions] ([QuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Questions_SurveyId] ON [Questions] ([SurveyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Questions_TypeId] ON [Questions] ([TypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Surveys_DepartmentId] ON [Surveys] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Surveys_TypeId] ON [Surveys] ([TypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserRefreshTokens_Token] ON [UserRefreshTokens] ([Token]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_UserRefreshTokens_UserId] ON [UserRefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_UserRoles_DepartmentId] ON [UserRoles] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Users_DepartmentId] ON [Users] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251219110645_Initial', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerAttachments] DROP CONSTRAINT [FK_AnswerAttachments_Answers_AnswerId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerOptions] DROP CONSTRAINT [FK_AnswerOptions_Answers_AnswerId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerOptions] DROP CONSTRAINT [FK_AnswerOptions_QuestionOptions_QuestionOptionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Answers] DROP CONSTRAINT [FK_Answers_Participations_ParticipationId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Answers] DROP CONSTRAINT [FK_Answers_Questions_QuestionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachments] DROP CONSTRAINT [FK_Attachments_QuestionOptions_QuestionOptionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachments] DROP CONSTRAINT [FK_Attachments_Questions_QuestionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachments] DROP CONSTRAINT [FK_Attachments_Surveys_SurveyId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [DependentQuestions] DROP CONSTRAINT [FK_DependentQuestions_QuestionOptions_ParentQuestionOptionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [DependentQuestions] DROP CONSTRAINT [FK_DependentQuestions_Questions_ChildQuestionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participations] DROP CONSTRAINT [FK_Participations_Participants_ParticipantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participations] DROP CONSTRAINT [FK_Participations_Surveys_SurveyId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [QuestionOptions] DROP CONSTRAINT [FK_QuestionOptions_Questions_QuestionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Questions] DROP CONSTRAINT [FK_Questions_Parameters_TypeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Questions] DROP CONSTRAINT [FK_Questions_Surveys_SurveyId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [RolePermissions] DROP CONSTRAINT [FK_RolePermissions_Permissions_PermissionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [RolePermissions] DROP CONSTRAINT [FK_RolePermissions_Roles_RoleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Surveys] DROP CONSTRAINT [FK_Surveys_Departments_DepartmentId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Surveys] DROP CONSTRAINT [FK_Surveys_Parameters_TypeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRefreshTokens] DROP CONSTRAINT [FK_UserRefreshTokens_Users_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRoles] DROP CONSTRAINT [FK_UserRoles_Departments_DepartmentId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRoles] DROP CONSTRAINT [FK_UserRoles_Roles_RoleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRoles] DROP CONSTRAINT [FK_UserRoles_Users_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Departments_DepartmentId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Users] DROP CONSTRAINT [PK_Users];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRoles] DROP CONSTRAINT [PK_UserRoles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRefreshTokens] DROP CONSTRAINT [PK_UserRefreshTokens];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Surveys] DROP CONSTRAINT [PK_Surveys];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Roles] DROP CONSTRAINT [PK_Roles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [RolePermissions] DROP CONSTRAINT [PK_RolePermissions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Questions] DROP CONSTRAINT [PK_Questions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [QuestionOptions] DROP CONSTRAINT [PK_QuestionOptions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Permissions] DROP CONSTRAINT [PK_Permissions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participations] DROP CONSTRAINT [PK_Participations];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participants] DROP CONSTRAINT [PK_Participants];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Parameters] DROP CONSTRAINT [PK_Parameters];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [DependentQuestions] DROP CONSTRAINT [PK_DependentQuestions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Departments] DROP CONSTRAINT [PK_Departments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachments] DROP CONSTRAINT [PK_Attachments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachments] DROP CONSTRAINT [CK_Attachments_SingleOwner];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Answers] DROP CONSTRAINT [PK_Answers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerOptions] DROP CONSTRAINT [PK_AnswerOptions];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerAttachments] DROP CONSTRAINT [PK_AnswerAttachments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Users]', N'User', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[UserRoles]', N'UserRole', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[UserRefreshTokens]', N'UserRefreshToken', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Surveys]', N'Survey', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Roles]', N'Role', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[RolePermissions]', N'RolePermission', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Questions]', N'Question', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[QuestionOptions]', N'QuestionOption', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Permissions]', N'Permission', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Participations]', N'Participation', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Participants]', N'Participant', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Parameters]', N'Parameter', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[DependentQuestions]', N'DependentQuestion', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Departments]', N'Department', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Attachments]', N'Attachment', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Answers]', N'Answer', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[AnswerOptions]', N'AnswerOption', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[AnswerAttachments]', N'AnswerAttachment', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[User].[IX_Users_DepartmentId]', N'IX_User_DepartmentId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[UserRole].[IX_UserRoles_RoleId]', N'IX_UserRole_RoleId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[UserRole].[IX_UserRoles_DepartmentId]', N'IX_UserRole_DepartmentId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[UserRefreshToken].[IX_UserRefreshTokens_UserId]', N'IX_UserRefreshToken_UserId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[UserRefreshToken].[IX_UserRefreshTokens_Token]', N'IX_UserRefreshToken_Token', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Survey].[IX_Surveys_TypeId]', N'IX_Survey_TypeId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Survey].[IX_Surveys_DepartmentId]', N'IX_Survey_DepartmentId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[RolePermission].[IX_RolePermissions_PermissionId]', N'IX_RolePermission_PermissionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Question].[IX_Questions_TypeId]', N'IX_Question_TypeId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Question].[IX_Questions_SurveyId]', N'IX_Question_SurveyId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[QuestionOption].[IX_QuestionOptions_QuestionId]', N'IX_QuestionOption_QuestionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Participation].[IX_Participations_SurveyId]', N'IX_Participation_SurveyId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Participation].[IX_Participations_ParticipantId]', N'IX_Participation_ParticipantId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Participant].[IX_Participants_LdapUsername]', N'IX_Participant_LdapUsername', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Participant].[IX_Participants_ExternalId]', N'IX_Participant_ExternalId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[DependentQuestion].[IX_DependentQuestions_ParentQuestionOptionId]', N'IX_DependentQuestion_ParentQuestionOptionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[DependentQuestion].[IX_DependentQuestions_ChildQuestionId]', N'IX_DependentQuestion_ChildQuestionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Department].[IX_Departments_ExternalIdentifier]', N'IX_Department_ExternalIdentifier', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Attachment].[IX_Attachments_SurveyId]', N'IX_Attachment_SurveyId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Attachment].[IX_Attachments_QuestionOptionId]', N'IX_Attachment_QuestionOptionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Attachment].[IX_Attachments_QuestionId]', N'IX_Attachment_QuestionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Answer].[IX_Answers_QuestionId]', N'IX_Answer_QuestionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[Answer].[IX_Answers_ParticipationId]', N'IX_Answer_ParticipationId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[AnswerOption].[IX_AnswerOptions_QuestionOptionId]', N'IX_AnswerOption_QuestionOptionId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[AnswerOption].[IX_AnswerOptions_AnswerId]', N'IX_AnswerOption_AnswerId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC sp_rename N'[AnswerAttachment].[IX_AnswerAttachments_AnswerId]', N'IX_AnswerAttachment_AnswerId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [User] ADD CONSTRAINT [PK_User] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRole] ADD CONSTRAINT [PK_UserRole] PRIMARY KEY ([UserId], [RoleId], [DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRefreshToken] ADD CONSTRAINT [PK_UserRefreshToken] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Survey] ADD CONSTRAINT [PK_Survey] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Role] ADD CONSTRAINT [PK_Role] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [RolePermission] ADD CONSTRAINT [PK_RolePermission] PRIMARY KEY ([RoleId], [PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Question] ADD CONSTRAINT [PK_Question] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [QuestionOption] ADD CONSTRAINT [PK_QuestionOption] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Permission] ADD CONSTRAINT [PK_Permission] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participation] ADD CONSTRAINT [PK_Participation] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participant] ADD CONSTRAINT [PK_Participant] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Parameter] ADD CONSTRAINT [PK_Parameter] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [DependentQuestion] ADD CONSTRAINT [PK_DependentQuestion] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Department] ADD CONSTRAINT [PK_Department] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachment] ADD CONSTRAINT [PK_Attachment] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Answer] ADD CONSTRAINT [PK_Answer] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerOption] ADD CONSTRAINT [PK_AnswerOption] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerAttachment] ADD CONSTRAINT [PK_AnswerAttachment] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    EXEC(N'ALTER TABLE [Attachment] ADD CONSTRAINT [CK_Attachment_SingleOwner] CHECK (
                    (
                        (CASE WHEN SurveyId IS NOT NULL THEN 1 ELSE 0 END) +
                        (CASE WHEN QuestionId IS NOT NULL THEN 1 ELSE 0 END) +
                        (CASE WHEN QuestionOptionId IS NOT NULL THEN 1 ELSE 0 END)
                    ) = 1)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Answer] ADD CONSTRAINT [FK_Answer_Participation_ParticipationId] FOREIGN KEY ([ParticipationId]) REFERENCES [Participation] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Answer] ADD CONSTRAINT [FK_Answer_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerAttachment] ADD CONSTRAINT [FK_AnswerAttachment_Answer_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answer] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerOption] ADD CONSTRAINT [FK_AnswerOption_Answer_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answer] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [AnswerOption] ADD CONSTRAINT [FK_AnswerOption_QuestionOption_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOption] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachment] ADD CONSTRAINT [FK_Attachment_QuestionOption_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOption] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachment] ADD CONSTRAINT [FK_Attachment_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Attachment] ADD CONSTRAINT [FK_Attachment_Survey_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Survey] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [DependentQuestion] ADD CONSTRAINT [FK_DependentQuestion_QuestionOption_ParentQuestionOptionId] FOREIGN KEY ([ParentQuestionOptionId]) REFERENCES [QuestionOption] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [DependentQuestion] ADD CONSTRAINT [FK_DependentQuestion_Question_ChildQuestionId] FOREIGN KEY ([ChildQuestionId]) REFERENCES [Question] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participation] ADD CONSTRAINT [FK_Participation_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Participation] ADD CONSTRAINT [FK_Participation_Survey_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Survey] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Question] ADD CONSTRAINT [FK_Question_Parameter_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Parameter] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Question] ADD CONSTRAINT [FK_Question_Survey_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Survey] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [QuestionOption] ADD CONSTRAINT [FK_QuestionOption_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [RolePermission] ADD CONSTRAINT [FK_RolePermission_Permission_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permission] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [RolePermission] ADD CONSTRAINT [FK_RolePermission_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Survey] ADD CONSTRAINT [FK_Survey_Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Department] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [Survey] ADD CONSTRAINT [FK_Survey_Parameter_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Parameter] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [User] ADD CONSTRAINT [FK_User_Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Department] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRefreshToken] ADD CONSTRAINT [FK_UserRefreshToken_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRole] ADD CONSTRAINT [FK_UserRole_Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Department] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRole] ADD CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    ALTER TABLE [UserRole] ADD CONSTRAINT [FK_UserRole_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222121208_RenameTablesToPluralToSingular'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251222121208_RenameTablesToPluralToSingular', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222135217_AddSlugUniqueIndex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Survey_Slug] ON [Survey] ([Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251222135217_AddSlugUniqueIndex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251222135217_AddSlugUniqueIndex', N'9.0.0');
END;

COMMIT;
GO

