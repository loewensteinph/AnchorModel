CREATE TABLE [dbo].[PE_DAT_Performance_Date] (
    [PE_DAT_PE_ID]            INT      NOT NULL,
    [PE_DAT_Performance_Date] DATETIME NOT NULL,
    [Metadata_PE_DAT]         INT      NOT NULL,
    CONSTRAINT [pkPE_DAT_Performance_Date] PRIMARY KEY CLUSTERED ([PE_DAT_PE_ID] ASC),
    CONSTRAINT [fkPE_DAT_Performance_Date] FOREIGN KEY ([PE_DAT_PE_ID]) REFERENCES [dbo].[PE_Performance] ([PE_ID])
);

