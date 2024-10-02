BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240130151305_AlterNacabelsEntityMap')
BEGIN
	ALTER TABLE [dbo].[NacabelsEntityMap] DROP COLUMN [NacabelCode];
    ALTER TABLE [dbo].[NacabelsEntityMap] ADD [NacabelId] INT NOT NULL;
	ALTER TABLE [NacabelsEntityMap] ADD CONSTRAINT [FK_NacabelsEntityMap_Nacabels_NacabelId] FOREIGN KEY ([NacabelId]) REFERENCES [Nacabels] ([Id]);
END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240130151305_AlterNacabelsEntityMap')
BEGIN

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE());
DECLARE @NacabelId [INT] = SCOPE_IDENTITY();
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Transport', GETDATE());
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Transport', GETDATE());
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Transport', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId  = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'ContrôlePhysique', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'PhysicalControl', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'FysischeControle', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Journalist', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Journalist', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Journalist', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Radiophysique', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'RadioPhysics', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Stralingsfysica', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Vétérinaire', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Veterinarian', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Dierenarts', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Dentiste', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Dentist', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Tandarts', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Radiopharma', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Radiopharma', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'RadioFarma', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'NucMed', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'NucMed', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'NucGen', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'RadioThérapie', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'RadioTherapy', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'RadioTherapie', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Radiology', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Radiology', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Radiologie', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Médecin', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Medecin', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Geneeskunde', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'études', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Studies', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Studies', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'RadioDistribution', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'RadioDistribution', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'RadioDistributie', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Déchets', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Waste', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Afval', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Industriel', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Industrial', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Industrieel', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Class 1', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Class 1', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Klasse 1', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Sécurité/contrôle', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Security/Control', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Security/controle', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'FANC', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'FANC', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'FANC', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Dosimétrie', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Dosimetry', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'dosimetrie', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Formation', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Formation', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Vorming', GETDATE());

INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'RadioNaturelle', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'RadioNatural', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'RadioNatuur', GETDATE());
			
INSERT INTO "Nacabels" ("NacabelCode","CreationTime") VALUES('', GETDATE()); 
SET @NacabelId = SCOPE_IDENTITY();	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'fr', 'Aviation', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'en', 'Aviation', GETDATE());	
INSERT INTO "NacabelTranslation" ("NacabelId", "LanguageCode", "Description", "CreationTime") VALUES(@NacabelId,'nl', 'Luchtvaart', GETDATE());

END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240130151305_AlterNacabelsEntityMap')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240130151305_AlterNacabelsEntityMap', N'7.0.5');
END;
GO

COMMIT;
GO