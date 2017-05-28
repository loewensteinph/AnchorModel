CREATE TABLE [dbo].[PE_DAT_Performance_Date] (
    [PE_DAT_PE_ID]            INT      NOT NULL,
    [PE_DAT_Performance_Date] DATETIME NOT NULL,
    [Metadata_PE_DAT]         INT      NOT NULL,
    CONSTRAINT [pkPE_DAT_Performance_Date] PRIMARY KEY CLUSTERED ([PE_DAT_PE_ID] ASC),
    CONSTRAINT [fkPE_DAT_Performance_Date] FOREIGN KEY ([PE_DAT_PE_ID]) REFERENCES [dbo].[PE_Performance] ([PE_ID])
);




GO
CREATE TRIGGER [dbo].[it_PE_DAT_Performance_Date]
    ON [dbo].[PE_DAT_Performance_Date]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @PE_DAT_Performance_Date TABLE (
               PE_DAT_PE_ID            INT      NOT NULL,
               Metadata_PE_DAT         INT      NOT NULL,
               PE_DAT_Performance_Date DATETIME NOT NULL,
               PE_DAT_Version          BIGINT   NOT NULL,
               PE_DAT_StatementType    CHAR (1) NOT NULL,
               PRIMARY KEY (PE_DAT_Version, PE_DAT_PE_ID));
           INSERT INTO @PE_DAT_Performance_Date
           SELECT i.PE_DAT_PE_ID,
                  i.Metadata_PE_DAT,
                  i.PE_DAT_Performance_Date,
                  ROW_NUMBER() OVER (PARTITION BY i.PE_DAT_PE_ID ORDER BY (SELECT 1) ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = 1,
                  @currentVersion = 0
           FROM   @PE_DAT_Performance_Date;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.PE_DAT_StatementType = CASE WHEN [DAT].PE_DAT_PE_ID IS NOT NULL THEN 'D' ELSE 'N' END
                   FROM   @PE_DAT_Performance_Date AS v
                          LEFT OUTER JOIN
                          [dbo].[PE_DAT_Performance_Date] AS [DAT]
                          ON [DAT].PE_DAT_PE_ID = v.PE_DAT_PE_ID
                             AND [DAT].PE_DAT_Performance_Date = v.PE_DAT_Performance_Date
                   WHERE  v.PE_DAT_Version = @currentVersion;
                   INSERT INTO [dbo].[PE_DAT_Performance_Date] (PE_DAT_PE_ID, Metadata_PE_DAT, PE_DAT_Performance_Date)
                   SELECT PE_DAT_PE_ID,
                          Metadata_PE_DAT,
                          PE_DAT_Performance_Date
                   FROM   @PE_DAT_Performance_Date
                   WHERE  PE_DAT_Version = @currentVersion
                          AND PE_DAT_StatementType IN ('N');
               END
       END