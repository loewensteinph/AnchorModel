CREATE PROCEDURE [dbo].[kPR_Program] (
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
            )INSERT INTO [dbo].[PR_Program] (
                Metadata_PR
            )
            OUTPUT
                inserted.PR_ID
            SELECT
                @metadata
            FROM
                idGenerator
            OPTION (MAXRECURSION 0);
        END
    END