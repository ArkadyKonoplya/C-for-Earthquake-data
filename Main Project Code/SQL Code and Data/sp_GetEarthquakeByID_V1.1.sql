/****************************************************************************************************************************************
* ScriptName: sp_GetEarthquakeByID_V1.0
* Coder : Bibhu Gautam, Sarah Price, Biswash Lamsal, Sarah Duplan
* Date: 2021-01-21

Vers     Date                   Coder       Comments
1.0      2021-01-31             Bibhu       Initial
****************************************************************************************************************************************/

USE [DB_Ursula]
GO

IF OBJECT_ID('sp_GetEarthquakeByID', 'P') IS NOT NULL

DROP PROCEDURE [dbo].[sp_GetEarthquakeByID]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GetEarthquakeByID]

@EarthquakeID INT
 
AS  

BEGIN TRANSACTION
BEGIN TRY

  
SET NOCOUNT ON  
SET ANSI_WARNINGS OFF  


SELECT
E.EarthquakeID,
E.EventDate,
E.Latitude,
E.Longitude, 
E.Depth,
E.Magnitude,
E.MagnitudeType,
E.Place,
E.Region
FROM tbl_Earthquake E Where EarthquakeID = @EarthquakeID


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