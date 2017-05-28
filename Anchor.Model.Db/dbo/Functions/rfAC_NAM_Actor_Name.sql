CREATE FUNCTION [dbo].[rfAC_NAM_Actor_Name]
(@id INT, @value VARCHAR (42), @changed DATETIME)
RETURNS TINYINT
AS
BEGIN
    RETURN (CASE WHEN EXISTS (SELECT @value
                              WHERE  @value = (SELECT   TOP 1 pre.AC_NAM__ID
                                               FROM     [dbo].[AC_Actor_Name] AS pre
                                               WHERE    pre.AC_NAM_AC_ID = @id
                                                        AND pre.AC_NAM_ChangedAt < @changed
                                               ORDER BY pre.AC_NAM_ChangedAt DESC))
                      OR EXISTS (SELECT @value
                                 WHERE  @value = (SELECT   TOP 1 fol.AC_NAM__ID
                                                  FROM     [dbo].[AC_Actor_Name] AS fol
                                                  WHERE    fol.AC_NAM_AC_ID = @id
                                                           AND fol.AC_NAM_ChangedAt > @changed
                                                  ORDER BY fol.AC_NAM_ChangedAt ASC)) THEN 1 ELSE 0 END);
END