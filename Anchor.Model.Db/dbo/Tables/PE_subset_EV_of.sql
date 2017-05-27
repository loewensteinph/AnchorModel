CREATE TABLE [dbo].[PE_subset_EV_of] (
    [PE_ID_subset]             INT NOT NULL,
    [EV_ID_of]                 INT NOT NULL,
    [Metadata_PE_subset_EV_of] INT NOT NULL,
    CONSTRAINT [pkPE_subset_EV_of] PRIMARY KEY CLUSTERED ([PE_ID_subset] ASC, [EV_ID_of] ASC),
    CONSTRAINT [PE_subset_EV_of_fkEV_of] FOREIGN KEY ([EV_ID_of]) REFERENCES [dbo].[EV_Event] ([EV_ID]),
    CONSTRAINT [PE_subset_EV_of_fkPE_subset] FOREIGN KEY ([PE_ID_subset]) REFERENCES [dbo].[PE_Performance] ([PE_ID]),
    CONSTRAINT [PE_subset_EV_of_uqEV_of] UNIQUE NONCLUSTERED ([EV_ID_of] ASC),
    CONSTRAINT [PE_subset_EV_of_uqPE_subset] UNIQUE NONCLUSTERED ([PE_ID_subset] ASC)
);

