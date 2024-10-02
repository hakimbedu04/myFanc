BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    CREATE TABLE [FormCategory] (
        [Id] int NOT NULL IDENTITY,
		[Code] NVARCHAR(50) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormCategory] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    ALTER TABLE [Form] ADD [FormCategoryId] INT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    ALTER TABLE [Form] ADD CONSTRAINT [FK_Form_FormCategory_FormCategoryId] FOREIGN KEY ([FormCategoryId]) REFERENCES [FormCategory] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    CREATE TABLE [FormDocuments] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormId] UNIQUEIDENTIFIER NOT NULL,
		[LanguageCode] nvarchar(5) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormDocuments] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormDocuments_Form_FormId] FOREIGN KEY ([FormId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    CREATE TABLE [Documents] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormDocumentId] UNIQUEIDENTIFIER NOT NULL,
		[Type] NVARCHAR(50) NOT NULL,
		[Name] NVARCHAR(100) NOT NULL,
		[Path] NVARCHAR(MAX) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_Documents] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_Documents_FormDocuments_FormDocumentId] FOREIGN KEY ([FormDocumentId]) REFERENCES [FormDocuments] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    CREATE TABLE [FormsNacabels] (
        [FormsId] UNIQUEIDENTIFIER NOT NULL,
        [NacabelsId] INT NOT NULL,
        CONSTRAINT [PK_FormsNacabels] PRIMARY KEY ([FormsId], [NacabelsId]),
        CONSTRAINT [FK_FormsNacabels_Forms_FormsId] FOREIGN KEY ([FormsId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_FormsNacabels_Nacabels_NacabelsId] FOREIGN KEY ([NacabelsId]) REFERENCES [Nacabels] ([Id]) ON DELETE CASCADE
    );
END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    CREATE INDEX [IX_FormsNacabels_NacabelsId] ON [FormsNacabels] ([NacabelsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN

INSERT INTO "FormCategory" ("Code","CreationTime") VALUES('IAII', GETDATE());
INSERT INTO "FormCategory" ("Code","CreationTime") VALUES('GLMI', GETDATE());
INSERT INTO "FormCategory" ("Code","CreationTime") VALUES('GLBEG', GETDATE());
INSERT INTO "FormCategory" ("Code","CreationTime") VALUES('BVNB', GETDATE());
INSERT INTO "FormCategory" ("Code","CreationTime") VALUES('BVVER', GETDATE());
INSERT INTO "FormCategory" ("Code","CreationTime") VALUES('General', GETDATE());

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240116185520_R4_FormDocument')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240116185520_R4_FormDocument', N'7.0.5');
END;
GO

COMMIT;
GO