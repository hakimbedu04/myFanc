BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    CREATE TABLE [Form] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[ExternalId] int NOT NULL,
        [Version] int NOT NULL,
        [IsActive] bit NOT NULL,
		[Type] nvarchar(50) NOT NULL,
		[Status] int NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormBlock] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    CREATE TABLE [FormsLabelsTranslations] (
        [FormsLabelsId] uniqueidentifier NOT NULL,
        [LabelsId] int NOT NULL,
        CONSTRAINT [PK_FormsLabelsTranslations] PRIMARY KEY ([FormsLabelsId], [LabelsId]),
        CONSTRAINT [FK_FormsLabelsTranslations_Form_FormsLabelsId] FOREIGN KEY ([FormsLabelsId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormsLabelsTranslations_Translations_LabelsId] FOREIGN KEY ([LabelsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    CREATE TABLE [FormsDescriptionsTranslations] (
        [FormsDescriptionsId] uniqueidentifier NOT NULL,
        [DescriptionsId] int NOT NULL,
        CONSTRAINT [PK_FormsDescriptionsTranslations] PRIMARY KEY ([FormsDescriptionsId], [DescriptionsId]),
        CONSTRAINT [FK_FormsDescriptionsTranslations_Form_FormsDescriptionsId] FOREIGN KEY ([FormsDescriptionsId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormsDescriptionsTranslations_Translations_DescriptionsId] FOREIGN KEY ([DescriptionsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    CREATE TABLE [FormsUrlsTranslations] (
        [FormsUrlsId] uniqueidentifier NOT NULL,
        [UrlsId] int NOT NULL,
        CONSTRAINT [PK_FormsUrlsTranslations] PRIMARY KEY ([FormsUrlsId], [UrlsId]),
        CONSTRAINT [FK_FormsUrlsTranslations_Form_FormsUrlsId] FOREIGN KEY ([FormsUrlsId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormsUrlsTranslations_Translations_UrlsId] FOREIGN KEY ([UrlsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    CREATE INDEX [IX_FormsLabelsTranslations_LabelsId] ON [FormsLabelsTranslations] ([LabelsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    CREATE INDEX [IX_FormsDescriptionsTranslations_DescriptionsId] ON [FormsDescriptionsTranslations] ([DescriptionsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    CREATE INDEX [IX_FormsUrlsTranslations_UrlsId] ON [FormsUrlsTranslations] ([UrlsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231228175619_R4_FormBlock')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231228175619_R4_FormBlock', N'7.0.5');
END;
GO

COMMIT;
GO