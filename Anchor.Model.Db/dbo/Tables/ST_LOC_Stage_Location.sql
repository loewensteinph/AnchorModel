CREATE TABLE [dbo].[ST_LOC_Stage_Location] (
    [ST_LOC_ST_ID]          INT               NOT NULL,
    [ST_LOC_Stage_Location] [sys].[geography] NOT NULL,
    [ST_LOC_Checksum]       AS                cast(dbo.MD5(cast(ST_LOC_Stage_Location as varbinary(max))) as varbinary(16)) PERSISTED,
    [Metadata_ST_LOC]       INT               NOT NULL,
    CONSTRAINT [pkST_LOC_Stage_Location] PRIMARY KEY CLUSTERED ([ST_LOC_ST_ID] ASC),
    CONSTRAINT [fkST_LOC_Stage_Location] FOREIGN KEY ([ST_LOC_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID])
);




GO
CREATE TRIGGER [dbo].[it_ST_LOC_Stage_Location]
    ON [dbo].[ST_LOC_Stage_Location]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @ST_LOC_Stage_Location TABLE (
               ST_LOC_ST_ID          INT            NOT NULL,
               Metadata_ST_LOC       INT            NOT NULL,
               ST_LOC_Stage_Location geography      NOT NULL,
               ST_LOC_Checksum       VARBINARY (16) NOT NULL,
               ST_LOC_Version        BIGINT         NOT NULL,
               ST_LOC_StatementType  CHAR (1)       NOT NULL,
               PRIMARY KEY (ST_LOC_Version, ST_LOC_ST_ID));
           INSERT INTO @ST_LOC_Stage_Location
           SELECT i.ST_LOC_ST_ID,
                  i.Metadata_ST_LOC,
                  i.ST_LOC_Stage_Location,
                  dbo.MD5(CAST (i.ST_LOC_Stage_Location AS VARBINARY (MAX))),
                  ROW_NUMBER() OVER (PARTITION BY i.ST_LOC_ST_ID ORDER BY (SELECT 1) ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = 1,
                  @currentVersion = 0
           FROM   @ST_LOC_Stage_Location;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.ST_LOC_StatementType = CASE WHEN [LOC].ST_LOC_ST_ID IS NOT NULL THEN 'D' ELSE 'N' END
                   FROM   @ST_LOC_Stage_Location AS v
                          LEFT OUTER JOIN
                          [dbo].[ST_LOC_Stage_Location] AS [LOC]
                          ON [LOC].ST_LOC_ST_ID = v.ST_LOC_ST_ID
                             AND [LOC].ST_LOC_Checksum = v.ST_LOC_Checksum
                   WHERE  v.ST_LOC_Version = @currentVersion;
                   INSERT INTO [dbo].[ST_LOC_Stage_Location] (ST_LOC_ST_ID, Metadata_ST_LOC, ST_LOC_Stage_Location)
                   SELECT ST_LOC_ST_ID,
                          Metadata_ST_LOC,
                          ST_LOC_Stage_Location
                   FROM   @ST_LOC_Stage_Location
                   WHERE  ST_LOC_Version = @currentVersion
                          AND ST_LOC_StatementType IN ('N');
               END
       END