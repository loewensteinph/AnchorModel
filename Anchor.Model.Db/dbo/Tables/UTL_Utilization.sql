CREATE TABLE [dbo].[UTL_Utilization] (
    [UTL_ID]          TINYINT NOT NULL,
    [UTL_Utilization] TINYINT NOT NULL,
    [Metadata_UTL]    INT     NOT NULL,
    CONSTRAINT [pkUTL_Utilization] PRIMARY KEY CLUSTERED ([UTL_ID] ASC),
    CONSTRAINT [uqUTL_Utilization] UNIQUE NONCLUSTERED ([UTL_Utilization] ASC)
);

