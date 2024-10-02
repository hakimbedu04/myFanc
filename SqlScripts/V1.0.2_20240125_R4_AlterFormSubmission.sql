BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240125093006_AlterFormSubmission')
BEGIN
    ALTER TABLE [dbo].[FormSubmissions] ADD [FormSubmissionType] INT NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240125093006_AlterFormSubmission')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240125093006_AlterFormSubmission', N'7.0.5');
END;
GO

COMMIT;
GO

