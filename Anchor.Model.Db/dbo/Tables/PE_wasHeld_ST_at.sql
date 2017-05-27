CREATE TABLE [dbo].[PE_wasHeld_ST_at] (
    [PE_ID_wasHeld]             INT NOT NULL,
    [ST_ID_at]                  INT NOT NULL,
    [Metadata_PE_wasHeld_ST_at] INT NOT NULL,
    CONSTRAINT [pkPE_wasHeld_ST_at] PRIMARY KEY CLUSTERED ([PE_ID_wasHeld] ASC),
    CONSTRAINT [PE_wasHeld_ST_at_fkPE_wasHeld] FOREIGN KEY ([PE_ID_wasHeld]) REFERENCES [dbo].[PE_Performance] ([PE_ID]),
    CONSTRAINT [PE_wasHeld_ST_at_fkST_at] FOREIGN KEY ([ST_ID_at]) REFERENCES [dbo].[ST_Stage] ([ST_ID])
);

