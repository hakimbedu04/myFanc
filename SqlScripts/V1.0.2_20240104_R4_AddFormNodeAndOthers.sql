BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormNodes] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[IsActive] bit NOT NULL,
        [Version] int NOT NULL,
        [FormId] UNIQUEIDENTIFIER NOT NULL,
		[ParentId] UNIQUEIDENTIFIER NULL,
		[Type] nvarchar(50) NOT NULL,
		[FieldType] nvarchar(50) NULL,
		[Order] int NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormNodes] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormNodes_Form_FormId] FOREIGN KEY ([FormId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
		CONSTRAINT [FK_FormNodes_FormNodes_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [FormNodes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormNodesLabelsTranslations] (
        [FormNodesLabelsId] uniqueidentifier NOT NULL,
        [LabelsId] int NOT NULL,
        CONSTRAINT [PK_FormNodesLabelsTranslations] PRIMARY KEY ([FormNodesLabelsId], [LabelsId]),
        CONSTRAINT [FK_FormNodesLabelsTranslations_FormNodes_FormNodesLabelsId] FOREIGN KEY ([FormNodesLabelsId]) REFERENCES [FormNodes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormNodesLabelsTranslations_Translations_LabelsId] FOREIGN KEY ([LabelsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormNodeFields] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormNodeId] UNIQUEIDENTIFIER NOT NULL,
		[Property] nvarchar(MAX) NOT NULL,
		[Version] int NOT NULL,
		[Value] nvarchar(MAX) NOT NULL,
		[Type] nvarchar(50) NOT NULL,
		[CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormNodeFields] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormNodeFields_FormNodes_FormNodeId] FOREIGN KEY ([FormNodeId]) REFERENCES [FormNodes] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormNodeFieldsLabelsTranslations] (
        [FormNodeFieldsLabelsId] uniqueidentifier NOT NULL,
        [LabelsId] int NOT NULL,
        CONSTRAINT [PK_FormNodeFieldsLabelsTranslations] PRIMARY KEY ([FormNodeFieldsLabelsId], [LabelsId]),
        CONSTRAINT [FK_FormNodeFieldsLabelsTranslations_FormNodeFields_FormNodeFieldsLabelsId] FOREIGN KEY ([FormNodeFieldsLabelsId]) REFERENCES [FormNodeFields] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormNodeFieldsLabelsTranslations_Translations_LabelsId] FOREIGN KEY ([LabelsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormValueFields] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormNodeFieldId] UNIQUEIDENTIFIER NOT NULL,
		[Version] int NOT NULL,
		[Value] nvarchar(MAX) NOT NULL,
		[CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormValueFields] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormValueFields_FormNodeFields_FormNodeFieldId] FOREIGN KEY ([FormNodeFieldId]) REFERENCES [FormNodeFields] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormValueFieldsLabelsTranslations] (
        [FormValueFieldsLabelsId] uniqueidentifier NOT NULL,
        [LabelsId] int NOT NULL,
        CONSTRAINT [PK_FormValueFieldsLabelsTranslations] PRIMARY KEY ([FormValueFieldsLabelsId], [LabelsId]),
        CONSTRAINT [FK_FormValueFieldsLabelsTranslations_FormValueFields_FormValueFieldsLabelsId] FOREIGN KEY ([FormValueFieldsLabelsId]) REFERENCES [FormValueFields] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormValueFieldsLabelsTranslations_Translations_LabelsId] FOREIGN KEY ([LabelsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormConditionals] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormNodeFieldId] UNIQUEIDENTIFIER NOT NULL,
		[FormNodeId] UNIQUEIDENTIFIER NOT NULL,
		[Condition] nvarchar(MAX) NOT NULL,
		[State] nvarchar(50) NOT NULL,
		[CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormConditionals] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormConditionals_FormNodeFields_FormNodeFieldId] FOREIGN KEY ([FormNodeFieldId]) REFERENCES [FormNodeFields] ([Id]) ON DELETE CASCADE,
		CONSTRAINT [FK_FormConditionals_FormNodes_FormNodeId] FOREIGN KEY ([FormNodeId]) REFERENCES [FormNodes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE TABLE [FormDataSources] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[Name] nvarchar(MAX) NOT NULL,
		[Version] int NOT NULL,
		[CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormDataSources] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE INDEX [IX_FormNodesLabelsTranslations_LabelsId] ON [FormNodesLabelsTranslations] ([LabelsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE INDEX [IX_FormNodeFieldsLabelsTranslations_LabelsId] ON [FormNodeFieldsLabelsTranslations] ([LabelsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    CREATE INDEX [IX_FormValueFieldsLabelsTranslations_LabelsId] ON [FormValueFieldsLabelsTranslations] ([LabelsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240104045120_R4_FormNodes')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240104045120_R4_FormNodes', N'7.0.5');
END;
GO

COMMIT;
GO