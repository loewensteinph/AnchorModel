CREATE FUNCTION [dbo].[rST_NAM_Stage_Name]
(@changingTimepoint DATETIME)
RETURNS TABLE 
WITH SCHEMABINDING
AS
RETURN 
    SELECT Metadata_ST_NAM,
           ST_NAM_ST_ID,
           ST_NAM_Stage_Name,
           ST_NAM_ChangedAt
    FROM   [dbo].[ST_NAM_Stage_Name]
    WHERE  ST_NAM_ChangedAt <= @changingTimepoint