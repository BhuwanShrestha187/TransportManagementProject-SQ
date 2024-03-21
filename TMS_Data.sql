USE TMS; 

INSERT INTO Users (FirstName, LastName, Username, Password, Email, UserType) 
VALUES 
('Bhuwan', 'Shrestha', 'Buyer', 'Buyer24', 'bhuwan@gmail.com', 0),
('Bhuwan', 'Shrestha', 'Planner', 'Planner24', 'bhuwan@gmail.com', 1),
('Bhuwan', 'Shrestha', 'Admin', 'Admin24', 'bhuwan@gmail.com', 2);

SELECT * FROM USERS; 



-- Insert random values into Carriers table
INSERT INTO Carriers (CarrierName, FTLRate, LTLRate, reefCharge, IsActive)
VALUES 
  ('Planet Express', 4.985, 0.2995, 0.08, 1), 
   ('Schooner''s', 4.985, 0.2995, 0.08, 1), 
    ('Tillman Transport', 4.985, 0.2995, 0.08, 1), 
     ('We Haul', 4.985, 0.2995, 0.08, 1);
  
-- Insert random values into CarrierCity table
INSERT INTO CarrierCity (CarrierID, DepotCity, FTLAval, LTLAval, FTLRate, LTLRate, reefCharge)
VALUES 
  (1, 'Windsor', 50, 640, 5.21, 0.3621, 0.08),
  (1, 'Hamilton', 50, 640, 5.21, 0.3621, 0.08),
  (1, 'Oshawa', 50, 640, 5.21, 0.3621, 0.08),
  (1, 'Belleville', 50, 640, 5.21, 0.3621, 0.08),
  (1, 'Ottawa', 50, 640, 5.21, 0.3621, 0.08),
  (2, 'London', 18, 98, 5.05, 0.3434, 0.07),
  (2, 'Toronto', 18, 98, 5.05, 0.3434, 0.07),
  (2, 'Kingston', 18, 98, 5.05, 0.3434, 0.07),
  (3, 'Windsor', 24, 35, 5.11, 0.3012, 0.09),
  (3, 'London', 18, 45, 5.11, 0.3012, 0.09),
  (3, 'Hamilton', 18, 45, 5.11, 0.3012, 0.09),
  (4, 'Ottawa', 11, 0, 5.2, 0, 0.065),
  (4, 'Toronto', 11, 0, 5.2, 0, 0.065);


  
  -- Inserting sample values into the Rates table
INSERT INTO Rates (FTLRate, LTLRate)
VALUES
  (4.985, 0.2995);
  

  INSERT INTO Route (Destination, Distance, Time, West, East)
  VALUES
	('Windsor', 191, 2.5, 'END', 'London'), 
    ('London', 128, 1.75, 'Windsor', 'Hamilton'), 
    ('Hamilton', 68, 1.25, 'London', 'Toronto'), 
    ('Toronto', 60, 1.3, 'Hamilton', 'Oshawa'), 
    ('Oshawa', 134, 1.65, 'Tonront', 'Belleville'), 
    ('Kingston', 196, 2.5, 'Belleville', 'Ottawa'), 
    ('Ottawa', null, null, 'Kingston', 'END'); 
  
  SELECT * FROM CarrierCity;
 
rates