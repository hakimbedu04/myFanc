BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Claims');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Users] DROP COLUMN [Claims];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    EXEC sp_rename N'[Users].[LastestUpdateUserId]', N'LatestUpdateUserId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    EXEC sp_rename N'[Users].[LastestUpdateTime]', N'LatestUpdateTime', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    EXEC sp_rename N'[Users].[LastSychData]', N'LatestSynchronization', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    EXEC sp_rename N'[Users].[LastConnected]', N'LatestConnection', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    EXEC sp_rename N'[Dossiers].[LastestUpdateUserId]', N'LatestUpdateUserId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    EXEC sp_rename N'[Dossiers].[LastestUpdateTime]', N'LatestUpdateTime', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230823142202_ReviewR1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230823142202_ReviewR1', N'7.0.5');
END;
GO

COMMIT;
GO

