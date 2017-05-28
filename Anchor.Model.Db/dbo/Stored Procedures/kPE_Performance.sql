CREATE PROCEDURE [dbo].[kPE_Performance]
@requestedNumberOfIdentities BIGINT, @metadata INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @requestedNumberOfIdentities > 0
        BEGIN
            WITH idGenerator (idNumber)
            AS   (SELECT 1
                  UNION ALL
                  SELECT idNumber + 1
                  FROM   idGenerator
                  WHERE  idNumber < @requestedNumberOfIdentities)
            INSERT INTO [dbo].[PE_Performance] (Metadata_PE)
            OUTPUT inserted.PE_ID
            SELECT @metadata
            FROM   idGenerator
            OPTION (MAXRECURSION 0);
        END
END