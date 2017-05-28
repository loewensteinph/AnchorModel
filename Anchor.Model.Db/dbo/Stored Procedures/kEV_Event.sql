CREATE PROCEDURE [dbo].[kEV_Event]
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
            INSERT INTO [dbo].[EV_Event] (Metadata_EV)
            OUTPUT inserted.EV_ID
            SELECT @metadata
            FROM   idGenerator
            OPTION (MAXRECURSION 0);
        END
END