CREATE TABLE [dbo].[AC_PLV_Actor_ProfessionalLevel] (
    [AC_PLV_AC_ID]     INT      NOT NULL,
    [AC_PLV_PLV_ID]    TINYINT  NOT NULL,
    [AC_PLV_ChangedAt] DATETIME NOT NULL,
    [Metadata_AC_PLV]  INT      NOT NULL,
    CONSTRAINT [pkAC_PLV_Actor_ProfessionalLevel] PRIMARY KEY CLUSTERED ([AC_PLV_AC_ID] ASC, [AC_PLV_ChangedAt] DESC),
    CONSTRAINT [fk_A_AC_PLV_Actor_ProfessionalLevel] FOREIGN KEY ([AC_PLV_AC_ID]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [fk_K_AC_PLV_Actor_ProfessionalLevel] FOREIGN KEY ([AC_PLV_PLV_ID]) REFERENCES [dbo].[PLV_ProfessionalLevel] ([PLV_ID])
);




GO
CREATE TRIGGER [dbo].[it_AC_PLV_Actor_ProfessionalLevel]
    ON [dbo].[AC_PLV_Actor_ProfessionalLevel]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @AC_PLV_Actor_ProfessionalLevel TABLE (
               AC_PLV_AC_ID         INT      NOT NULL,
               Metadata_AC_PLV      INT      NOT NULL,
               AC_PLV_ChangedAt     DATETIME NOT NULL,
               AC_PLV_PLV_ID        TINYINT  NOT NULL,
               AC_PLV_Version       BIGINT   NOT NULL,
               AC_PLV_StatementType CHAR (1) NOT NULL,
               PRIMARY KEY (AC_PLV_Version, AC_PLV_AC_ID));
           INSERT INTO @AC_PLV_Actor_ProfessionalLevel
           SELECT i.AC_PLV_AC_ID,
                  i.Metadata_AC_PLV,
                  i.AC_PLV_ChangedAt,
                  i.AC_PLV_PLV_ID,
                  DENSE_RANK() OVER (PARTITION BY i.AC_PLV_AC_ID ORDER BY i.AC_PLV_ChangedAt ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = max(AC_PLV_Version),
                  @currentVersion = 0
           FROM   @AC_PLV_Actor_ProfessionalLevel;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.AC_PLV_StatementType = CASE WHEN [PLV].AC_PLV_AC_ID IS NOT NULL THEN 'D' WHEN [dbo].[rfAC_PLV_Actor_ProfessionalLevel](v.AC_PLV_AC_ID, v.AC_PLV_PLV_ID, v.AC_PLV_ChangedAt) = 1 THEN 'R' ELSE 'N' END
                   FROM   @AC_PLV_Actor_ProfessionalLevel AS v
                          LEFT OUTER JOIN
                          [dbo].[AC_PLV_Actor_ProfessionalLevel] AS [PLV]
                          ON [PLV].AC_PLV_AC_ID = v.AC_PLV_AC_ID
                             AND [PLV].AC_PLV_ChangedAt = v.AC_PLV_ChangedAt
                             AND [PLV].AC_PLV_PLV_ID = v.AC_PLV_PLV_ID
                   WHERE  v.AC_PLV_Version = @currentVersion;
                   INSERT INTO [dbo].[AC_PLV_Actor_ProfessionalLevel] (AC_PLV_AC_ID, Metadata_AC_PLV, AC_PLV_ChangedAt, AC_PLV_PLV_ID)
                   SELECT AC_PLV_AC_ID,
                          Metadata_AC_PLV,
                          AC_PLV_ChangedAt,
                          AC_PLV_PLV_ID
                   FROM   @AC_PLV_Actor_ProfessionalLevel
                   WHERE  AC_PLV_Version = @currentVersion
                          AND AC_PLV_StatementType IN ('N', 'R');
               END
       END