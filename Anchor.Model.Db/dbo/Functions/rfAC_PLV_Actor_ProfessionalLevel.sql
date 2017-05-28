CREATE FUNCTION [dbo].[rfAC_PLV_Actor_ProfessionalLevel]
(@id INT, @value TINYINT, @changed DATETIME)
RETURNS TINYINT
AS
BEGIN
    RETURN (CASE WHEN EXISTS (SELECT @value
                              WHERE  @value = (SELECT   TOP 1 pre.AC_PLV_PLV_ID
                                               FROM     [dbo].[AC_Actor_ProfessionalLevel] AS pre
                                               WHERE    pre.AC_PLV_AC_ID = @id
                                                        AND pre.AC_PLV_ChangedAt < @changed
                                               ORDER BY pre.AC_PLV_ChangedAt DESC))
                      OR EXISTS (SELECT @value
                                 WHERE  @value = (SELECT   TOP 1 fol.AC_PLV_PLV_ID
                                                  FROM     [dbo].[AC_Actor_ProfessionalLevel] AS fol
                                                  WHERE    fol.AC_PLV_AC_ID = @id
                                                           AND fol.AC_PLV_ChangedAt > @changed
                                                  ORDER BY fol.AC_PLV_ChangedAt ASC)) THEN 1 ELSE 0 END);
END