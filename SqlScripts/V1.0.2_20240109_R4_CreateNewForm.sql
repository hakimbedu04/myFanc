BEGIN TRANSACTION;
GO
	
	IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202401091806_R4_CreateNewForm')
	BEGIN
	CREATE TABLE [FormNacabels] (
		[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormId] UNIQUEIDENTIFIER  NULL ,
		[NacabelsId] int  NULL,
		[CreationTime] datetime2 NOT NULL,
		[CreatorUserId] int NULL,
		[DeletedTime] datetime2 NULL,
		[DeleterUserId] int NULL,
		[LatestUpdateTime] datetime2 NULL,
		[LatestUpdateUserId] int NULL,
		CONSTRAINT [PK_FormNacabels] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormNacabels_Form_Id] FOREIGN KEY ([FormId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
	);
	END;

	IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202401091806_R4_CreateNewForm')
	BEGIN
	CREATE TABLE [FormUrls] (
		[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormId] UNIQUEIDENTIFIER  NULL ,
		[LanguageCode] varchar(10) NULL,
		[Url] varchar(max) NULL,
		[CreationTime] datetime2 NOT NULL,
		[CreatorUserId] int NULL,
		[DeletedTime] datetime2 NULL,
		[DeleterUserId] int NULL,
		[LatestUpdateTime] datetime2 NULL,
		[LatestUpdateUserId] int NULL,
		CONSTRAINT [PK_FormUrls] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormUrls_Forms_Id] FOREIGN KEY ([FormId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
	);
	END;

	IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202401091806_R4_CreateNewForm')
	BEGIN
	CREATE TABLE [FormDocuments] (
		[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormId] UNIQUEIDENTIFIER  NULL ,
		[LanguageCode] varchar(10) NULL,
		[DocumentId] UNIQUEIDENTIFIER NULL,
		[Type] varchar(20) NULL,
		[CreationTime] datetime2 NOT NULL,
		[CreatorUserId] int NULL,
		[DeletedTime] datetime2 NULL,
		[DeleterUserId] int NULL,
		[LatestUpdateTime] datetime2 NULL,
		[LatestUpdateUserId] int NULL,
		CONSTRAINT [PK_FormDocuments] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormDocuments_Forms_Id] FOREIGN KEY ([FormId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
	);
	END;

	IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202401091806_R4_CreateNewForm')
	BEGIN
	CREATE TABLE [FormSubmissions] (
		[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[UniqueId] UNIQUEIDENTIFIER  NULL ,
		[Version] int NULL,
		[IsDeleted] bit NULL,
		[Value] VARCHAR(max) NULL,
		[FancOrganisationId] UNIQUEIDENTIFIER NULL,
		[CompanyName] VARCHAR(200) NULL,
		[SubmissionDate] datetime2 NULL,
		[CompanyType] VARCHAR(200) NULL,
		[Email] VARCHAR(200) NULL,
		[CreationTime] datetime2 NOT NULL,
		[CreatorUserId] int NULL,
		[DeletedTime] datetime2 NULL,
		[DeleterUserId] int NULL,
		[LatestUpdateTime] datetime2 NULL,
		[LatestUpdateUserId] int NULL,
		CONSTRAINT [PK_FormDocuments] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormDocuments_Forms_Id] FOREIGN KEY ([FormId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE,
	);
	END;

COMMIT;
GO