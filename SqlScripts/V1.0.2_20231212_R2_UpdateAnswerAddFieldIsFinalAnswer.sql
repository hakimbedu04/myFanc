BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231212172105_UpdateAswerAddFieldIsFinalAnswer')
BEGIN
    ALTER TABLE [Answers] ADD [IsFinalAnswer] BIT NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231212172105_UpdateAswerAddFieldIsFinalAnswer')
BEGIN
    CREATE TABLE [AnswersFinalAnswerTextsTranslations] (
        [AnswersFinalAnswerTextsId] int NOT NULL,
        [FinalAnswerTextsId] int NOT NULL,
        CONSTRAINT [PK_AnswersFinalAnswerTextsTranslations] PRIMARY KEY ([AnswersFinalAnswerTextsId], [FinalAnswerTextsId]),
        CONSTRAINT [FK_AnswersFinalAnswerTextsTranslations_Answers_FinalAnswerTextsId] FOREIGN KEY ([AnswersFinalAnswerTextsId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AnswersFinalAnswerTextsTranslations_Translations_TextsId] FOREIGN KEY ([FinalAnswerTextsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231212172105_UpdateAswerAddFieldIsFinalAnswer')
BEGIN
    CREATE INDEX [IX_AnswersFinalAnswerTextsTranslations_TextsId] ON [AnswersFinalAnswerTextsTranslations] ([FinalAnswerTextsId]);
END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231212172105_UpdateAswerAddFieldIsFinalAnswer')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231212172105_UpdateAswerAddFieldIsFinalAnswer', N'7.0.5');
END;
GO

COMMIT;
GO

