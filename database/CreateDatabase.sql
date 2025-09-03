CREATE DATABASE RealEstateDb;
GO
USE RealEstateDb;
GO

IF OBJECT_ID('dbo.PropertyTrace','U') IS NOT NULL DROP TABLE dbo.PropertyTrace;
IF OBJECT_ID('dbo.PropertyImage','U') IS NOT NULL DROP TABLE dbo.PropertyImage;
IF OBJECT_ID('dbo.Property','U') IS NOT NULL DROP TABLE dbo.Property;
IF OBJECT_ID('dbo.Owner','U') IS NOT NULL DROP TABLE dbo.Owner;
GO

CREATE TABLE dbo.Owner (
  IdOwner INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(200) NOT NULL,
  Address NVARCHAR(250) NOT NULL,
  Photo NVARCHAR(2048) NULL,
  Birthday DATETIME2 NULL
);

CREATE TABLE dbo.Property (
  IdProperty INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(200) NOT NULL,
  Address NVARCHAR(250) NOT NULL,
  Price DECIMAL(18,2) NOT NULL,
  CodeInternal NVARCHAR(64) NOT NULL UNIQUE,
  Year INT NOT NULL,
  IdOwner INT NOT NULL
    CONSTRAINT FK_Property_Owner REFERENCES dbo.Owner(IdOwner) ON DELETE NO ACTION
);

CREATE TABLE dbo.PropertyImage (
  IdPropertyImage INT IDENTITY(1,1) PRIMARY KEY,
  IdProperty INT NOT NULL
    CONSTRAINT FK_PropertyImage_Property REFERENCES dbo.Property(IdProperty) ON DELETE CASCADE,
  File NVARCHAR(2048) NOT NULL,
  Enabled BIT NOT NULL DEFAULT(1)
);
CREATE INDEX IX_PropertyImage_Property_Enabled ON dbo.PropertyImage(IdProperty, Enabled);

CREATE TABLE dbo.PropertyTrace (
  IdPropertyTrace INT IDENTITY(1,1) PRIMARY KEY,
  DateSale DATETIME2 NOT NULL,
  Name NVARCHAR(200) NOT NULL,
  Value DECIMAL(18,2) NOT NULL,
  Tax DECIMAL(18,2) NOT NULL,
  IdProperty INT NOT NULL
    CONSTRAINT FK_PropertyTrace_Property REFERENCES dbo.Property(IdProperty) ON DELETE CASCADE
);
CREATE INDEX IX_PropertyTrace_Property_Date ON dbo.PropertyTrace(IdProperty, DateSale);
GO

INSERT INTO dbo.Owner(Name, Address) VALUES ('Owner 1', 'Addr 1');
