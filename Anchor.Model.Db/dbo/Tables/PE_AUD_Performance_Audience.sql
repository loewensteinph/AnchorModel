CREATE TABLE [dbo].[PE_AUD_Performance_Audience] (
    [PE_AUD_PE_ID]                INT NOT NULL,
    [PE_AUD_Performance_Audience] INT NOT NULL,
    [Metadata_PE_AUD]             INT NOT NULL,
    CONSTRAINT [pkPE_AUD_Performance_Audience] PRIMARY KEY CLUSTERED ([PE_AUD_PE_ID] ASC),
    CONSTRAINT [fkPE_AUD_Performance_Audience] FOREIGN KEY ([PE_AUD_PE_ID]) REFERENCES [dbo].[PE_Performance] ([PE_ID])
);

