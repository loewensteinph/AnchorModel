CREATE FUNCTION [dbo].[pPE_Performance]
(@changingTimepoint DATETIME2 (7))
RETURNS TABLE 
WITH SCHEMABINDING
AS
RETURN 
    SELECT [PE].PE_ID,
           [PE].Metadata_PE,
           [DAT].PE_DAT_PE_ID,
           [DAT].Metadata_PE_DAT,
           [DAT].PE_DAT_Performance_Date,
           [AUD].PE_AUD_PE_ID,
           [AUD].Metadata_PE_AUD,
           [AUD].PE_AUD_Performance_Audience,
           [REV].PE_REV_PE_ID,
           [REV].Metadata_PE_REV,
           [REV].PE_REV_ChangedAt,
           [REV].PE_REV_Checksum,
           [REV].PE_REV_Performance_Revenue
    FROM   [dbo].[PE_Performance] AS [PE]
           LEFT OUTER JOIN
           [dbo].[PE_DAT_Performance_Date] AS [DAT]
           ON [DAT].PE_DAT_PE_ID = [PE].PE_ID
           LEFT OUTER JOIN
           [dbo].[PE_AUD_Performance_Audience] AS [AUD]
           ON [AUD].PE_AUD_PE_ID = [PE].PE_ID
           LEFT OUTER JOIN
           [dbo].[rPE_REV_Performance_Revenue](@changingTimepoint) AS [REV]
           ON [REV].PE_REV_PE_ID = [PE].PE_ID
              AND [REV].PE_REV_ChangedAt = (SELECT max(sub.PE_REV_ChangedAt)
                                            FROM   [dbo].[rPE_REV_Performance_Revenue](@changingTimepoint) AS sub
                                            WHERE  sub.PE_REV_PE_ID = [PE].PE_ID)