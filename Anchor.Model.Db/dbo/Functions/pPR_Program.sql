CREATE FUNCTION [dbo].[pPR_Program]
(@changingTimepoint DATETIME2 (7))
RETURNS TABLE 
WITH SCHEMABINDING
AS
RETURN 
    SELECT [PR].PR_ID,
           [PR].Metadata_PR,
           [NAM].PR_NAM_PR_ID,
           [NAM].Metadata_PR_NAM,
           [NAM].PR_NAM_Program_Name
    FROM   [dbo].[PR_Program] AS [PR]
           LEFT OUTER JOIN
           [dbo].[PR_NAM_Program_Name] AS [NAM]
           ON [NAM].PR_NAM_PR_ID = [PR].PR_ID