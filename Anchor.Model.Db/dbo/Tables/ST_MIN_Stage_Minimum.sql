CREATE TABLE [dbo].[ST_MIN_Stage_Minimum] (
    [ST_MIN_ST_ID]    INT     NOT NULL,
    [ST_MIN_UTL_ID]   TINYINT NOT NULL,
    [Metadata_ST_MIN] INT     NOT NULL,
    CONSTRAINT [pkST_MIN_Stage_Minimum] PRIMARY KEY CLUSTERED ([ST_MIN_ST_ID] ASC),
    CONSTRAINT [fk_A_ST_MIN_Stage_Minimum] FOREIGN KEY ([ST_MIN_ST_ID]) REFERENCES [dbo].[ST_Stage] ([ST_ID]),
    CONSTRAINT [fk_K_ST_MIN_Stage_Minimum] FOREIGN KEY ([ST_MIN_UTL_ID]) REFERENCES [dbo].[UTL_Utilization] ([UTL_ID])
);




GO
CREATE TRIGGER [dbo].[it_ST_MIN_Stage_Minimum]
    ON [dbo].[ST_MIN_Stage_Minimum]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @ST_MIN_Stage_Minimum TABLE (
               ST_MIN_ST_ID         INT      NOT NULL,
               Metadata_ST_MIN      INT      NOT NULL,
               ST_MIN_UTL_ID        TINYINT  NOT NULL,
               ST_MIN_Version       BIGINT   NOT NULL,
               ST_MIN_StatementType CHAR (1) NOT NULL,
               PRIMARY KEY (ST_MIN_Version, ST_MIN_ST_ID));
           INSERT INTO @ST_MIN_Stage_Minimum
           SELECT i.ST_MIN_ST_ID,
                  i.Metadata_ST_MIN,
                  i.ST_MIN_UTL_ID,
                  ROW_NUMBER() OVER (PARTITION BY i.ST_MIN_ST_ID ORDER BY (SELECT 1) ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = 1,
                  @currentVersion = 0
           FROM   @ST_MIN_Stage_Minimum;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.ST_MIN_StatementType = CASE WHEN [MIN].ST_MIN_ST_ID IS NOT NULL THEN 'D' ELSE 'N' END
                   FROM   @ST_MIN_Stage_Minimum AS v
                          LEFT OUTER JOIN
                          [dbo].[ST_MIN_Stage_Minimum] AS [MIN]
                          ON [MIN].ST_MIN_ST_ID = v.ST_MIN_ST_ID
                             AND [MIN].ST_MIN_UTL_ID = v.ST_MIN_UTL_ID
                   WHERE  v.ST_MIN_Version = @currentVersion;
                   INSERT INTO [dbo].[ST_MIN_Stage_Minimum] (ST_MIN_ST_ID, Metadata_ST_MIN, ST_MIN_UTL_ID)
                   SELECT ST_MIN_ST_ID,
                          Metadata_ST_MIN,
                          ST_MIN_UTL_ID
                   FROM   @ST_MIN_Stage_Minimum
                   WHERE  ST_MIN_Version = @currentVersion
                          AND ST_MIN_StatementType IN ('N');
               END
       END