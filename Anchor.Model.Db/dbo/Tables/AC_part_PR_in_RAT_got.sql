CREATE TABLE [dbo].[AC_part_PR_in_RAT_got] (
    [AC_ID_part]                     INT     NOT NULL,
    [PR_ID_in]                       INT     NOT NULL,
    [RAT_ID_got]                     TINYINT NOT NULL,
    [Metadata_AC_part_PR_in_RAT_got] INT     NOT NULL,
    CONSTRAINT [pkAC_part_PR_in_RAT_got] PRIMARY KEY CLUSTERED ([AC_ID_part] ASC, [PR_ID_in] ASC),
    CONSTRAINT [AC_part_PR_in_RAT_got_fkAC_part] FOREIGN KEY ([AC_ID_part]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [AC_part_PR_in_RAT_got_fkPR_in] FOREIGN KEY ([PR_ID_in]) REFERENCES [dbo].[PR_Program] ([PR_ID]),
    CONSTRAINT [AC_part_PR_in_RAT_got_fkRAT_got] FOREIGN KEY ([RAT_ID_got]) REFERENCES [dbo].[RAT_Rating] ([RAT_ID])
);



