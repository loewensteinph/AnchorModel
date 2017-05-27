CREATE TABLE [dbo].[PE_at_PR_wasPlayed] (
    [PE_ID_at]                    INT NOT NULL,
    [PR_ID_wasPlayed]             INT NOT NULL,
    [Metadata_PE_at_PR_wasPlayed] INT NOT NULL,
    CONSTRAINT [pkPE_at_PR_wasPlayed] PRIMARY KEY CLUSTERED ([PE_ID_at] ASC),
    CONSTRAINT [PE_at_PR_wasPlayed_fkPE_at] FOREIGN KEY ([PE_ID_at]) REFERENCES [dbo].[PE_Performance] ([PE_ID]),
    CONSTRAINT [PE_at_PR_wasPlayed_fkPR_wasPlayed] FOREIGN KEY ([PR_ID_wasPlayed]) REFERENCES [dbo].[PR_Program] ([PR_ID])
);

