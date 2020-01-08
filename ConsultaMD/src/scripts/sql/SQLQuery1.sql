--DECLARE @TableName NVARCHAR(50)='dbo.Localities'
--DECLARE @Tsv NVARCHAR(100)='E:\\WebProjects\\ConsultaMD\\ConsultaMD\\Data\\Locality\\Localities.tsv'
--DECLARE @SQLSelectQuery NVARCHAR(MAX)=''
--SET @SQLSelectQuery = 'BULK INSERT ' + @TableName + ' FROM ' + QUOTENAME(@Tsv) + ' WITH (DATAFILETYPE=''widechar'')'
--PRINT @SQLSelectQuery
--exec(@SQLSelectQuery)

--BULK INSERT dbo.Localities FROM 'E:\\WebProjects\\ConsultaMD\\ConsultaMD\\Data\\Locality\\Localities.tsv' WITH (DATAFILETYPE='widechar')

--DELETE FROM dbo.Localities
--select * from sysobjects where type='P' and name='BulkInsert'

--DROP PROCEDURE BulkInsert
--GO

--BulkInsert 'dbo.Localities', 'E:\\WebProjects\\ConsultaMD\\ConsultaMD\\Data\\Locality\\Localities.tsv'

--CREATE PROCEDURE BulkInsert(@TableName NVARCHAR(50), @Tsv NVARCHAR(100))
--AS
--BEGIN 
--DECLARE @SQLSelectQuery NVARCHAR(MAX)=''
--DECLARE @widechar NVARCHAR(MAX)=''
--SET @SQLSelectQuery = 'BULK INSERT ' + @TableName + ' FROM ' + QUOTENAME(@Tsv) + ' WITH (DATAFILETYPE=''widechar'')'
--exec(@SQLSelectQuery)
--END

--EXEC BulkInsert @TableName = 'dbo.Localities', @Tsv = 'E:\\WebProjects\\ConsultaMD\\ConsultaMD\\Data\\Locality\\Localities.tsv'
