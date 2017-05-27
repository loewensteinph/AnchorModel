CREATE TABLE [dbo].[PLV_ProfessionalLevel] (
    [PLV_ID]                TINYINT       NOT NULL,
    [PLV_ProfessionalLevel] VARCHAR (MAX) NOT NULL,
    [PLV_Checksum]          AS            cast(dbo.MD5(cast(PLV_ProfessionalLevel as varbinary(max))) as varbinary(16)) PERSISTED,
    [Metadata_PLV]          INT           NOT NULL,
    CONSTRAINT [pkPLV_ProfessionalLevel] PRIMARY KEY CLUSTERED ([PLV_ID] ASC),
    CONSTRAINT [uqPLV_ProfessionalLevel] UNIQUE NONCLUSTERED ([PLV_Checksum] ASC)
);

