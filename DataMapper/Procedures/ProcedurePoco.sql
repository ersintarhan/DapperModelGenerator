
CREATE PROCEDURE [dbo].[Helper_CreatePocoFromProcName]    
    @procname varchar(100)
AS
BEGIN
SET NOCOUNT ON;

-- Subquery to return only the copy paste text
Select PropertyColumn from (
    SELECT 1 as rowNr, 'public class ' + @procname + ' {' as PropertyColumn
    UNION
    SELECT 2 as rowNr, 'public ' + a1.NewType + ' ' + a1.name + ' {get;set;}' as PropertyColumn
    -- ,* comment added so that i get copy pasteable output
     FROM 
    (
        /*using top because i'm putting an order by ordinal_position on it. 
        putting a top on it is the only way for a subquery to be ordered*/
        SELECT TOP 100 PERCENT
        name,
        system_type_name,
        is_nullable,
        CASE 
            WHEN system_type_name LIKE 'varchar%' THEN 'string'
            WHEN system_type_name LIKE 'nvarchar%' THEN 'string' 
            WHEN system_type_name = 'datetime' AND IS_NULLABLE = 0 THEN 'DateTime'
            WHEN system_type_name = 'datetime' AND IS_NULLABLE = 1 THEN 'DateTime?'
			WHEN system_type_name LIKE 'datetime%' AND IS_NULLABLE = 0 THEN 'DateTime'
            WHEN system_type_name LIKE 'datetime%' AND IS_NULLABLE = 1 THEN 'DateTime?'
            WHEN system_type_name = 'smalldatetime' AND IS_NULLABLE = 0 THEN 'DateTime'
            WHEN system_type_name = 'datetime2' AND IS_NULLABLE = 0 THEN 'DateTime'
            WHEN system_type_name = 'smalldatetime' AND IS_NULLABLE = 1 THEN 'DateTime?'
            WHEN system_type_name = 'datetime2' AND IS_NULLABLE = 1 THEN 'DateTime?'
            WHEN system_type_name = 'int' AND IS_NULLABLE = 1 THEN 'int?'
            WHEN system_type_name = 'int' AND IS_NULLABLE = 0 THEN 'int'
            WHEN system_type_name = 'smallint' AND IS_NULLABLE = 0 THEN 'Int16'
            WHEN system_type_name = 'smallint' AND IS_NULLABLE = 1 THEN 'Int16?'
            WHEN system_type_name = 'decimal' AND IS_NULLABLE = 0 THEN 'decimal'
            WHEN system_type_name = 'decimal' AND IS_NULLABLE = 1 THEN 'decimal?'
            WHEN system_type_name = 'numeric' AND IS_NULLABLE = 0 THEN 'decimal'
            WHEN system_type_name = 'numeric' AND IS_NULLABLE = 1 THEN 'decimal?'
            WHEN system_type_name = 'money' AND IS_NULLABLE = 0 THEN 'decimal'
            WHEN system_type_name = 'money' AND IS_NULLABLE = 1 THEN 'decimal?'
            WHEN system_type_name = 'bigint' AND IS_NULLABLE = 0 THEN 'long'
            WHEN system_type_name = 'bigint' AND IS_NULLABLE = 1 THEN 'long?'
            WHEN system_type_name = 'tinyint' AND IS_NULLABLE = 0 THEN 'byte'
            WHEN system_type_name = 'tinyint' AND IS_NULLABLE = 1 THEN 'byte?'
			WHEN system_type_name LIKE 'char%' THEN 'string'                       
            WHEN system_type_name = 'timestamp' THEN 'byte[]'
            WHEN system_type_name = 'varbinary' THEN 'byte[]'
            WHEN system_type_name = 'bit' AND IS_NULLABLE = 0 THEN 'bool'
            WHEN system_type_name = 'bit' AND IS_NULLABLE = 1 THEN 'bool?'
			WHEN system_type_name = 'float' AND IS_NULLABLE = 0 THEN 'double'
            WHEN system_type_name = 'float' AND IS_NULLABLE = 1 THEN 'double?'
            WHEN system_type_name = 'xml' THEN 'string'
        END AS NewType
       
FROM sys.dm_exec_describe_first_result_set_for_object
(
  OBJECT_ID(@procname),
  NULL
) ORDER BY column_ordinal
        ) AS a1 
    UNION 
    SELECT 3 as rowNr,  '} // class ' + @procname
    ) as t 
END

GO

