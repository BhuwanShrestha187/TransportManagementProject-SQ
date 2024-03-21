-- Create Database and Use
CREATE DATABASE TMS;
USE TMS;

DROP DATABASE TMS; 

-- Create Clients Table
CREATE TABLE Clients (
    ClientID INT PRIMARY KEY AUTO_INCREMENT,
    ClientName VARCHAR(255) NOT NULL,
    IsActive INT NOT NULL DEFAULT 0
);


-- Create Users Table
CREATE TABLE Users (
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    Username VARCHAR(255) PRIMARY KEY,
    Password VARCHAR(255),
    Email VARCHAR(255) NOT NULL,
    UserType INT NOT NULL
);

-- Create Orders Table
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY AUTO_INCREMENT,
    ClientID INT,
    ClientName VARCHAR(255),
    Origin VARCHAR(255) NOT NULL,
    Destination VARCHAR(255) NOT NULL,
    JobType VARCHAR(255) NOT NULL,
    Quantity INT NOT NULL,
    VanType VARCHAR(255) NOT NULL,
    OrderCreationDate DATETIME NOT NULL,
    OrderAcceptedDate DATETIME DEFAULT NULL, 
    InvoiceGenerated INT DEFAULT 0,
    IsCompleted INT DEFAULT 0,
    OrderCompletedDate DATETIME  DEFAULT NULL,
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID)
);


-- Create Carriers Table
CREATE TABLE Carriers (
    CarrierID INT PRIMARY KEY AUTO_INCREMENT,
    CarrierName VARCHAR(255) NOT NULL,
    FTLRate DOUBLE NOT NULL,
    LTLRate DOUBLE NOT NULL,
    reefCharge DOUBLE NOT NULL,
    IsActive INT NOT NULL
);

-- Create CarrierCity Table
CREATE TABLE CarrierCity (
    CarrierID INT NOT NULL,
    DepotCity VARCHAR(255) NOT NULL,
    FTLAval INT NOT NULL,
    LTLAval INT NOT NULL,
    FTLRate DOUBLE NOT NULL,
    LTLRate DOUBLE NOT NULL,
    reefCharge DOUBLE NOT NULL,
    FOREIGN KEY (CarrierID) REFERENCES Carriers(CarrierID) ON DELETE CASCADE
);

-- Create Trips Table
CREATE TABLE Trips (
    TripID INT PRIMARY KEY AUTO_INCREMENT,
    OrderID INT NOT NULL,
    CarrierID INT NOT NULL,
    OriginCity VARCHAR(255) NOT NULL,
    DestinationCity VARCHAR(255) NOT NULL,
    TotalDistance INT NOT NULL,
    TotalTime DATETIME NOT NULL,
    JobType VARCHAR(255) NOT NULL,
    VanType VARCHAR(255) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (CarrierID) REFERENCES Carriers(CarrierID) ON DELETE CASCADE
);

-- Create Invoices Table
CREATE TABLE Invoices (
    OrderID INT NOT NULL,
    ClientName VARCHAR(255) NOT NULL,
    Origin VARCHAR(255) NOT NULL,
    Destination VARCHAR(255) NOT NULL,
    TotalAmount DOUBLE NOT NULL,
    TotalDistance INT NOT NULL,
    TotalDays INT NOT NULL,
    CompletedDate DATETIME NOT NULL
);

-- Create Route Table
CREATE TABLE Route (
	RouteID int PRIMARY KEY AUTO_INCREMENT,
    Destination VARCHAR(255) NOT NULL,
    Distance INT,
    Time DECIMAL (5, 2),
    West VARCHAR(255) NOT NULL,
    East VARCHAR(255) NOT NULL
);

-- Create Rates Table
CREATE TABLE Rates (
    FTLRate DOUBLE,
    LTLRate DOUBLE
);

-- Drop Tables (only if necessary for testing)
-- DROP TABLE Users;
-- DROP TABLE Orders;
-- DROP TABLE Invoices;
DROP TABLE Route;
-- DROP TABLE Carriers;
-- DROP TABLE CarrierCity;
-- DROP TABLE Clients;
-- DROP TABLE Trips;
-- DROP TABLE Rates;

SELECT * FROM Orders;
SELECT * FROM Clients;