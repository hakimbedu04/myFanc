BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231212120810_AddNacabelMappingTable-R2')
BEGIN
    CREATE TABLE [Nacabels] (
        [Id] int NOT NULL IDENTITY,
        [NacabelCode] nvarchar(max) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_Nacabels] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231212120810_AddNacabelMappingTable-R2')
BEGIN
    CREATE TABLE [NacabelTranslation] (
        [Id] int NOT NULL IDENTITY,
        [NacabelId] int NOT NULL,
        [LanguageCode] nvarchar(max) NULL,
	[Description] nvarchar(max) NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_NacabelTranslation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NacabelTranslation_Nacabels_NacabelId] FOREIGN KEY ([NacabelId]) REFERENCES [Nacabels] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231212120810_AddNacabelMappingTable-R2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231212120810_AddNacabelMappingTable-R2', N'7.0.5');
END;
GO

COMMIT;
GO

