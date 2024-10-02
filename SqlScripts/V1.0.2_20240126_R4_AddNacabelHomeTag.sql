BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240126131415_R4_AddHomeTag')
BEGIN

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('Home', GETDATE());

DECLARE @NacabelId [INT] = SCOPE_IDENTITY()

INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId, 'en', 'Home', GETDATE());
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId, 'fr', 'Maison', GETDATE());
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId, 'nl', 'Thuis', GETDATE());

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240126131415_R4_AddHomeTag')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240126131415_R4_AddHomeTag', N'7.0.5');
END;
GO

COMMIT;
GO