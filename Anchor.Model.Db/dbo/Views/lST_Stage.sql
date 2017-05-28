CREATE VIEW [dbo].[lST_Stage]
WITH SCHEMABINDING
AS
SELECT [ST].ST_ID,
       [ST].Metadata_ST,
       [NAM].ST_NAM_ST_ID,
       [NAM].Metadata_ST_NAM,
       [NAM].ST_NAM_ChangedAt,
       [NAM].ST_NAM_Stage_Name,
       [LOC].ST_LOC_ST_ID,
       [LOC].Metadata_ST_LOC,
       [LOC].ST_LOC_Checksum,
       [LOC].ST_LOC_Stage_Location,
       [AVG].ST_AVG_ST_ID,
       [AVG].Metadata_ST_AVG,
       [AVG].ST_AVG_ChangedAt,
       [kAVG].UTL_Utilization AS ST_AVG_UTL_Utilization,
       [kAVG].Metadata_UTL AS ST_AVG_Metadata_UTL,
       [AVG].ST_AVG_UTL_ID,
       [MIN].ST_MIN_ST_ID,
       [MIN].Metadata_ST_MIN,
       [kMIN].UTL_Utilization AS ST_MIN_UTL_Utilization,
       [kMIN].Metadata_UTL AS ST_MIN_Metadata_UTL,
       [MIN].ST_MIN_UTL_ID
FROM   [dbo].[ST_Stage] AS [ST]
       LEFT OUTER JOIN
       [dbo].[ST_NAM_Stage_Name] AS [NAM]
       ON [NAM].ST_NAM_ST_ID = [ST].ST_ID
          AND [NAM].ST_NAM_ChangedAt = (SELECT max(sub.ST_NAM_ChangedAt)
                                        FROM   [dbo].[ST_NAM_Stage_Name] AS sub
                                        WHERE  sub.ST_NAM_ST_ID = [ST].ST_ID)
       LEFT OUTER JOIN
       [dbo].[ST_LOC_Stage_Location] AS [LOC]
       ON [LOC].ST_LOC_ST_ID = [ST].ST_ID
       LEFT OUTER JOIN
       [dbo].[ST_AVG_Stage_Average] AS [AVG]
       ON [AVG].ST_AVG_ST_ID = [ST].ST_ID
          AND [AVG].ST_AVG_ChangedAt = (SELECT max(sub.ST_AVG_ChangedAt)
                                        FROM   [dbo].[ST_AVG_Stage_Average] AS sub
                                        WHERE  sub.ST_AVG_ST_ID = [ST].ST_ID)
       LEFT OUTER JOIN
       [dbo].[UTL_Utilization] AS [kAVG]
       ON [kAVG].UTL_ID = [AVG].ST_AVG_UTL_ID
       LEFT OUTER JOIN
       [dbo].[ST_MIN_Stage_Minimum] AS [MIN]
       ON [MIN].ST_MIN_ST_ID = [ST].ST_ID
       LEFT OUTER JOIN
       [dbo].[UTL_Utilization] AS [kMIN]
       ON [kMIN].UTL_ID = [MIN].ST_MIN_UTL_ID;