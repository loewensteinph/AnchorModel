CREATE TABLE [dbo].[PAT_ParentalType] (
    [PAT_ID]           TINYINT      NOT NULL,
    [PAT_ParentalType] VARCHAR (42) NOT NULL,
    [Metadata_PAT]     INT          NOT NULL,
    CONSTRAINT [pkPAT_ParentalType] PRIMARY KEY CLUSTERED ([PAT_ID] ASC),
    CONSTRAINT [uqPAT_ParentalType] UNIQUE NONCLUSTERED ([PAT_ParentalType] ASC)
);

