CREATE TABLE [dbo].[ONG_Ongoing] (
    [ONG_ID]       TINYINT     NOT NULL,
    [ONG_Ongoing]  VARCHAR (3) NOT NULL,
    [Metadata_ONG] INT         NOT NULL,
    CONSTRAINT [pkONG_Ongoing] PRIMARY KEY CLUSTERED ([ONG_ID] ASC),
    CONSTRAINT [uqONG_Ongoing] UNIQUE NONCLUSTERED ([ONG_Ongoing] ASC)
);

