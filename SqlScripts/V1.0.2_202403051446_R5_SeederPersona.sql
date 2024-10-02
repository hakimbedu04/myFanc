BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051446_R5_SeederPersona')
BEGIN

DECLARE @ParentCategoryId INT, @CategoryId INT, @CategoryLabelId INT, @NacabelId INT

-- USER
-- Transport
-- LEVEL 1 
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, [CreationTime]) VALUES (NULL, 1, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in transport')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans le transport')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in transport')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

-- LEVEL 2  
SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Transport' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','Driver')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Chauffeur')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Chauffeur')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- PhysicalControl
-- LEVEL 1
SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'PhysicalControl' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a physical control expert')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis un expert en contrôle physique')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een expert in fysische controle')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- Journalist
-- LEVEL 1
SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Journalist' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I use class I images')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','J''utilise des images de classe I')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik gebruik classe I afbeeldingen')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- Medical
-- LEVEL 1
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 1, NULL, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I practise a medical profession')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, [Text]) VALUES ('fr','J''excerce une profession médicale')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, [Text]) VALUES ('nl','Ik oefen een medisch beroep uit')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

-- LEVEL 2

SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'RadioPhysics' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am an expert in medical radiophysics')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis expert en radiophysique médicale')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een expert in medische stralingsfysica')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- VETERINARY
-- LEVEL 2

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, NULL, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a veterinary')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis vétérinaire')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een dierenarts')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

-- LEVEL 3

DECLARE @VetParentCategoryId INT 
SET @VetParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Veterinarian' AND LanguageCode = 'en'

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@VetParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a veterinary surgeon using nuclear medicine techniques')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis vétérinaire utilisant des techniques en médecine nucléaire')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een dierenarts die nucleaire geneeskunde technieken gebruikt')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@VetParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a veterinary surgeon using radiotherapy techniques')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis vétérinaire utilisant des techniques en radiothérapie')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een dierenarts die radiotherapietechnieken gebruikt')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Dentist' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a dental practitioner')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis praticien de l’art dentaire')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een tandarts')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Radiopharma' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a radiopharmacist')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis radiopharmacien')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een radiofarmaceut')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'NucMed' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a nuclear medicine practitioner')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis praticien en médecine nucléaire')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een nucleair geneeskundige')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'RadioTherapy' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a radiotherapy practitioner')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis praticien en radiothérapie')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een radiotherapeut')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Radiology' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a practitioner in radiological applications')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis praticien en applications radiologiques')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een beoefenaar in radiologische toepassingen')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Medecin' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 1, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a doctor')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis médecin du travail')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een dokter')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


------------------------------------------------------------------------------------------------------------------------------------------

-- COMPANY
-- MEDICAL
-- LEVEL 1

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, [CreationTime]) VALUES (NULL, 2, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am attached to a medical activity')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je suis rattaché à une activité medicale')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben verbonden aan een medische activiteit')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

-- LEVEL 2  
SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Dentist' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in dentistry')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaile en médecine dentaire')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de tandheelkunde')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Veterinarian' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in veterinary medicine')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille en médicine vétérinaire')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de diergeneeskunde')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Medecin' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in human medicine')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille en médicine humaine')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de menselijke geneeskunde')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Studies' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I''m doing clinical studies')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je fais des études clinique')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik doe klinische studies')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'RadioDistribution' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','Distribution of radioactive products for use in medicine')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Distribution de produits radioactifs pour usage dans la médicine')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Distributie van radioactieve producten voor medisch gebruik')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- INDUSTRY
-- LEVEL 1

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, [CreationTime]) VALUES (NULL, 2, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am attached to an industrial activity')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je suis rattaché à une activité industrielle')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben verbonden aan een industriële activiteit')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

-- LEVEL 2  
SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Waste' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in the waste and recycling sector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans le secteur des déchets et du recyclage')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de afval- en recyclingsector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Industrial' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in a class II or III establishment')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je travaille dans un établissement de classe II et III')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in een inrichting van klasse II of III')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in a Class IIA establishment')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans un établissement de classe IIA')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in een inrichting van klasse IIA')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- CLASS 1
-- LEVEL 2

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, NULL, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in a class I establishment')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans un établissement de classe I')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in een klasse I-inrichting')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

