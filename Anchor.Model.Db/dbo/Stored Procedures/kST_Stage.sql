CREATE PROCEDURE [dbo].[kST_Stage] (
        @requestedNumberOfIdentities BIGINT,
        @metadata INT
    ) AS
    BEGIN
        SET NOCOUNT ON;
        IF @requestedNumberOfIdentities > 0
        BEGIN
            WITH idGenerator (idNumber) AS (
                SELECT
                    1
                UNION ALL
                SELECT
                    idNumber + 1
                FROM
                    idGenerator
                WHERE
                    idNumber < @requestedNumberOfIdentities
            )INSERT INTO [dbo].[ST_Stage] (
                Metadata_ST
            )
            OUTPUT
                inserted.ST_ID
            SELECT
                @metadata
            FROM
                idGenerator
            OPTION (MAXRECURSION 0);
        END
    END