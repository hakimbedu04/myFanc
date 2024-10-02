BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231010102521_R3-updateAnswerEntity')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Answers]') AND [c].[name] = N'Link');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Answers] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Answers] DROP COLUMN [Link];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231010102521_R3-updateAnswerEntity')
BEGIN
    CREATE TABLE [AnswersDetailsTranslations] (
        [AnswersDetailsId] int NOT NULL,
        [DetailsId] int NOT NULL,
        CONSTRAINT [PK_AnswersDetailsTranslations] PRIMARY KEY ([AnswersDetailsId], [DetailsId]),
        CONSTRAINT [FK_AnswersDetailsTranslations_Answers_AnswersDetailsId] FOREIGN KEY ([AnswersDetailsId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AnswersDetailsTranslations_Translations_DetailsId] FOREIGN KEY ([DetailsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231010102521_R3-updateAnswerEntity')
BEGIN
    CREATE TABLE [AnswersLinksTranslations] (
        [AnswersLinksId] int NOT NULL,
        [LinksId] int NOT NULL,
        CONSTRAINT [PK_AnswersLinksTranslations] PRIMARY KEY ([AnswersLinksId], [LinksId]),
        CONSTRAINT [FK_AnswersLinksTranslations_Answers_AnswersLinksId] FOREIGN KEY ([AnswersLinksId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AnswersLinksTranslations_Translations_LinksId] FOREIGN KEY ([LinksId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231010102521_R3-updateAnswerEntity')
BEGIN
    CREATE INDEX [IX_AnswersDetailsTranslations_DetailsId] ON [AnswersDetailsTranslations] ([DetailsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231010102521_R3-updateAnswerEntity')
BEGIN
    CREATE INDEX [IX_AnswersLinksTranslations_LinksId] ON [AnswersLinksTranslations] ([LinksId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231010102521_R3-updateAnswerEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231010102521_R3-updateAnswerEntity', N'7.0.5');
END;
GO

COMMIT;
GO

