CREATE DATABASE Projects;

CREATE SCHEMA project0;

CREATE TABLE project0.Orders
(
	OrderID INT NOT NULL IDENTITY PRIMARY KEY,
	UserID INT NOT NULL,
	LocationID INT NOT NULL,
	TotalCost MONEY NOT NULL CHECK(TotalCost < 500.00),
	OrderDescription NVARCHAR(MAX) NOT NULL,
	Time DATETIME2 NOT NULL
);


CREATE TABLE project0.Users
(
	UserID INT NOT NULL IDENTITY PRIMARY KEY,
	FirstName NVARCHAR(100) NOT NULL,
	LastName NVARCHAR(100) NOT NULL,
	DefaultLocation INT NOT NULL	
);

ALTER TABLE project0.Users
	ADD Password NVARCHAR(100) NOT NULL DEFAULT 'password';

ALTER TABLE project0.Orders
	ADD CONSTRAINT Fk_Orders_UserID FOREIGN KEY (UserID) REFERENCES project0.Users(UserID);

DROP TABLE project0.Locations;

CREATE TABLE project0.Locations
(
	LocationID INT NOT NULL IDENTITY PRIMARY KEY,
	LocationDescription NVARCHAR(100) NOT NULL,
	Inventory NVARCHAR(100) NOT NULL,
	Menu NVARCHAR(200)
); 

ALTER TABLE project0.Orders
	ADD CONSTRAINT FK_Orders_LocationID FOREIGN KEY (LocationID) REFERENCES project0.Locations (LocationID);

ALTER TABLE project0.Orders
	DROP CONSTRAINT FK_Orders_LocationID;

ALTER TABLE project0.Users
	ADD CONSTRAINT Fk_Users_DefaultLocation FOREIGN KEY (DefaultLocation) REFERENCES project0.Locations(LocationID);

ALTER TABLE project0.Users
	DROP CONSTRAINT Fk_Users_DefaultLocation;

--ALTER TABLE project0.Locations
--	ADD Menu NVARCHAR(200) NOT NULL;

--ALTER TABLE project0.Locations
--	DROP COLUMN Menu;

--ALTER TABLE project0.Locations
--	DROP COLUMN Inventory;

--ALTER TABLE project0.Locations
--	ADD Inventory NVARCHAR(100) NOT NULL;


INSERT INTO project0.Locations (LocationDescription, Menu, Inventory) VALUES
	('Galleria', 'Small,Medium,Large/Hand Tossed,Thin Crust,Deep-Dish/Pepperoni,Canadian Bacon,Sausage,Mushrooms,Black Olives,Green Peppers,Onions', '50,50,50,50,50,50,50');

INSERT INTO project0.Locations (LocationDescription, Menu, Inventory) VALUES
	('Heights', 'Small,Medium,Large/Hand Tossed,Thin Crust,Deep-Dish/Pepperoni,Canadian Bacon,Sausage,Mushrooms,Black Olives,Green Peppers,Onions', '50,50,50,50,50,50,50');

INSERT INTO project0.Locations (LocationDescription, Menu, Inventory) VALUES
	('Meyerland', 'Small,Medium,Large/Hand Tossed,Thin Crust,Deep-Dish/Pepperoni,Canadian Bacon,Sausage,Mushrooms,Black Olives,Green Peppers,Onions', '50,50,50,50,50,50,50');


INSERT INTO project0.Users (FirstName, LastName, DefaultLocation) VALUES
	('Will', 'Belt', (SELECT LocationID FROM project0.Locations WHERE LocationDescription = 'Galleria'));



--ALTER TABLE project0.Orders
--	DROP COLUMN OrderDescription;

--ALTER TABLE project0.Orders
--	ADD OrderDescription NVARCHAR(100) NOT NULL;

SELECT * FROM project0.Orders

SELECT * FROM project0.Users;

SELECT * FROM project0.Locations;

