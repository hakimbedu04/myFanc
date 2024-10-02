BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311171710_Roles')
BEGIN
-- craete table Roles
CREATE TABLE [Roles] (
		[Id] int NOT NULL IDENTITY,
		[ExternalRole] varchar(50),
		[InternalRole] varchar(50),
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311171710_Roles')
BEGIN
-- create table UserRoles
CREATE TABLE [UserRoles] (
		[Id] int NOT NULL IDENTITY,
		[UserId] int NOT NULL,
		[InternalRole] varchar(50),
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([Id]),
		CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])

);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311171710_Roles')
BEGIN
	INSERT INTO Roles (ExternalRole, InternalRole, CreationTime) VALUES ('MyFANC_Manager','Manager', GETDATE());

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311171710_Roles')
BEGIN
	INSERT INTO UserRoles (UserId, InternalRole, CreationTime) VALUES ((SELECT TOP 1 Id FROM Users),'Admin', GETDATE());

END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202311171710_Roles')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'202311171710_Roles', N'7.0.5');
END;
GO

COMMIT;
GO

