IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PCConfigurator')
BEGIN
    CREATE DATABASE PCConfigurator;
END
GO

USE PCConfigurator;
GO


DROP TABLE IF EXISTS ConfigurationStorage;
DROP TABLE IF EXISTS ConfigurationRAM;
DROP TABLE IF EXISTS Configurations;
DROP TABLE IF EXISTS Storage;
DROP TABLE IF EXISTS GPUs;
DROP TABLE IF EXISTS RAM;
DROP TABLE IF EXISTS Motherboards;
DROP TABLE IF EXISTS Processors;
DROP TABLE IF EXISTS Manufacturers;
GO

CREATE TABLE Manufacturers (
    ManufacturerId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Country NVARCHAR(50),
    Website NVARCHAR(200)
);
GO

CREATE TABLE Processors (
    ProcessorId INT PRIMARY KEY IDENTITY(1,1),
    ManufacturerId INT NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Socket NVARCHAR(50) NOT NULL,
    Cores INT NOT NULL,
    Frequency DECIMAL(4,2) NOT NULL, -- ÃÃö
    Price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Processors_Manufacturers FOREIGN KEY (ManufacturerId) 
        REFERENCES Manufacturers(ManufacturerId) ON DELETE CASCADE
);
GO

CREATE TABLE Motherboards (
    MotherboardId INT PRIMARY KEY IDENTITY(1,1),
    ManufacturerId INT NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Socket NVARCHAR(50) NOT NULL,
    Chipset NVARCHAR(50) NOT NULL,
    RAMType NVARCHAR(20) NOT NULL,
    MaxRAM INT NOT NULL, -- ÃÁ
    Price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Motherboards_Manufacturers FOREIGN KEY (ManufacturerId) 
        REFERENCES Manufacturers(ManufacturerId) ON DELETE CASCADE
);
GO

CREATE TABLE RAM (
    RAMId INT PRIMARY KEY IDENTITY(1,1),
    ManufacturerId INT NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Type NVARCHAR(20) NOT NULL,
    Capacity INT NOT NULL, -- ÃÁ
    Frequency INT NOT NULL, -- ÌÃö
    Price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_RAM_Manufacturers FOREIGN KEY (ManufacturerId) 
        REFERENCES Manufacturers(ManufacturerId) ON DELETE CASCADE
);
GO

CREATE TABLE GPUs (
    GPUId INT PRIMARY KEY IDENTITY(1,1),
    ManufacturerId INT NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Memory INT NOT NULL, -- ÃÁ
    MemoryType NVARCHAR(20) NOT NULL, -- GDDR6, GDDR6X
    PowerConsumption INT NOT NULL, -- Âò
    Price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_GPUs_Manufacturers FOREIGN KEY (ManufacturerId) 
        REFERENCES Manufacturers(ManufacturerId) ON DELETE CASCADE
);
GO

CREATE TABLE Storage (
    StorageId INT PRIMARY KEY IDENTITY(1,1),
    ManufacturerId INT NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Type NVARCHAR(20) NOT NULL,
    Capacity INT NOT NULL, -- ÃÁ
    Interface NVARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Storage_Manufacturers FOREIGN KEY (ManufacturerId) 
        REFERENCES Manufacturers(ManufacturerId) ON DELETE CASCADE
);
GO

CREATE TABLE Configurations (
    ConfigurationId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    ProcessorId INT NOT NULL,
    MotherboardId INT NOT NULL,
    GPUId INT NULL, -- nullable, òàê êàê âñòðîåííàÿ ãðàôèêà
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(10,2) NOT NULL DEFAULT 0,
    Description NVARCHAR(500),
    CONSTRAINT FK_Configurations_Processors FOREIGN KEY (ProcessorId) 
        REFERENCES Processors(ProcessorId),
    CONSTRAINT FK_Configurations_Motherboards FOREIGN KEY (MotherboardId) 
        REFERENCES Motherboards(MotherboardId),
    CONSTRAINT FK_Configurations_GPUs FOREIGN KEY (GPUId) 
        REFERENCES GPUs(GPUId)
);
GO

CREATE TABLE ConfigurationRAM (
    ConfigurationRAMId INT PRIMARY KEY IDENTITY(1,1),
    ConfigurationId INT NOT NULL,
    RAMId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_ConfigurationRAM_Configurations FOREIGN KEY (ConfigurationId) 
        REFERENCES Configurations(ConfigurationId) ON DELETE CASCADE,
    CONSTRAINT FK_ConfigurationRAM_RAM FOREIGN KEY (RAMId) 
        REFERENCES RAM(RAMId),
    CONSTRAINT CHK_ConfigurationRAM_Quantity CHECK (Quantity > 0)
);
GO

CREATE TABLE ConfigurationStorage (
    ConfigurationStorageId INT PRIMARY KEY IDENTITY(1,1),
    ConfigurationId INT NOT NULL,
    StorageId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_ConfigurationStorage_Configurations FOREIGN KEY (ConfigurationId) 
        REFERENCES Configurations(ConfigurationId) ON DELETE CASCADE,
    CONSTRAINT FK_ConfigurationStorage_Storage FOREIGN KEY (StorageId) 
        REFERENCES Storage(StorageId),
    CONSTRAINT CHK_ConfigurationStorage_Quantity CHECK (Quantity > 0)
);
GO
