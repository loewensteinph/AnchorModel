CREATE TABLE [dbo].[ST_AVG_Stage_Average] (
    [ST_AVG_ST_ID]     INT      NOT NULL,
    [ST_AVG_UTL_ID]    TINYINT  NOT NULL,
    [ST_AVG_ChangedAt] DATETIME NOT NULL,
    [Metadata_ST_AVG]  INT      NOT NULL,
    CONSTRAINT [pkST_AVG_Stage_Average] PRIMARY KEY CLUSTERED ([ST_AVG_ST_ID] ASC, [ST_AVG_ChangedAt] DESC),
    CONSTRAINT [fk_A_ST_AVG_Stage_Average] FOREIGN KEY ([ST_AVG_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID]),
    CONSTRAINT [fk_K_ST_AVG_Stage_Average] FOREIGN KEY ([ST_AVG_UTL_ID]) REFERENCES [dbo].[UTL_Utilization] ([UTL_ID])
);

