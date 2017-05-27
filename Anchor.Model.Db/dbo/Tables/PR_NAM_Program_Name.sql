CREATE TABLE [dbo].[PR_NAM_Program_Name] (
    [PR_NAM_PR_ID]        INT          NOT NULL,
    [PR_NAM_Program_Name] VARCHAR (42) NOT NULL,
    [Metadata_PR_NAM]     INT          NOT NULL,
    CONSTRAINT [pkPR_NAM_Program_Name] PRIMARY KEY CLUSTERED ([PR_NAM_PR_ID] ASC),
    CONSTRAINT [fkPR_NAM_Program_Name] FOREIGN KEY ([PR_NAM_PR_ID]) REFERENCES [dbo].[PR_Program] ([PR_ID])
);

