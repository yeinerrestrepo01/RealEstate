-- SQL Server database creation script
-- Run in SQL Server (SSMS) to create schema
CREATE DATABASE RealEstateDb;
GO
USE RealEstateDb;
GO

IF OBJECT_ID('dbo.Properties', 'U') IS NOT NULL DROP TABLE dbo.Properties;
IF OBJECT_ID('dbo.PropertyImages', 'U') IS NOT NULL DROP TABLE dbo.PropertyImages;
GO

CREATE TABLE dbo.Properties(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Code NVARCHAR(32) NOT NULL UNIQUE,
    Title NVARCHAR(160) NOT NULL,
    Description NVARCHAR(4000) NULL,
    Address NVARCHAR(200) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(2) NOT NULL,
    ZipCode NVARCHAR(10) NOT NULL,
    Country NVARCHAR(3) NOT NULL DEFAULT('USA'),
    Bedrooms INT NOT NULL,
    Bathrooms DECIMAL(5,2) NOT NULL,
    AreaSqFt INT NOT NULL,
    YearBuilt INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL DEFAULT(0),
    Stories INT NOT NULL DEFAULT(1),
    ParkingSpaces INT NOT NULL DEFAULT(0),
    HasHeating BIT NOT NULL DEFAULT(0),
    HasCooling BIT NOT NULL DEFAULT(0),
    LotSizeSqFt DECIMAL(18,2) NOT NULL DEFAULT(0),
    CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME())
);
GO

CREATE TABLE dbo.PropertyImages(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PropertyId UNIQUEIDENTIFIER NOT NULL,
    Url NVARCHAR(2048) NOT NULL,
    IsCover BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_PropertyImages_Properties FOREIGN KEY (PropertyId) REFERENCES dbo.Properties(Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_PropertyImages_PropertyId_IsCover ON dbo.PropertyImages(PropertyId, IsCover);
GO

-- Sample seed
INSERT INTO dbo.Properties (Id, Code, Title, Address, City, State, ZipCode, Country, Bedrooms, Bathrooms, AreaSqFt, YearBuilt, Price, Status, Stories, ParkingSpaces, HasHeating, HasCooling, LotSizeSqFt)
VALUES
(NEWID(), 'NYC-0001', 'Cozy Apartment in Manhattan', '123 Main St', 'New York', 'NY', '10001', 'USA', 2, 1.50, 900, 1995, 950000, 1, 1, 0, 1, 1, 0),
(NEWID(), 'LA-0002', 'House in LA', '1 Ocean Dr', 'Los Angeles', 'CA', '90001', 'USA', 3, 2.00, 1500, 2001, 1200000, 1, 2, 2, 1, 1, 5000.0);
GO
