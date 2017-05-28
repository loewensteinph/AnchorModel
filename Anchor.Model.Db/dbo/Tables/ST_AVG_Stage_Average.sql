CREATE TABLE [dbo].[ST_AVG_Stage_Average] (
    [ST_AVG_ST_ID]     INT      NOT NULL,
    [ST_AVG_UTL_ID]    TINYINT  NOT NULL,
    [ST_AVG_ChangedAt] DATETIME NOT NULL,
    [Metadata_ST_AVG]  INT      NOT NULL,
    CONSTRAINT [pkST_AVG_Stage_Average] PRIMARY KEY CLUSTERED ([ST_AVG_ST_ID] ASC, [ST_AVG_ChangedAt] DESC),
    CONSTRAINT [fk_A_ST_AVG_Stage_Average] FOREIGN KEY ([ST_AVG_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID]),
    CONSTRAINT [fk_K_ST_AVG_Stage_Average] FOREIGN KEY ([ST_AVG_UTL_ID]) REFERENCES [dbo].[UTL_Utilization] ([UTL_ID])
);




GO
CREATE TRIGGER [dbo].[it_ST_AVG_Stage_Average]
    ON [dbo].[ST_AVG_Stage_Average]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @ST_AVG_Stage_Average TABLE (
               ST_AVG_ST_ID         INT      NOT NULL,
               Metadata_ST_AVG      INT      NOT NULL,
               ST_AVG_ChangedAt     DATETIME NOT NULL,
               ST_AVG_UTL_ID        TINYINT  NOT NULL,
               ST_AVG_Version       BIGINT   NOT NULL,
               ST_AVG_StatementType CHAR (1) NOT NULL,
               PRIMARY KEY (ST_AVG_Version, ST_AVG_ST_ID));
           INSERT INTO @ST_AVG_Stage_Average
           SELECT i.ST_AVG_ST_ID,
                  i.Metadata_ST_AVG,
                  i.ST_AVG_ChangedAt,
                  i.ST_AVG_UTL_ID,
                  DENSE_RANK() OVER (PARTITION BY i.ST_AVG_ST_ID ORDER BY i.ST_AVG_ChangedAt ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = max(ST_AVG_Version),
                  @currentVersion = 0
           FROM   @ST_AVG_Stage_Average;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.ST_AVG_StatementType = CASE WHEN [AVG].ST_AVG_ST_ID IS NOT NULL THEN 'D' WHEN [dbo].[rfST_AVG_Stage_Average](v.ST_AVG_ST_ID, v.ST_AVG_UTL_ID, v.ST_AVG_ChangedAt) = 1 THEN 'R' ELSE 'N' END
                   FROM   @ST_AVG_Stage_Average AS v
                          LEFT OUTER JOIN
                          [dbo].[ST_AVG_Stage_Average] AS [AVG]
                          ON [AVG].ST_AVG_ST_ID = v.ST_AVG_ST_ID
                             AND [AVG].ST_AVG_ChangedAt = v.ST_AVG_ChangedAt
                             AND [AVG].ST_AVG_UTL_ID = v.ST_AVG_UTL_ID
                   WHERE  v.ST_AVG_Version = @currentVersion;
                   INSERT INTO [dbo].[ST_AVG_Stage_Average] (ST_AVG_ST_ID, Metadata_ST_AVG, ST_AVG_ChangedAt, ST_AVG_UTL_ID)
                   SELECT ST_AVG_ST_ID,
                          Metadata_ST_AVG,
                          ST_AVG_ChangedAt,
                          ST_AVG_UTL_ID
                   FROM   @ST_AVG_Stage_Average
                   WHERE  ST_AVG_Version = @currentVersion
                          AND ST_AVG_StatementType IN ('N', 'R');
               END
       END