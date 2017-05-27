CREATE TABLE [dbo].[RAT_Rating] (
    [RAT_ID]       TINYINT      NOT NULL,
    [RAT_Rating]   VARCHAR (42) NOT NULL,
    [Metadata_RAT] INT          NOT NULL,
    CONSTRAINT [pkRAT_Rating] PRIMARY KEY CLUSTERED ([RAT_ID] ASC),
    CONSTRAINT [uqRAT_Rating] UNIQUE NONCLUSTERED ([RAT_Rating] ASC)
);

