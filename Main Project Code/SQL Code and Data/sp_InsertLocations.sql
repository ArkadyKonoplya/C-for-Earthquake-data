/****************************************************************************************************************************************
* ScriptName: sp_InsertLocations_V1.0
* Coder : Bibhu Gautam, Sarah Price, Biswash Lamsal, Sarah Duplan
* Date: 2021-02-01

Vers     Date                   Coder       Comments
1.0      2021-02-01             Bibhu       Initial
****************************************************************************************************************************************/

USE [DB_Ursula]
GO

IF OBJECT_ID('sp_InsertLocations', 'P') IS NOT NULL

DROP PROCEDURE [dbo].[sp_InsertLocations]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_InsertLocations]

		@Lat DECIMAL(10,8),
		@Lng DECIMAL(11,8)
 
AS  

BEGIN TRANSACTION
BEGIN TRY

  
SET NOCOUNT ON  
SET ANSI_WARNINGS OFF  

INSERT INTO tbl_Location(Latitude,Longitude)
SELECT @Lat, @Lng


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