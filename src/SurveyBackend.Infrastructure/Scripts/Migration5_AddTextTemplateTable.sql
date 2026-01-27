BEGIN TRANSACTION;
CREATE TABLE [TextTemplate] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [Content] nvarchar(4000) NOT NULL,
    [Type] int NOT NULL,
    [DepartmentId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDelete] bit NOT NULL,
    [CreateDate] datetime2 NOT NULL,
    [CreateEmployeeId] int NULL,
    [UpdateDate] datetime2 NULL,
    [UpdateEmployeeId] int NULL,
    [RowVersion] rowversion NULL,
    CONSTRAINT [PK_TextTemplate] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TextTemplate_Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Department] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_TextTemplate_DepartmentId] ON [TextTemplate] ([DepartmentId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260127091717_AddTextTemplateTable', N'10.0.0');

COMMIT;
GO

