CREATE TABLE [dbo].[AC_parent_AC_child_PAT_having] (
    [AC_ID_parent]                            INT      NOT NULL,
    [AC_ID_child]                             INT      NOT NULL,
    [PAT_ID_having]                           TINYINT  NOT NULL,
    [AC_parent_AC_child_PAT_having_ChangedAt] DATETIME NOT NULL,
    [Metadata_AC_parent_AC_child_PAT_having]  INT      NOT NULL,
    CONSTRAINT [pkAC_parent_AC_child_PAT_having] PRIMARY KEY CLUSTERED ([AC_ID_parent] ASC, [AC_ID_child] ASC, [PAT_ID_having] ASC, [AC_parent_AC_child_PAT_having_ChangedAt] DESC),
    CONSTRAINT [AC_parent_AC_child_PAT_having_fkAC_child] FOREIGN KEY ([AC_ID_child]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [AC_parent_AC_child_PAT_having_fkAC_parent] FOREIGN KEY ([AC_ID_parent]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [AC_parent_AC_child_PAT_having_fkPAT_having] FOREIGN KEY ([PAT_ID_having]) REFERENCES [dbo].[PAT_ParentalType] ([PAT_ID])
);

