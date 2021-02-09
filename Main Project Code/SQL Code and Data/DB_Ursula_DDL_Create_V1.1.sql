/****************************************************************************************************************************************
* ScriptName: DB_Ursula_DDL_Create_V1.0
* Coder : Bibhu Gautam, Sarah Price, Biswash Lamsal, Sarah Duplan
* Date: 2021-01-21

Vers     Date                   Coder       Comments
1.0      2021-01-21             Bibhu       Initial
1.1      2021-01-22             Bibhu       Created location and shape table
1.2      2021-01-30             Bibhu       Deleted Province table and replaced "province"column with "Region"
1.3      2021-02-02             Bibhu       Added EarthquakeFK altered columns on location table
****************************************************************************************************************************************/

USE master
GO

IF EXISTS(SELECT * FROM sys.databases WHERE name ='DB_Ursula')
DROP DATABASE DB_Ursula
GO
CREATE DATABASE DB_Ursula
GO
USE DB_Ursula

CREATE TABLE tbl_Earthquake
(
EarthquakeID INT IDENTITY(1,1),
EventDate DATETIME NOT NULL,
Latitude DECIMAL(10,8) NOT NULL,
Longitude DECIMAL(11,8) NOT NULL,
Depth DECIMAL(4,1) NOT NULL,
Magnitude DECIMAL(2,1) NOT NULL,
MagnitudeType NVARCHAR(MAX) NULL,
Place NVARCHAR(MAX) NULL,
Region NVARCHAR(MAX) NULL,
)


CREATE TABLE tbl_Location
(
LocationID INT IDENTITY(1,1),
EarthquakeFK INT NOT NULL,
Lat DECIMAL(10,8) NOT NULL,
Long DECIMAL(11,8) NOT NULL,
Depths DECIMAL(4,1) NOT NULL
)

CREATE TABLE tbl_Shape
(
ShapeID INT IDENTITY(1,1),
LocationFK INT NOT NULL,
Ordinal INT NOT NULL,
HexLineColour VARCHAR(10) NOT NULL,
HexFillColour VARCHAR(10) NULL,
IsPolyline BIT NOT NULL,
IsPolygon BIT NOT NULL,
Magnitude DECIMAL(2,1) NOT NULL,
)

CREATE TABLE tbl_OverLay
(
OverlayID INT IDENTITY(1,1),
FilePath VARCHAR(MAX), /* path where image file is located */
DataPoint VARCHAR(MAX), /* will decide later what the datapoint is going to be, could be name of map */
LocationFK INT NULL,
BoundsLat DECIMAL(11,8),  /*how far north or south*/
BoundsLong DECIMAL(11,8)  /*how far east or west*/
)



ALTER TABLE tbl_Earthquake 
ADD PRIMARY KEY (EarthquakeID)

ALTER TABLE tbl_Location 
ADD PRIMARY KEY (LocationID)

ALTER TABLE tbl_Shape
ADD PRIMARY KEY (ShapeID)

ALTER TABLE tbl_Overlay 
ADD PRIMARY KEY (OverlayID)

ALTER TABLE tbl_Location
ADD FOREIGN KEY (EarthquakeFK) REFERENCES tbl_Earthquake(EarthquakeID);

ALTER TABLE tbl_Shape 
ADD FOREIGN KEY (LocationFK) REFERENCES tbl_Location(LocationID);

ALTER TABLE tbl_Overlay
ADD FOREIGN KEY (LocationFK) REFERENCES tbl_Location(LocationID);
