CREATE TABLE [dbo].[PE_REV_Performance_Revenue] (
    [PE_REV_PE_ID]               INT      NOT NULL,
    [PE_REV_Performance_Revenue] MONEY    NOT NULL,
    [PE_REV_ChangedAt]           DATETIME NOT NULL,
    [PE_REV_Checksum]            AS       cast(dbo.MD5(cast(PE_REV_Performance_Revenue as varbinary(max))) as varbinary(16)) PERSISTED,
    [Metadata_PE_REV]            INT      NOT NULL,
    CONSTRAINT [pkPE_REV_Performance_Revenue] PRIMARY KEY CLUSTERED ([PE_REV_PE_ID] ASC, [PE_REV_ChangedAt] DESC),
    CONSTRAINT [fkPE_REV_Performance_Revenue] FOREIGN KEY ([PE_REV_PE_ID]) REFERENCES [dbo].[PE_Performance] ([PE_ID])
);

