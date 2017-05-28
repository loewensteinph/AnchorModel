CREATE FUNCTION [dbo].[rfAC_exclusive_AC_with_ONG_currently]
(@AC_ID_exclusive INT, @AC_ID_with INT, @ONG_ID_currently TINYINT, @changed DATETIME)
RETURNS TINYINT
AS
BEGIN
    RETURN (SELECT COUNT(*)
            FROM   (SELECT   TOP 1 pre.AC_ID_exclusive,
                                   pre.AC_ID_with,
                                   pre.ONG_ID_currently
                    FROM     [dbo].[AC_exclusive_AC_with_ONG_currently] AS pre
                    WHERE    (pre.AC_ID_exclusive = @AC_ID_exclusive
                              OR pre.AC_ID_with = @AC_ID_with)
                             AND pre.AC_exclusive_AC_with_ONG_currently_ChangedAt < @changed
                    ORDER BY pre.AC_exclusive_AC_with_ONG_currently_ChangedAt DESC
                    UNION
                    SELECT   TOP 1 fol.AC_ID_exclusive,
                                   fol.AC_ID_with,
                                   fol.ONG_ID_currently
                    FROM     [dbo].[AC_exclusive_AC_with_ONG_currently] AS fol
                    WHERE    (fol.AC_ID_exclusive = @AC_ID_exclusive
                              OR fol.AC_ID_with = @AC_ID_with)
                             AND fol.AC_exclusive_AC_with_ONG_currently_ChangedAt > @changed
                    ORDER BY fol.AC_exclusive_AC_with_ONG_currently_ChangedAt ASC) AS s
            WHERE  s.AC_ID_exclusive = @AC_ID_exclusive
                   AND s.AC_ID_with = @AC_ID_with
                   AND s.ONG_ID_currently = @ONG_ID_currently);
END