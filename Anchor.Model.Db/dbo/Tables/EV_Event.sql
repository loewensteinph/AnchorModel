CREATE TABLE [dbo].[EV_Event] (
    [EV_ID]       INT IDENTITY (1, 1) NOT NULL,
    [Metadata_EV] INT NOT NULL,
    CONSTRAINT [pkEV_Event] PRIMARY KEY CLUSTERED ([EV_ID] ASC)
);

