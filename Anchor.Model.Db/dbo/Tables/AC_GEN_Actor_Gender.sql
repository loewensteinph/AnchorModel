CREATE TABLE [dbo].[AC_GEN_Actor_Gender] (
    [AC_GEN_AC_ID]    INT NOT NULL,
    [AC_GEN_GEN_ID]   BIT NOT NULL,
    [Metadata_AC_GEN] INT NOT NULL,
    CONSTRAINT [pkAC_GEN_Actor_Gender] PRIMARY KEY CLUSTERED ([AC_GEN_AC_ID] ASC),
    CONSTRAINT [fk_A_AC_GEN_Actor_Gender] FOREIGN KEY ([AC_GEN_AC_ID]) REFERENCES [dbo].[AC_Actor] ([AC_ID]),
    CONSTRAINT [fk_K_AC_GEN_Actor_Gender] FOREIGN KEY ([AC_GEN_GEN_ID]) REFERENCES [dbo].[GEN_Gender] ([GEN_ID])
);




GO
CREATE TRIGGER [dbo].[it_AC_GEN_Actor_Gender]
    ON [dbo].[AC_GEN_Actor_Gender]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @AC_GEN_Actor_Gender TABLE (
               AC_GEN_AC_ID         INT      NOT NULL,
               Metadata_AC_GEN      INT      NOT NULL,
               AC_GEN_GEN_ID        BIT      NOT NULL,
               AC_GEN_Version       BIGINT   NOT NULL,
               AC_GEN_StatementType CHAR (1) NOT NULL,
               PRIMARY KEY (AC_GEN_Version, AC_GEN_AC_ID));
           INSERT INTO @AC_GEN_Actor_Gender
           SELECT i.AC_GEN_AC_ID,
                  i.Metadata_AC_GEN,
                  i.AC_GEN_GEN_ID,
                  ROW_NUMBER() OVER (PARTITION BY i.AC_GEN_AC_ID ORDER BY (SELECT 1) ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = 1,
                  @currentVersion = 0
           FROM   @AC_GEN_Actor_Gender;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.AC_GEN_StatementType = CASE WHEN [GEN].AC_GEN_AC_ID IS NOT NULL THEN 'D' ELSE 'N' END
                   FROM   @AC_GEN_Actor_Gender AS v
                          LEFT OUTER JOIN
                          [dbo].[AC_GEN_Actor_Gender] AS [GEN]
                          ON [GEN].AC_GEN_AC_ID = v.AC_GEN_AC_ID
                             AND [GEN].AC_GEN_GEN_ID = v.AC_GEN_GEN_ID
                   WHERE  v.AC_GEN_Version = @currentVersion;
                   INSERT INTO [dbo].[AC_GEN_Actor_Gender] (AC_GEN_AC_ID, Metadata_AC_GEN, AC_GEN_GEN_ID)
                   SELECT AC_GEN_AC_ID,
                          Metadata_AC_GEN,
                          AC_GEN_GEN_ID
                   FROM   @AC_GEN_Actor_Gender
                   WHERE  AC_GEN_Version = @currentVersion
                          AND AC_GEN_StatementType IN ('N');
               END
       END