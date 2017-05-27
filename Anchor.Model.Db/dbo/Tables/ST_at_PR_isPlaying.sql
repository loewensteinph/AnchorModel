CREATE TABLE [dbo].[ST_at_PR_isPlaying] (
    [ST_ID_at]                     INT      NOT NULL,
    [PR_ID_isPlaying]              INT      NOT NULL,
    [ST_at_PR_isPlaying_ChangedAt] DATETIME NOT NULL,
    [Metadata_ST_at_PR_isPlaying]  INT      NOT NULL,
    CONSTRAINT [pkST_at_PR_isPlaying] PRIMARY KEY CLUSTERED ([ST_ID_at] ASC, [PR_ID_isPlaying] ASC, [ST_at_PR_isPlaying_ChangedAt] DESC),
    CONSTRAINT [ST_at_PR_isPlaying_fkPR_isPlaying] FOREIGN KEY ([PR_ID_isPlaying]) REFERENCES [dbo].[PR_Program] ([PR_ID]),
    CONSTRAINT [ST_at_PR_isPlaying_fkST_at] FOREIGN KEY ([ST_ID_at]) REFERENCES [dbo].[ST_Stage] ([ST_ID])
);

