CREATE TABLE [dbo].[PE_REV_Performance_Revenue] (
    [PE_REV_PE_ID]               INT      NOT NULL,
    [PE_REV_Performance_Revenue] MONEY    NOT NULL,
    [PE_REV_ChangedAt]           DATETIME NOT NULL,
    [PE_REV_Checksum]            AS       cast(dbo.MD5(cast(PE_REV_Performance_Revenue as varbinary(max))) as varbinary(16)) PERSISTED,
    [Metadata_PE_REV]            INT      NOT NULL,
    CONSTRAINT [pkPE_REV_Performance_Revenue] PRIMARY KEY CLUSTERED ([PE_REV_PE_ID] ASC, [PE_REV_ChangedAt] DESC),
    CONSTRAINT [fkPE_REV_Performance_Revenue] FOREIGN KEY ([PE_REV_PE_ID]) REFERENCES [dbo].[PE_Performance] ([PE_ID])
);




GO
CREATE TRIGGER [dbo].[it_PE_REV_Performance_Revenue]
    ON [dbo].[PE_REV_Performance_Revenue]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @PE_REV_Performance_Revenue TABLE (
               PE_REV_PE_ID               INT            NOT NULL,
               Metadata_PE_REV            INT            NOT NULL,
               PE_REV_ChangedAt           DATETIME       NOT NULL,
               PE_REV_Performance_Revenue MONEY          NOT NULL,
               PE_REV_Checksum            VARBINARY (16) NOT NULL,
               PE_REV_Version             BIGINT         NOT NULL,
               PE_REV_StatementType       CHAR (1)       NOT NULL,
               PRIMARY KEY (PE_REV_Version, PE_REV_PE_ID));
           INSERT INTO @PE_REV_Performance_Revenue
           SELECT i.PE_REV_PE_ID,
                  i.Metadata_PE_REV,
                  i.PE_REV_ChangedAt,
                  i.PE_REV_Performance_Revenue,
                  dbo.MD5(CAST (i.PE_REV_Performance_Revenue AS VARBINARY (MAX))),
                  DENSE_RANK() OVER (PARTITION BY i.PE_REV_PE_ID ORDER BY i.PE_REV_ChangedAt ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = max(PE_REV_Version),
                  @currentVersion = 0
           FROM   @PE_REV_Performance_Revenue;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.PE_REV_StatementType = CASE WHEN [REV].PE_REV_PE_ID IS NOT NULL THEN 'D' WHEN [dbo].[rfPE_REV_Performance_Revenue](v.PE_REV_PE_ID, v.PE_REV_Checksum, v.PE_REV_ChangedAt) = 1 THEN 'R' ELSE 'N' END
                   FROM   @PE_REV_Performance_Revenue AS v
                          LEFT OUTER JOIN
                          [dbo].[PE_REV_Performance_Revenue] AS [REV]
                          ON [REV].PE_REV_PE_ID = v.PE_REV_PE_ID
                             AND [REV].PE_REV_ChangedAt = v.PE_REV_ChangedAt
                             AND [REV].PE_REV_Checksum = v.PE_REV_Checksum
                   WHERE  v.PE_REV_Version = @currentVersion;
                   INSERT INTO [dbo].[PE_REV_Performance_Revenue] (PE_REV_PE_ID, Metadata_PE_REV, PE_REV_ChangedAt, PE_REV_Performance_Revenue)
                   SELECT PE_REV_PE_ID,
                          Metadata_PE_REV,
                          PE_REV_ChangedAt,
                          PE_REV_Performance_Revenue
                   FROM   @PE_REV_Performance_Revenue
                   WHERE  PE_REV_Version = @currentVersion
                          AND PE_REV_StatementType IN ('N');
               END
       END