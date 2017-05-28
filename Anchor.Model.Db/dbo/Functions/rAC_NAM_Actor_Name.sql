CREATE FUNCTION [dbo].[rAC_NAM_Actor_Name]
(@changingTimepoint DATETIME)
RETURNS TABLE 
WITH SCHEMABINDING
AS
RETURN 
    SELECT Metadata_AC_NAM,
           AC_NAM_AC_ID,
           AC_NAM_Actor_Name,
           AC_NAM_ChangedAt
    FROM   [dbo].[AC_NAM_Actor_Name]
    WHERE  AC_NAM_ChangedAt <= @changingTimepoint