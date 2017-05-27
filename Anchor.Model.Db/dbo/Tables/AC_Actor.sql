CREATE TABLE [dbo].[AC_Actor] (
    [AC_ID]       INT IDENTITY (1, 1) NOT NULL,
    [Metadata_AC] INT NOT NULL,
    CONSTRAINT [pkAC_Actor] PRIMARY KEY CLUSTERED ([AC_ID] ASC)
);