-- LEVEL 3

SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Class 1' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in a class I establishment')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans un établissement de classe I')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in een klasse I-inrichting')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I''m carrying out work on a class 1 installation')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je fais des travaux sur une installation de classe 1')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik voer werkzaamheden uit aan een klasse I installatie')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I use images of Class I installations in my work')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','J''utilise des images d''installations de classe I dans le cadre de mon travail')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik gebruik beelden van klasse I installaties in mijn werk')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- SECURITY
-- LEVEL 1

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Security/Control' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am attached to an approved organisation')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je suis rattaché à un organisme agréé')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben verbonden aan een erkende organisatie')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


------------------------------------------------------------------------------------------------------------------------------------------

-- TRANSPORT
-- LEVEL 1

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 2, NULL, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in transport')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans le transport')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in transport')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


-- LEVEL 2

SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Transport' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in the class 7 transport sector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans le secteur du transport de la classe 7')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de transportsector klasse 7')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in the import/transit/export of radioactive materials sector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans le secteur de l’importation/transit/exportation de matières radioactives')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de sector import/transit/export van radioactief materiaal')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work in the model parcels/packaging sector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je travaille dans le secteur des modèles de colis / emballages')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de modelpakketten/verpakkingssector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


------------------------------------------------------------------------------------------------------------------------------------------

-- FANC
-- LEVEL 1

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 2, NULL, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I have an in-house profession (FANC)')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','J''exerce une profession en interne (FANC)')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik heb een intern beroep (FANC)')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


-- LEVEL 2

SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'FANC' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am an administrator from the parameters')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je suis administrateur dès paramètres')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een administrator van de parameters')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','Ik zorg voor support')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je m''occupe de support')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk in de sector import/transit/export van radioactief materiaal')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


------------------------------------------------------------------------------------------------------------------------------------------

-- DOSIMETRY
-- LEVEL 1

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Dosimetry' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I work for a dosimetry service')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je travaille pour un service de dosimétrie')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik werk voor een dosimetriedienst')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


------------------------------------------------------------------------------------------------------------------------------------------

-- FORMATION
-- LEVEL 1

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Formation' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am a training organiser')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','je suis organisateur de formation')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben een opleidingsorganisator')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


------------------------------------------------------------------------------------------------------------------------------------------

-- RADIO NATURAL
-- LEVEL 1

INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 2, NULL, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','My case relates to natural radioactivity')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Mon dossier est lié à la radioactivité naturelle')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Mijn dossier heeft te maken met natuurlijke radioactiviteit')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


-- LEVEL 2

SET @ParentCategoryId = @CategoryId

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'RadioNatural' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','NORM')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','NORM')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','NORM')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)


INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (@ParentCategoryId, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','Radon')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Radon')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Radon')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

-- AVIATION
-- LEVEL 1

SELECT TOP 1 @NacabelId = NacabelId FROM NacabelTranslation WHERE Description = 'Aviation' AND LanguageCode = 'en'
INSERT INTO [dbo].[PersonaCategories] (ParentId, Type, NacabelId, [CreationTime]) VALUES (NULL, 2, @NacabelId, GETDATE())
SET @CategoryId = SCOPE_IDENTITY();

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('en','I am an attaché in the aviation sector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('fr','Je suis attaché dans le secteur aviation')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

INSERT INTO [dbo].[Translations] (LanguageCode, Text) VALUES ('nl','Ik ben attaché in de luchtvaartsector')
SET @CategoryLabelId = SCOPE_IDENTITY();
INSERT INTO [dbo].[PersonaCategoryLabelsTranslations] (PersonaCategoryId, LabelsId) VALUES (@CategoryId, @CategoryLabelId)

------------------------------------------------------------------------------------------------------------------------------------------

END;
GO


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'202403051446_R5_SeederPersona')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'202403051446_R5_SeederPersona', N'7.0.5');
END;
GO

COMMIT;
GO