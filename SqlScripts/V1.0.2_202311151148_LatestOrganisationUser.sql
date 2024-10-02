BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311151148_LatestOrganisationUser')
BEGIN
    ALTER TABLE Users ADD LatestOrganisation uniqueidentifier null
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311151148_LatestOrganisationUser')
BEGIN
    ALTER TABLE Users ADD LatestEstablishment uniqueidentifier null
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311151148_LatestOrganisationUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'202311151148_LatestOrganisationUser', N'7.0.5');
END;
GO

COMMIT;
GO