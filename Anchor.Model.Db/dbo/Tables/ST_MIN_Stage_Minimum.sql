CREATE TABLE [dbo].[ST_MIN_Stage_Minimum] (
    [ST_MIN_ST_ID]    INT     NOT NULL,
    [ST_MIN_UTL_ID]   TINYINT NOT NULL,
    [Metadata_ST_MIN] INT     NOT NULL,
    CONSTRAINT [pkST_MIN_Stage_Minimum] PRIMARY KEY CLUSTERED ([ST_MIN_ST_ID] ASC),
    CONSTRAINT [fk_A_ST_MIN_Stage_Minimum] FOREIGN KEY ([ST_MIN_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID]),
    CONSTRAINT [fk_K_ST_MIN_Stage_Minimum] FOREIGN KEY ([ST_MIN_UTL_ID]) REFERENCES [dbo].[UTL_Utilization] ([UTL_ID])
);

