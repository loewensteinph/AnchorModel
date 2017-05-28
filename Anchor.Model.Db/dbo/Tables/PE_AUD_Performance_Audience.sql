CREATE TABLE [dbo].[PE_AUD_Performance_Audience] (
    [PE_AUD_PE_ID]                INT NOT NULL,
    [PE_AUD_Performance_Audience] INT NOT NULL,
    [Metadata_PE_AUD]             INT NOT NULL,
    CONSTRAINT [pkPE_AUD_Performance_Audience] PRIMARY KEY CLUSTERED ([PE_AUD_PE_ID] ASC),
    CONSTRAINT [fkPE_AUD_Performance_Audience] FOREIGN KEY ([PE_AUD_PE_ID]) REFERENCES [dbo].[PE_Performance] ([PE_ID])
);




GO
CREATE TRIGGER [dbo].[it_PE_AUD_Performance_Audience]
    ON [dbo].[PE_AUD_Performance_Audience]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @PE_AUD_Performance_Audience TABLE (
               PE_AUD_PE_ID                INT      NOT NULL,
               Metadata_PE_AUD             INT      NOT NULL,
               PE_AUD_Performance_Audience INT      NOT NULL,
               PE_AUD_Version              BIGINT   NOT NULL,
               PE_AUD_StatementType        CHAR (1) NOT NULL,
               PRIMARY KEY (PE_AUD_Version, PE_AUD_PE_ID));
           INSERT INTO @PE_AUD_Performance_Audience
           SELECT i.PE_AUD_PE_ID,
                  i.Metadata_PE_AUD,
                  i.PE_AUD_Performance_Audience,
                  ROW_NUMBER() OVER (PARTITION BY i.PE_AUD_PE_ID ORDER BY (SELECT 1) ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = 1,
                  @currentVersion = 0
           FROM   @PE_AUD_Performance_Audience;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.PE_AUD_StatementType = CASE WHEN [AUD].PE_AUD_PE_ID IS NOT NULL THEN 'D' ELSE 'N' END
                   FROM   @PE_AUD_Performance_Audience AS v
                          LEFT OUTER JOIN
                          [dbo].[PE_AUD_Performance_Audience] AS [AUD]
                          ON [AUD].PE_AUD_PE_ID = v.PE_AUD_PE_ID
                             AND [AUD].PE_AUD_Performance_Audience = v.PE_AUD_Performance_Audience
                   WHERE  v.PE_AUD_Version = @currentVersion;
                   INSERT INTO [dbo].[PE_AUD_Performance_Audience] (PE_AUD_PE_ID, Metadata_PE_AUD, PE_AUD_Performance_Audience)
                   SELECT PE_AUD_PE_ID,
                          Metadata_PE_AUD,
                          PE_AUD_Performance_Audience
                   FROM   @PE_AUD_Performance_Audience
                   WHERE  PE_AUD_Version = @currentVersion
                          AND PE_AUD_StatementType IN ('N');
               END
       END