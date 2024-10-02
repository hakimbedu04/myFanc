BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240301112610_AlterFormValueFields')
BEGIN
	ALTER TABLE [dbo].[FormValueFields] ADD [Order] INT NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240301112610_AlterFormValueFields')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240301112610_AlterFormValueFields', N'7.0.5');
END;
GO

COMMIT;
GO