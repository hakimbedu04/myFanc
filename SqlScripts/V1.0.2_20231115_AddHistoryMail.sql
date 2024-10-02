BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231115180160_AddHistoryMail-R2')
BEGIN
    CREATE TABLE [HistoryMails] (
        [Id] int NOT NULL IDENTITY,
        [MailTo] nvarchar(max) NOT NULL,
        [Attributes] nvarchar(max) NOT NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_HistoryMails] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231115180160_AddHistoryMail-R2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231115180160_AddHistoryMail-R2', N'7.0.5');
END;
GO

COMMIT;
GO

