CREATE TABLE [dbo].[PE_Performance] (
    [PE_ID]       INT IDENTITY (1, 1) NOT NULL,
    [Metadata_PE] INT NOT NULL,
    CONSTRAINT [pkPE_Performance] PRIMARY KEY CLUSTERED ([PE_ID] ASC)
);

