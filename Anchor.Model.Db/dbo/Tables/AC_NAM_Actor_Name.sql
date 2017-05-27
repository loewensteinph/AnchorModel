CREATE TABLE [dbo].[AC_NAM_Actor_Name] (
    [AC_NAM_AC_ID]      INT          NOT NULL,
    [AC_NAM_Actor_Name] VARCHAR (42) NOT NULL,
    [AC_NAM_ChangedAt]  DATETIME     NOT NULL,
    [Metadata_AC_NAM]   INT          NOT NULL,
    CONSTRAINT [pkAC_NAM_Actor_Name] PRIMARY KEY CLUSTERED ([AC_NAM_AC_ID] ASC, [AC_NAM_ChangedAt] DESC),
    CONSTRAINT [fkAC_NAM_Actor_Name] FOREIGN KEY ([AC_NAM_AC_ID]) REFERENCES [dbo].[AC_Actor] ([AC_ID])
);

