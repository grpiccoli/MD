CREATE PROCEDURE bulkInsert(@TableName NVARCHAR(50), @tsv NVARCHAR(100))
AS
BEGIN 
DECLARE @SQLSelectQuery NVARCHAR(MAX)=''
SET @SQLSelectQuery = 'BULK INSERT ' + @TableName + ' FROM ' + QUOTENAME(@tsv) + ' WITH (DATAFILETYPE='+QUOTENAME('widechar')+')'
  exec(@SQLSelectQuery)
END