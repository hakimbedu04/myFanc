BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE TABLE [PersonaCategories] (
		[Id] int NOT NULL IDENTITY,
		[ParentId] int NULL,
		[Type] int NULL,
		[NacabelId] int NULL,
		[CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
		CONSTRAINT [PK_PersonaCategories] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_PersonaCategories_PersonaCategories_ParentId] FOREIGN KEY (ParentId) REFERENCES PersonaCategories(Id),
		CONSTRAINT [FK_PersonaCategories_Nacabels_NacabelId] FOREIGN KEY (NacabelId) REFERENCES Nacabels(Id)
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE TABLE [PersonaCategoryLabelsTranslations] (
		[PersonaCategoryId] int NOT NULL,
		[LabelsId] int NOT NULL,
        CONSTRAINT [PK_PersonaCategoryLabelsTranslations] PRIMARY KEY ([PersonaCategoryId], [LabelsId]),
        CONSTRAINT [FK_PersonaCategoryLabelsTranslations_PersonaCategories_PersonaCategoryId] FOREIGN KEY ([PersonaCategoryId]) REFERENCES [PersonaCategories] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PersonaCategoryLabelsTranslations_Translations_LabelsId] FOREIGN KEY ([LabelsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE TABLE [CompanyPersonas] (
		[Id] int NOT NULL IDENTITY,
		[OrganisationFancId] uniqueidentifier NOT NULL,
		[NacabelCode] nvarchar(max) NULL,
		[CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
		CONSTRAINT [PK_CompanyPersonas] PRIMARY KEY ([Id]),
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE TABLE [CompanyPersonaCategories] (
		[CompanyPersonaId] int NOT NULL,
		[PersonaCategoryId] int NOT NULL,
        CONSTRAINT [PK_CompanyPersonaCategories] PRIMARY KEY ([PersonaCategoryId], [CompanyPersonaId]),
        CONSTRAINT [FK_CompanyPersonaCategories_PersonaCategories_PersonaCategoryId] FOREIGN KEY ([PersonaCategoryId]) REFERENCES [PersonaCategories] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CompanyPersonaCategories_CompanyPersonas_CompanyPersonaId] FOREIGN KEY ([CompanyPersonaId]) REFERENCES [CompanyPersonas] ([Id]) ON DELETE CASCADE
    );
END;
GO



IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE TABLE [UserPersonas] (
		[Id] int NOT NULL IDENTITY,
		[UserId] int NOT NULL,
		[INAMINumber] varchar(255),
		[CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
		CONSTRAINT [PK_UserPersonas] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_UserPersonas_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE TABLE [UserPersonaCategories] (
		[UserPersonaId] int NOT NULL,
		[PersonaCategoryId] int NOT NULL,
        CONSTRAINT [PK_UserPersonaCategories] PRIMARY KEY ([PersonaCategoryId], [UserPersonaId]),
        CONSTRAINT [FK_UserPersonaCategories_PersonaCategories_PersonaCategoryId] FOREIGN KEY ([PersonaCategoryId]) REFERENCES [PersonaCategories] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPersonaCategories_UserPersonas_UserPersonaId] FOREIGN KEY ([UserPersonaId]) REFERENCES [UserPersonas] ([Id]) ON DELETE CASCADE
    );
END;
GO

--PersonaCategories
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_PersonaCategories_ParentId] ON [PersonaCategories] ([ParentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_PersonaCategories_NacabelId] ON [PersonaCategories] ([NacabelId]);
END;
GO

--PersonaCategoryLabelsTranslations
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_PersonaCategoryLabelsTranslations_LabelId] ON [PersonaCategoryLabelsTranslations] ([LabelsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_PersonaCategoryLabelsTranslations_PersonaCategoryId] ON [PersonaCategoryLabelsTranslations] ([PersonaCategoryId]);
END;
GO


-- CompanyPersonaCategories
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_CompanyPersonaCategories_CompanyPersonaId] ON [CompanyPersonaCategories] ([CompanyPersonaId]);
END;
GO
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_CompanyPersonaCategories_PersonaCategoryId] ON [CompanyPersonaCategories] ([PersonaCategoryId]);
END;
GO

--UserPersonas
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_UserPersonas_UserId] ON [UserPersonas] ([UserId]);
END;
GO

-- UserPersonaCategories
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_UserPersonaCategories_UserPersonaId] ON [UserPersonaCategories] ([UserPersonaId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    CREATE INDEX [IX_UserPersonaCategories_PersonaCategoryId] ON [UserPersonaCategories] ([PersonaCategoryId]);
END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051146_R5_DefinePersona')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'202403051146_R5_DefinePersona', N'7.0.5');
END;
GO


COMMIT;
GO
