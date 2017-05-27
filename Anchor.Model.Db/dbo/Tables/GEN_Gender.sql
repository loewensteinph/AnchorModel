CREATE TABLE [dbo].[GEN_Gender] (
    [GEN_ID]       BIT          NOT NULL,
    [GEN_Gender]   VARCHAR (42) NOT NULL,
    [Metadata_GEN] INT          NOT NULL,
    CONSTRAINT [pkGEN_Gender] PRIMARY KEY CLUSTERED ([GEN_ID] ASC),
    CONSTRAINT [uqGEN_Gender] UNIQUE NONCLUSTERED ([GEN_Gender] ASC)
);

