BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240318123000_AlterFormNodeFieldAndForm')
BEGIN
	ALTER TABLE [dbo].[FormNodeFields] DROP COLUMN [Version];
    ALTER TABLE [dbo].[FormValueFields] DROP COLUMN [Version];
	ALTER TABLE [dbo].[Form] ADD CONSTRAINT [UNQ_Form_ExternalId_Version_Type] UNIQUE ([ExternalId], [Version], [Type]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240318123000_AlterFormNodeFieldAndForm')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240318123000_AlterFormNodeFieldAndForm', N'7.0.5');
END;
GO

COMMIT;
GO