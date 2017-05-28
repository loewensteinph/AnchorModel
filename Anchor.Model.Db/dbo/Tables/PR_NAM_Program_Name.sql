CREATE TABLE [dbo].[PR_NAM_Program_Name] (
    [PR_NAM_PR_ID]        INT          NOT NULL,
    [PR_NAM_Program_Name] VARCHAR (42) NOT NULL,
    [Metadata_PR_NAM]     INT          NOT NULL,
    CONSTRAINT [pkPR_NAM_Program_Name] PRIMARY KEY CLUSTERED ([PR_NAM_PR_ID] ASC),
    CONSTRAINT [fkPR_NAM_Program_Name] FOREIGN KEY ([PR_NAM_PR_ID]) REFERENCES [dbo].[PR_Program] ([PR_ID])
);




GO
CREATE TRIGGER [dbo].[it_PR_NAM_Program_Name]
    ON [dbo].[PR_NAM_Program_Name]
    INSTEAD OF INSERT
    AS BEGIN
           SET NOCOUNT ON;
           DECLARE @maxVersion AS INT;
           DECLARE @currentVersion AS INT;
           DECLARE @PR_NAM_Program_Name TABLE (
               PR_NAM_PR_ID         INT          NOT NULL,
               Metadata_PR_NAM      INT          NOT NULL,
               PR_NAM_Program_Name  VARCHAR (42) NOT NULL,
               PR_NAM_Version       BIGINT       NOT NULL,
               PR_NAM_StatementType CHAR (1)     NOT NULL,
               PRIMARY KEY (PR_NAM_Version, PR_NAM_PR_ID));
           INSERT INTO @PR_NAM_Program_Name
           SELECT i.PR_NAM_PR_ID,
                  i.Metadata_PR_NAM,
                  i.PR_NAM_Program_Name,
                  ROW_NUMBER() OVER (PARTITION BY i.PR_NAM_PR_ID ORDER BY (SELECT 1) ASC),
                  'X'
           FROM   inserted AS i;
           SELECT @maxVersion = 1,
                  @currentVersion = 0
           FROM   @PR_NAM_Program_Name;
           WHILE (@currentVersion < @maxVersion)
               BEGIN
                   SET @currentVersion = @currentVersion + 1;
                   UPDATE v
                   SET    v.PR_NAM_StatementType = CASE WHEN [NAM].PR_NAM_PR_ID IS NOT NULL THEN 'D' ELSE 'N' END
                   FROM   @PR_NAM_Program_Name AS v
                          LEFT OUTER JOIN
                          [dbo].[PR_NAM_Program_Name] AS [NAM]
                          ON [NAM].PR_NAM_PR_ID = v.PR_NAM_PR_ID
                             AND [NAM].PR_NAM_Program_Name = v.PR_NAM_Program_Name
                   WHERE  v.PR_NAM_Version = @currentVersion;
                   INSERT INTO [dbo].[PR_NAM_Program_Name] (PR_NAM_PR_ID, Metadata_PR_NAM, PR_NAM_Program_Name)
                   SELECT PR_NAM_PR_ID,
                          Metadata_PR_NAM,
                          PR_NAM_Program_Name
                   FROM   @PR_NAM_Program_Name
                   WHERE  PR_NAM_Version = @currentVersion
                          AND PR_NAM_StatementType IN ('N');
               END
       END