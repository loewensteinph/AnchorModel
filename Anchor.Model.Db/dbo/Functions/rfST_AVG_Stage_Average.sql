CREATE FUNCTION [dbo].[rfST_AVG_Stage_Average]
(@id INT, @value TINYINT, @changed DATETIME)
RETURNS TINYINT
AS
BEGIN
    RETURN (CASE WHEN EXISTS (SELECT @value
                              WHERE  @value = (SELECT   TOP 1 pre.ST_AVG_UTL_ID
                                               FROM     [dbo].[ST_Stage_Average] AS pre
                                               WHERE    pre.ST_AVG_ST_ID = @id
                                                        AND pre.ST_AVG_ChangedAt < @changed
                                               ORDER BY pre.ST_AVG_ChangedAt DESC))
                      OR EXISTS (SELECT @value
                                 WHERE  @value = (SELECT   TOP 1 fol.ST_AVG_UTL_ID
                                                  FROM     [dbo].[ST_Stage_Average] AS fol
                                                  WHERE    fol.ST_AVG_ST_ID = @id
                                                           AND fol.ST_AVG_ChangedAt > @changed
                                                  ORDER BY fol.ST_AVG_ChangedAt ASC)) THEN 1 ELSE 0 END);
END