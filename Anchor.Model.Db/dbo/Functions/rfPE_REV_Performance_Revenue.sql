CREATE FUNCTION [dbo].[rfPE_REV_Performance_Revenue]
(@id INT, @value VARBINARY (16), @changed DATETIME)
RETURNS TINYINT
AS
BEGIN
    RETURN (CASE WHEN EXISTS (SELECT @value
                              WHERE  @value = (SELECT   TOP 1 pre.PE_REV__ID
                                               FROM     [dbo].[PE_Performance_Revenue] AS pre
                                               WHERE    pre.PE_REV_PE_ID = @id
                                                        AND pre.PE_REV_ChangedAt < @changed
                                               ORDER BY pre.PE_REV_ChangedAt DESC))
                      OR EXISTS (SELECT @value
                                 WHERE  @value = (SELECT   TOP 1 fol.PE_REV__ID
                                                  FROM     [dbo].[PE_Performance_Revenue] AS fol
                                                  WHERE    fol.PE_REV_PE_ID = @id
                                                           AND fol.PE_REV_ChangedAt > @changed
                                                  ORDER BY fol.PE_REV_ChangedAt ASC)) THEN 1 ELSE 0 END);
END