CREATE TABLE [dbo].[ST_NAM_Stage_Name] (
    [ST_NAM_ST_ID]      INT          NOT NULL,
    [ST_NAM_Stage_Name] VARCHAR (42) NOT NULL,
    [ST_NAM_ChangedAt]  DATETIME     NOT NULL,
    [Metadata_ST_NAM]   INT          NOT NULL,
    CONSTRAINT [pkST_NAM_Stage_Name] PRIMARY KEY CLUSTERED ([ST_NAM_ST_ID] ASC, [ST_NAM_ChangedAt] DESC),
    CONSTRAINT [fkST_NAM_Stage_Name] FOREIGN KEY ([ST_NAM_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID])
);

