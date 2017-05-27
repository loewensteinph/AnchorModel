CREATE VIEW [dbo].[lST_Stage] WITH SCHEMABINDING AS SELECT [ST].ST_ID,[ST].Metadata_ST,[NAM].ST_NAM_ST_ID,[NAM].Metadata_ST_NAM,[NAM].ST_NAM_ChangedAt,[NAM].ST_NAM_Stage_Name,  [LOC].ST_LOC_ST_ID,[LOC].Metadata_ST_LOC,[LOC].ST_LOC_Checksum,[LOC].ST_LOC_Stage_Location,  [AVG].ST_AVG_ST_ID,[AVG].Metadata_ST_AVG,[AVG].ST_AVG_ChangedAt,[kAVG].UTL_Utilization AS ST_AVG_UTL_Utilization,[kAVG].Metadata_UTL AS ST_AVG_Metadata_UTL,[AVG].ST_AVG_UTL_ID , [MIN].ST_MIN_ST_ID,[MIN].Metadata_ST_MIN,[kMIN].UTL_Utilization AS ST_MIN_UTL_Utilization,[kMIN].Metadata_UTL AS ST_MIN_Metadata_UTL,[MIN].ST_MIN_UTL_ID FROM [dbo].[ST_Stage] [ST] LEFT JOIN [dbo].[ST_NAM_Stage_Name] [NAM] ON [NAM].ST_NAM_ST_ID = [ST].ST_ID AND
    [NAM].ST_NAM_ChangedAt = (
        SELECT
            max(sub.ST_NAM_ChangedAt)
        FROM
            [dbo].[ST_NAM_Stage_Name] sub
        WHERE
            sub.ST_NAM_ST_ID = [ST].ST_ID
   ) LEFT JOIN [dbo].[ST_LOC_Stage_Location] [LOC] ON [LOC].ST_LOC_ST_ID = [ST].ST_ID LEFT JOIN [dbo].[ST_AVG_Stage_Average] [AVG] ON [AVG].ST_AVG_ST_ID = [ST].ST_ID AND
    [AVG].ST_AVG_ChangedAt = (
        SELECT
            max(sub.ST_AVG_ChangedAt)
        FROM
            [dbo].[ST_AVG_Stage_Average] sub
        WHERE
            sub.ST_AVG_ST_ID = [ST].ST_ID
   ) LEFT JOIN [dbo].[UTL_Utilization] [kAVG] ON [kAVG].UTL_ID = [AVG].ST_AVG_UTL_ID LEFT JOIN [dbo].[ST_MIN_Stage_Minimum] [MIN] ON [MIN].ST_MIN_ST_ID = [ST].ST_ID LEFT JOIN [dbo].[UTL_Utilization] [kMIN] ON [kMIN].UTL_ID = [MIN].ST_MIN_UTL_ID