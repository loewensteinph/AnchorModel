CREATE TABLE [dbo].[AC_PLV_Actor_ProfessionalLevel] (
    [AC_PLV_AC_ID]     INT      NOT NULL,
    [AC_PLV_PLV_ID]    TINYINT  NOT NULL,
    [AC_PLV_ChangedAt] DATETIME NOT NULL,
    [Metadata_AC_PLV]  INT      NOT NULL,
    CONSTRAINT [pkAC_PLV_Actor_ProfessionalLevel] PRIMARY KEY CLUSTERED ([AC_PLV_AC_ID] ASC, [AC_PLV_ChangedAt] DESC),
    CONSTRAINT [fk_A_AC_PLV_Actor_ProfessionalLevel] FOREIGN KEY ([AC_PLV_AC_ID]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [fk_K_AC_PLV_Actor_ProfessionalLevel] FOREIGN KEY ([AC_PLV_PLV_ID]) REFERENCES [dbo].[PLV_ProfessionalLevel] ([PLV_ID])
);

