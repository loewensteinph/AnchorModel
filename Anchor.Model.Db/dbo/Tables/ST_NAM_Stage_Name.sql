CREATE TABLE [dbo].[ST_NAM_Stage_Name] (
    [ST_NAM_ST_ID]      INT          NOT NULL,
    [ST_NAM_Stage_Name] VARCHAR (42) NOT NULL,
    [ST_NAM_ChangedAt]  DATETIME     NOT NULL,
    [Metadata_ST_NAM]   INT          NOT NULL,
    CONSTRAINT [pkST_NAM_Stage_Name] PRIMARY KEY CLUSTERED ([ST_NAM_ST_ID] ASC, [ST_NAM_ChangedAt] DESC),
    CONSTRAINT [fkST_NAM_Stage_Name] FOREIGN KEY ([ST_NAM_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID])
);




GO
CREATE TRIGGER [dbo].[it_ST_NAM_Stage_Name]
    ON [dbo].[ST_NAM_Stage_Name]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @ST_NAM_Stage_Name TABLE (
               ST_NAM_ST_ID         INT          NOT NULL,
               Metadata_ST_NAM      INT          NOT NULL,
               ST_NAM_ChangedAt     DATETIME     NOT NULL,
               ST_NAM_Stage_Name    VARCHAR (42) NOT NULL,
               ST_NAM_Version       BIGINT       NOT NULL,
               ST_NAM_StatementType CHAR (1)     NOT NULL,
               PRIMARY KEY (ST_NAM_Version, ST_NAM_ST_ID));
           INSERT INTO @ST_NAM_Stage_Name
           SELECT i.ST_NAM_ST_ID,
                  i.Metadata_ST_NAM,
                  i.ST_NAM_ChangedAt,
                  i.ST_NAM_Stage_Name,
                  DENSE_RANK() OVER (PARTITION BY i.ST_NAM_ST_ID ORDER BY i.ST_NAM_ChangedAt ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = max(ST_NAM_Version),
                  @currentVersion = 0
           FROM   @ST_NAM_Stage_Name;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.ST_NAM_StatementType = CASE WHEN [NAM].ST_NAM_ST_ID IS NOT NULL THEN 'D' WHEN [dbo].[rfST_NAM_Stage_Name](v.ST_NAM_ST_ID, v.ST_NAM_Stage_Name, v.ST_NAM_ChangedAt) = 1 THEN 'R' ELSE 'N' END
                   FROM   @ST_NAM_Stage_Name AS v
                          LEFT OUTER JOIN
                          [dbo].[ST_NAM_Stage_Name] AS [NAM]
                          ON [NAM].ST_NAM_ST_ID = v.ST_NAM_ST_ID
                             AND [NAM].ST_NAM_ChangedAt = v.ST_NAM_ChangedAt
                             AND [NAM].ST_NAM_Stage_Name = v.ST_NAM_Stage_Name
                   WHERE  v.ST_NAM_Version = @currentVersion;
                   INSERT INTO [dbo].[ST_NAM_Stage_Name] (ST_NAM_ST_ID, Metadata_ST_NAM, ST_NAM_ChangedAt, ST_NAM_Stage_Name)
                   SELECT ST_NAM_ST_ID,
                          Metadata_ST_NAM,
                          ST_NAM_ChangedAt,
                          ST_NAM_Stage_Name
                   FROM   @ST_NAM_Stage_Name
                   WHERE  ST_NAM_Version = @currentVersion
                          AND ST_NAM_StatementType IN ('N', 'R');
               END
       END