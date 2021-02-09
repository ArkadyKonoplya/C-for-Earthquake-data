/****************************************************************************************************************************************
* ScriptName: sp_GetAllLocations_V1.0
* Coder : Bibhu Gautam, Sarah Price, Biswash Lamsal, Sarah Duplan
* Date: 2021-02-01

Vers     Date                   Coder       Comments
1.0      2021-02-01             Bibhu       Initial
1.1      2021-02-02             Bibhu       Put the select statement for table joins
****************************************************************************************************************************************/
exec sp_GetAllLocations

USE [DB_Ursula]
GO

IF OBJECT_ID('sp_GetAllLocations', 'P') IS NOT NULL

DROP PROCEDURE [dbo].[sp_GetAllLocations]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GetAllLocations]
 
AS  

BEGIN TRANSACTION
BEGIN TRY

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
  
SET NOCOUNT ON  
SET ANSI_WARNINGS OFF  


DECLARE @LocationID INT

select 
E.Latitude,
E.Longitude,
E.Depth
FROM tbl_Earthquake E
Left JOIN tbl_Location L on L.EarthquakeFK = E.EarthquakeID


END TRY

BEGIN CATCH

DECLARE @ErMessage NVARCHAR(MAX),
        @ErSeverity INT,
		@ErState INT

SELECT @ErMessage = ERROR_MESSAGE(), @ErSeverity = ERROR_SEVERITY(), @ErState = ERROR_STATE()

IF @@TRANCOUNT > 0
BEGIN
ROLLBACK TRANSACTION

END
RAISERROR(@ErMessage,@ErSeverity,@ErState)

END CATCH

IF @@TRANCOUNT > 0
BEGIN
COMMIT TRANSACTION
END
GO