CREATE TABLE [dbo].[PR_Program] (
    [PR_ID]       INT IDENTITY (1, 1) NOT NULL,
    [Metadata_PR] INT NOT NULL,
    CONSTRAINT [pkPR_Program] PRIMARY KEY CLUSTERED ([PR_ID] ASC)
);

