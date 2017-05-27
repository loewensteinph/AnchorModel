CREATE TABLE [dbo].[ST_LOC_Stage_Location] (
    [ST_LOC_ST_ID]          INT               NOT NULL,
    [ST_LOC_Stage_Location] [sys].[geography] NOT NULL,
    [ST_LOC_Checksum]       AS                cast(dbo.MD5(cast(ST_LOC_Stage_Location as varbinary(max))) as varbinary(16)) PERSISTED,
    [Metadata_ST_LOC]       INT               NOT NULL,
    CONSTRAINT [pkST_LOC_Stage_Location] PRIMARY KEY CLUSTERED ([ST_LOC_ST_ID] ASC),
    CONSTRAINT [fkST_LOC_Stage_Location] FOREIGN KEY ([ST_LOC_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID])
);

