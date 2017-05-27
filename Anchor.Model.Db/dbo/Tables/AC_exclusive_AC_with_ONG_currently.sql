CREATE TABLE [dbo].[AC_exclusive_AC_with_ONG_currently] (
    [AC_ID_exclusive]                              INT      NOT NULL,
    [AC_ID_with]                                   INT      NOT NULL,
    [ONG_ID_currently]                             TINYINT  NOT NULL,
    [AC_exclusive_AC_with_ONG_currently_ChangedAt] DATETIME NOT NULL,
    [Metadata_AC_exclusive_AC_with_ONG_currently]  INT      NOT NULL,
    CONSTRAINT [pkAC_exclusive_AC_with_ONG_currently] PRIMARY KEY CLUSTERED ([AC_ID_exclusive] ASC, [AC_ID_with] ASC, [ONG_ID_currently] ASC, [AC_exclusive_AC_with_ONG_currently_ChangedAt] DESC),
    CONSTRAINT [AC_exclusive_AC_with_ONG_currently_fkAC_exclusive] FOREIGN KEY ([AC_ID_exclusive]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [AC_exclusive_AC_with_ONG_currently_fkAC_with] FOREIGN KEY ([AC_ID_with]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [AC_exclusive_AC_with_ONG_currently_fkONG_currently] FOREIGN KEY ([ONG_ID_currently]) REFERENCES [dbo].[ONG_Ongoing] ([ONG_ID]),
    CONSTRAINT [AC_exclusive_AC_with_ONG_currently_uqAC_exclusive] UNIQUE NONCLUSTERED ([AC_ID_exclusive] ASC, [AC_exclusive_AC_with_ONG_currently_ChangedAt] ASC),
    CONSTRAINT [AC_exclusive_AC_with_ONG_currently_uqAC_with] UNIQUE NONCLUSTERED ([AC_ID_with] ASC, [AC_exclusive_AC_with_ONG_currently_ChangedAt] ASC)
);

