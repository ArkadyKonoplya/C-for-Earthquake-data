
/****************************************************************************************************************************************
* ScriptName: DB_Ursula_DML_Insert_V1.2
* Coder : Bibhu Gautam, Sarah Price, Biswash Lamsal, Sarah Duplan
* Date: 2021-01-31

Vers     Date                   Coder       Comments
1.0      2021-01-31             Bibhu       Initial
1.1      2021-02-02             Bibhu       Wrote a script to alter datatype of imported data
1.2      2021-02-02             Bibhu       changed the script to execute sp_GetAllEarthquake
****************************************************************************************************************************************/
USE master
GO

IF EXISTS(SELECT * FROM sys.databases WHERE name ='DB_Ursula')
BEGIN
USE DB_Ursula

exec sp_GetAllEarthquakes


END

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name ='DB_Ursula')
BEGIN
SELECT 'Please promote the highest version of DB_Ursula_DDL_CREATE before attempting to load DDL'
END
 