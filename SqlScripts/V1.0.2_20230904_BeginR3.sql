BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    DROP TABLE [Dossiers];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    DROP TABLE [Faqs];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [Translations] (
        [Id] int NOT NULL IDENTITY,
        [LanguageCode] nvarchar(max) NOT NULL,
        [Text] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Translations] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [Wizards] (
        [Id] int NOT NULL IDENTITY,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_Wizards] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [Question] (
        [Id] int NOT NULL IDENTITY,
        [WizardId] int NOT NULL,
        [IsFirstQuestion] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_Question] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Question_Wizards_WizardId] FOREIGN KEY ([WizardId]) REFERENCES [Wizards] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [TranslationWizard] (
        [IntroductionTextsId] int NOT NULL,
        [WizardsTextsId] int NOT NULL,
        CONSTRAINT [PK_TranslationWizard] PRIMARY KEY ([IntroductionTextsId], [WizardsTextsId]),
        CONSTRAINT [FK_TranslationWizard_Translations_IntroductionTextsId] FOREIGN KEY ([IntroductionTextsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TranslationWizard_Wizards_WizardsTextsId] FOREIGN KEY ([WizardsTextsId]) REFERENCES [Wizards] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [WizardTitlesTranslations] (
        [TitlesId] int NOT NULL,
        [WizardsTitlesId] int NOT NULL,
        CONSTRAINT [PK_WizardTitlesTranslations] PRIMARY KEY ([TitlesId], [WizardsTitlesId]),
        CONSTRAINT [FK_WizardTitlesTranslations_Translations_TitlesId] FOREIGN KEY ([TitlesId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_WizardTitlesTranslations_Wizards_WizardsTitlesId] FOREIGN KEY ([WizardsTitlesId]) REFERENCES [Wizards] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [Answers] (
        [Id] int NOT NULL IDENTITY,
        [QuestionId] int NOT NULL,
        [Order] int NOT NULL,
        [Tags] nvarchar(max) NOT NULL,
        [LinkedQuestionId] int NULL,
        [Link] nvarchar(max) NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorUserId] int NULL,
        [DeletedTime] datetime2 NULL,
        [DeleterUserId] int NULL,
        [LatestUpdateTime] datetime2 NULL,
        [LatestUpdateUserId] int NULL,
        CONSTRAINT [PK_Answers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Answers_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [QuestionBreadcrumb] (
        [Id] int NOT NULL IDENTITY,
        [QuestionId] int NOT NULL,
        CONSTRAINT [PK_QuestionBreadcrumb] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionBreadcrumb_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [QuestionsTextsTranslations] (
        [QuestionsTextsId] int NOT NULL,
        [TextsId] int NOT NULL,
        CONSTRAINT [PK_QuestionsTextsTranslations] PRIMARY KEY ([QuestionsTextsId], [TextsId]),
        CONSTRAINT [FK_QuestionsTextsTranslations_Question_QuestionsTextsId] FOREIGN KEY ([QuestionsTextsId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QuestionsTextsTranslations_Translations_TextsId] FOREIGN KEY ([TextsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [QuestionsTitlesTranslations] (
        [QuestionsTitlesId] int NOT NULL,
        [TitlesId] int NOT NULL,
        CONSTRAINT [PK_QuestionsTitlesTranslations] PRIMARY KEY ([QuestionsTitlesId], [TitlesId]),
        CONSTRAINT [FK_QuestionsTitlesTranslations_Question_QuestionsTitlesId] FOREIGN KEY ([QuestionsTitlesId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QuestionsTitlesTranslations_Translations_TitlesId] FOREIGN KEY ([TitlesId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [AnswersLabelsTranslations] (
        [AnswersLabelsId] int NOT NULL,
        [LabelsId] int NOT NULL,
        CONSTRAINT [PK_AnswersLabelsTranslations] PRIMARY KEY ([AnswersLabelsId], [LabelsId]),
        CONSTRAINT [FK_AnswersLabelsTranslations_Answers_AnswersLabelsId] FOREIGN KEY ([AnswersLabelsId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AnswersLabelsTranslations_Translations_LabelsId] FOREIGN KEY ([LabelsId]) REFERENCES [Translations] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE TABLE [QuestionBreadcrumbItem] (
        [Id] int NOT NULL IDENTITY,
        [BreadcrumbId] int NOT NULL,
        [QuestionId] int NOT NULL,
        [Order] int NOT NULL,
        [IsALoop] bit NOT NULL,
        CONSTRAINT [PK_QuestionBreadcrumbItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionBreadcrumbItem_QuestionBreadcrumb_BreadcrumbId] FOREIGN KEY ([BreadcrumbId]) REFERENCES [QuestionBreadcrumb] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QuestionBreadcrumbItem_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_Answers_QuestionId] ON [Answers] ([QuestionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_AnswersLabelsTranslations_LabelsId] ON [AnswersLabelsTranslations] ([LabelsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_Question_WizardId] ON [Question] ([WizardId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_QuestionBreadcrumb_QuestionId] ON [QuestionBreadcrumb] ([QuestionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_QuestionBreadcrumbItem_BreadcrumbId] ON [QuestionBreadcrumbItem] ([BreadcrumbId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_QuestionBreadcrumbItem_QuestionId] ON [QuestionBreadcrumbItem] ([QuestionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_QuestionsTextsTranslations_TextsId] ON [QuestionsTextsTranslations] ([TextsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_QuestionsTitlesTranslations_TitlesId] ON [QuestionsTitlesTranslations] ([TitlesId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_TranslationWizard_WizardsTextsId] ON [TranslationWizard] ([WizardsTextsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    CREATE INDEX [IX_WizardTitlesTranslations_WizardsTitlesId] ON [WizardTitlesTranslations] ([WizardsTitlesId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230919100719_Begin-R3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230919100719_Begin-R3', N'7.0.5');
END;
GO

COMMIT;
GO

