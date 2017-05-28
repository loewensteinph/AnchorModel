CREATE FUNCTION [dbo].[rST_AVG_Stage_Average]
(@changingTimepoint DATETIME)
RETURNS TABLE 
WITH SCHEMABINDING
AS
RETURN 
    SELECT Metadata_ST_AVG,
           ST_AVG_ST_ID,
           ST_AVG_UTL_ID,
           ST_AVG_ChangedAt
    FROM   [dbo].[ST_AVG_Stage_Average]
    WHERE  ST_AVG_ChangedAt <= @changingTimepoint