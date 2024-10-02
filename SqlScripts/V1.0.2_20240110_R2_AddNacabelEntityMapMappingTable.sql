BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240110170000_AddNacabelEntityMapMappingTable-R2')
BEGIN
    CREATE TABLE [NacabelsEntityMap] (
        [Id] int NOT NULL IDENTITY,
        [NacabelCode] nvarchar(max) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_NacabelsEntityMap] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240110170000_AddNacabelEntityMapMappingTable-R2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240110170000_AddNacabelEntityMapMappingTable-R2', N'7.0.5');
END;
GO

COMMIT;
GO

