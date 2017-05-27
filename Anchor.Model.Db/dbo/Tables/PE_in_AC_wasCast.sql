CREATE TABLE [dbo].[PE_in_AC_wasCast] (
    [PE_ID_in]                  INT NOT NULL,
    [AC_ID_wasCast]             INT NOT NULL,
    [Metadata_PE_in_AC_wasCast] INT NOT NULL,
    CONSTRAINT [pkPE_in_AC_wasCast] PRIMARY KEY CLUSTERED ([PE_ID_in] ASC, [AC_ID_wasCast] ASC),
    CONSTRAINT [PE_in_AC_wasCast_fkAC_wasCast] FOREIGN KEY ([AC_ID_wasCast]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [PE_in_AC_wasCast_fkPE_in] FOREIGN KEY ([PE_ID_in]) REFERENCES [dbo].[PE_Performance] ([PE_ID])
);

