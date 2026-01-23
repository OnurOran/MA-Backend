BEGIN TRANSACTION;
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