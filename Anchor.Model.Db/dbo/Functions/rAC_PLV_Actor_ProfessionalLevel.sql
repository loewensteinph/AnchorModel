CREATE FUNCTION [dbo].[rAC_PLV_Actor_ProfessionalLevel]
(@changingTimepoint DATETIME)
RETURNS TABLE 
WITH SCHEMABINDING
AS
RETURN 
    SELECT Metadata_AC_PLV,
           AC_PLV_AC_ID,
           AC_PLV_PLV_ID,
           AC_PLV_ChangedAt
    FROM   [dbo].[AC_PLV_Actor_ProfessionalLevel]
    WHERE  AC_PLV_ChangedAt <= @changingTimepoint