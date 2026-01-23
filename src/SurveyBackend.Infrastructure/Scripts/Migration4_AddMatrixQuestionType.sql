BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [Question] ADD [MatrixExplanationLabel] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [Question] ADD [MatrixScale1Label] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [Question] ADD [MatrixScale2Label] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [Question] ADD [MatrixScale3Label] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [Question] ADD [MatrixScale4Label] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [Question] ADD [MatrixScale5Label] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [Question] ADD [MatrixShowExplanation] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [AnswerOption] ADD [Explanation] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    ALTER TABLE [AnswerOption] ADD [ScaleValue] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122092458_AddMatrixQuestionType'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260122092458_AddMatrixQuestionType', N'10.0.0');
END;

COMMIT;
GO

