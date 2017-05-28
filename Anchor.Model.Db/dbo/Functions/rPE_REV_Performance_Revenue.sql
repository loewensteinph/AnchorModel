CREATE FUNCTION [dbo].[rPE_REV_Performance_Revenue]
(@changingTimepoint DATETIME)
RETURNS TABLE 
WITH SCHEMABINDING
AS
RETURN 
    SELECT Metadata_PE_REV,
           PE_REV_PE_ID,
           PE_REV_Checksum,
           PE_REV_Performance_Revenue,
           PE_REV_ChangedAt
    FROM   [dbo].[PE_REV_Performance_Revenue]
    WHERE  PE_REV_ChangedAt <= @changingTimepoint