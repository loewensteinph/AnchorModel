CREATE TABLE [dbo].[PR_content_ST_location_PE_of] (
    [PR_ID_content]                         INT NOT NULL,
    [ST_ID_location]                        INT NOT NULL,
    [PE_ID_of]                              INT NOT NULL,
    [Metadata_PR_content_ST_location_PE_of] INT NOT NULL,
    CONSTRAINT [pkPR_content_ST_location_PE_of] PRIMARY KEY CLUSTERED ([PE_ID_of] ASC),
    CONSTRAINT [PR_content_ST_location_PE_of_fkPE_of] FOREIGN KEY ([PE_ID_of]) REFERENCES [dbo].[PE_Performance] ([PE_ID]),
    CONSTRAINT [PR_content_ST_location_PE_of_fkPR_content] FOREIGN KEY ([PR_ID_content]) REFERENCES [dbo].[PR_Program] ([PR_ID]),
    CONSTRAINT [PR_content_ST_location_PE_of_fkST_location] FOREIGN KEY ([ST_ID_location]) REFERENCES [dbo].[ST_Stage] ([ST_ID])
);

