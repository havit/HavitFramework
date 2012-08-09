/*
DECLARE @Array IntArray
SET @Array = NULL
SELECT Value FROM dbo.IntArrayToTable(@Array)

SELECT dbo.IntArrayAggregate(NULL)
*/

declare @p3 dbo.IntArray
set @p3=convert(dbo.IntArray,0x020000000100000002000000)
exec sp_executesql N'DELETE FROM dbo.Uzivatel_Role WHERE UzivatelID = @UzivatelID; INSERT INTO dbo.Uzivatel_Role (UzivatelID, RoleID) SELECT @UzivatelID AS UzivatelID, Value AS RoleID FROM dbo.IntArrayToTable(@Role); ',N'@Role IntArray,@UzivatelID int',@Role=@p3,@UzivatelID=1
exec sp_executesql N'DELETE FROM dbo.Uzivatel_Role WHERE UzivatelID = @UzivatelID; INSERT INTO dbo.Uzivatel_Role (UzivatelID, RoleID) SELECT @UzivatelID AS UzivatelID, Value AS RoleID FROM dbo.IntArrayToTable(@Role); ',N'@Role IntArray,@UzivatelID int',@Role=NULL,@UzivatelID=2
