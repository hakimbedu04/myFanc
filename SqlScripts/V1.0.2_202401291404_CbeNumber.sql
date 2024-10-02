IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'CbeNumber'
          AND Object_ID = Object_ID(N'NacabelsEntityMap'))
BEGIN
    ALTER TABLE dbo.NacabelsEntityMap ADD
	CbeNumber nvarchar(MAX) NULL
END
