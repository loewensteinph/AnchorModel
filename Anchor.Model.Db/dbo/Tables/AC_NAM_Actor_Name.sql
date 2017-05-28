CREATE TABLE [dbo].[AC_NAM_Actor_Name] (
    [AC_NAM_AC_ID]      INT          NOT NULL,
    [AC_NAM_Actor_Name] VARCHAR (42) NOT NULL,
    [AC_NAM_ChangedAt]  DATETIME     NOT NULL,
    [Metadata_AC_NAM]   INT          NOT NULL,
    CONSTRAINT [pkAC_NAM_Actor_Name] PRIMARY KEY CLUSTERED ([AC_NAM_AC_ID] ASC, [AC_NAM_ChangedAt] DESC),
    CONSTRAINT [fkAC_NAM_Actor_Name] FOREIGN KEY ([AC_NAM_AC_ID]) REFERENCES [dbo].[AC_Actor] ([AC_ID])
);




GO
CREATE TRIGGER [dbo].[it_AC_NAM_Actor_Name]
    ON [dbo].[AC_NAM_Actor_Name]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @AC_NAM_Actor_Name TABLE (
               AC_NAM_AC_ID         INT          NOT NULL,
               Metadata_AC_NAM      INT          NOT NULL,
               AC_NAM_ChangedAt     DATETIME     NOT NULL,
               AC_NAM_Actor_Name    VARCHAR (42) NOT NULL,
               AC_NAM_Version       BIGINT       NOT NULL,
               AC_NAM_StatementType CHAR (1)     NOT NULL,
               PRIMARY KEY (AC_NAM_Version, AC_NAM_AC_ID));
           INSERT INTO @AC_NAM_Actor_Name
           SELECT i.AC_NAM_AC_ID,
                  i.Metadata_AC_NAM,
                  i.AC_NAM_ChangedAt,
                  i.AC_NAM_Actor_Name,
                  DENSE_RANK() OVER (PARTITION BY i.AC_NAM_AC_ID ORDER BY i.AC_NAM_ChangedAt ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = max(AC_NAM_Version),
                  @currentVersion = 0
           FROM   @AC_NAM_Actor_Name;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.AC_NAM_StatementType = CASE WHEN [NAM].AC_NAM_AC_ID IS NOT NULL THEN 'D' WHEN [dbo].[rfAC_NAM_Actor_Name](v.AC_NAM_AC_ID, v.AC_NAM_Actor_Name, v.AC_NAM_ChangedAt) = 1 THEN 'R' ELSE 'N' END
                   FROM   @AC_NAM_Actor_Name AS v
                          LEFT OUTER JOIN
                          [dbo].[AC_NAM_Actor_Name] AS [NAM]
                          ON [NAM].AC_NAM_AC_ID = v.AC_NAM_AC_ID
                             AND [NAM].AC_NAM_ChangedAt = v.AC_NAM_ChangedAt
                             AND [NAM].AC_NAM_Actor_Name = v.AC_NAM_Actor_Name
                   WHERE  v.AC_NAM_Version = @currentVersion;
                   INSERT INTO [dbo].[AC_NAM_Actor_Name] (AC_NAM_AC_ID, Metadata_AC_NAM, AC_NAM_ChangedAt, AC_NAM_Actor_Name)
                   SELECT AC_NAM_AC_ID,
                          Metadata_AC_NAM,
                          AC_NAM_ChangedAt,
                          AC_NAM_Actor_Name
                   FROM   @AC_NAM_Actor_Name
                   WHERE  AC_NAM_Version = @currentVersion
                          AND AC_NAM_StatementType IN ('N', 'R');
               END
       END