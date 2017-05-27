﻿CREATE TABLE [dbo].[AC_GEN_Actor_Gender] (
    [AC_GEN_AC_ID]    INT NOT NULL,
    [AC_GEN_GEN_ID]   BIT NOT NULL,
    [Metadata_AC_GEN] INT NOT NULL,
    CONSTRAINT [pkAC_GEN_Actor_Gender] PRIMARY KEY CLUSTERED ([AC_GEN_AC_ID] ASC),
    CONSTRAINT [fk_A_AC_GEN_Actor_Gender] FOREIGN KEY ([AC_GEN_AC_ID]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [fk_K_AC_GEN_Actor_Gender] FOREIGN KEY ([AC_GEN_GEN_ID]) REFERENCES [dbo].[GEN_Gender] ([GEN_ID])
);
