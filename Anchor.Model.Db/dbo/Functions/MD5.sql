﻿CREATE FUNCTION [dbo].[MD5]
(@binaryData VARBINARY (MAX) NULL)
RETURNS VARBINARY (16)
AS
 EXTERNAL NAME [Anchor].[Utilities].[HashMD5]

