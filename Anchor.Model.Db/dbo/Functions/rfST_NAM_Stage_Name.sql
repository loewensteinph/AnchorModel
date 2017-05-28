CREATE FUNCTION [dbo].[rfST_NAM_Stage_Name]
(@id INT, @value VARCHAR (42), @changed DATETIME)
RETURNS TINYINT
AS
BEGIN
    RETURN (CASE WHEN EXISTS (SELECT @value
                              WHERE  @value = (SELECT   TOP 1 pre.ST_NAM_Stage_Name
                                               FROM     [dbo].[ST_NAM_Stage_Name] AS pre
                                               WHERE    pre.ST_NAM_ST_ID = @id
                                                        AND pre.ST_NAM_ChangedAt < @changed
                                               ORDER BY pre.ST_NAM_ChangedAt DESC))
                      OR EXISTS (SELECT @value
                                 WHERE  @value = (SELECT   TOP 1 fol.ST_NAM_Stage_Name
                                                  FROM     [dbo].[ST_NAM_Stage_Name] AS fol
                                                  WHERE    fol.ST_NAM_ST_ID = @id
                                                           AND fol.ST_NAM_ChangedAt > @changed
                                                  ORDER BY fol.ST_NAM_ChangedAt ASC)) THEN 1 ELSE 0 END);
END