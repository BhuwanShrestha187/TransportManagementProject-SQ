CREATE DATABASE ContractMarketPlace;
Use ContractMarketPlace; 

CREATE TABLE Contract ( 
ClientName VARCHAR(255) NOT NULL, 
JobType VARCHAR(255) NOT NULL, 
Quantity INT NOT NULL, 
Origin VARCHAR(255) NOT NULL, 
Destination VARCHAR(255) NOT NULL, 
VanType VARCHAR(255) NOT NULL
);