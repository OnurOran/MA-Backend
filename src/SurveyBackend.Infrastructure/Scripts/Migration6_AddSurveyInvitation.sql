BEGIN TRANSACTION;

-- Create SurveyInvitation table
CREATE TABLE [SurveyInvitation] (
    [Id] int NOT NULL IDENTITY,
    [SurveyId] int NOT NULL,
    [Token] nvarchar(8) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(254) NULL,
    [Phone] nvarchar(20) NULL,
    [DeliveryMethod] int NOT NULL,
    [Status] int NOT NULL,
    [SentAt] datetime2 NULL,
    [ViewedAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [ParticipationId] int NULL,
    [IsActive] bit NOT NULL,
    [IsDelete] bit NOT NULL,
    [CreateDate] datetime2 NOT NULL,
    [CreateEmployeeId] int NULL,
    [UpdateDate] datetime2 NULL,
    [UpdateEmployeeId] int NULL,
    [RowVersion] rowversion NULL,
    CONSTRAINT [PK_SurveyInvitation] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SurveyInvitation_Survey_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Survey] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SurveyInvitation_Participation_ParticipationId] FOREIGN KEY ([ParticipationId]) REFERENCES [Participation] ([Id]) ON DELETE SET NULL
);

-- Add InvitationId column to Participant table
ALTER TABLE [Participant] ADD [InvitationId] int NULL;

-- Create indexes
CREATE UNIQUE INDEX [IX_SurveyInvitation_Token] ON [SurveyInvitation] ([Token]);
CREATE INDEX [IX_SurveyInvitation_SurveyId] ON [SurveyInvitation] ([SurveyId]);
CREATE INDEX [IX_SurveyInvitation_ParticipationId] ON [SurveyInvitation] ([ParticipationId]);
CREATE INDEX [IX_Participant_InvitationId] ON [Participant] ([InvitationId]);

-- Add foreign key for Participant.InvitationId
ALTER TABLE [Participant] ADD CONSTRAINT [FK_Participant_SurveyInvitation_InvitationId]
    FOREIGN KEY ([InvitationId]) REFERENCES [SurveyInvitation] ([Id]) ON DELETE SET NULL;

-- Record migration in history
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260129000000_AddSurveyInvitation', N'10.0.0');

COMMIT;
GO
