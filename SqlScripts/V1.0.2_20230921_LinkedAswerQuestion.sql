BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230921094105_LinkedAnswerQuestion')
BEGIN
    CREATE INDEX [IX_Answers_LinkedQuestionId] ON [Answers] ([LinkedQuestionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230921094105_LinkedAnswerQuestion')
BEGIN
    ALTER TABLE [Answers] ADD CONSTRAINT [FK_Answers_Question_LinkedQuestionId] FOREIGN KEY ([LinkedQuestionId]) REFERENCES [Question] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230921094105_LinkedAnswerQuestion')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230921094105_LinkedAnswerQuestion', N'7.0.5');
END;
GO

COMMIT;
GO

