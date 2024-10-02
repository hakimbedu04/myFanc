BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240119103020_R4_FormSubmission')
BEGIN
    CREATE TABLE [FormSubmissions] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
		[FormId] UNIQUEIDENTIFIER NOT NULL,
        [Version] int NOT NULL,
        [Value] NVARCHAR(MAX) NOT NULL,
		[FancOrganisationId] NVARCHAR(50) NULL,
		[CompanyName] NVARCHAR(100) NOT NULL,
		[SubmissionDate] datetime2 NOT NULL,
		[CompanyType] NVARCHAR(5) NULL,
		[Email] NVARCHAR(50) NOT NULL,
		[UserId] NVARCHAR(50) NULL,
		[UserName] NVARCHAR(100) NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_FormSubmissions] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_FormSubmissions_Form_FormId] FOREIGN KEY ([FormId]) REFERENCES [Form] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240119103020_R4_FormSubmission')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240119103020_R4_FormSubmission', N'7.0.5');
END;
GO

COMMIT;
GO