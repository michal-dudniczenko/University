CREATE TABLE DimCustomer (
	CustomerID INT,
	FirstName NVARCHAR(50) NOT NULL, 
	LastName NVARCHAR(50) NOT NULL, 
	TerritoryName NVARCHAR(50) NOT NULL, 
	CountryRegionCode NVARCHAR(3) NOT NULL, 
	[Group] NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_dimCustomer PRIMARY KEY (CustomerID),
);

CREATE TABLE DimProduct (
	ProductID INT,
	[Name] NVARCHAR(50) NOT NULL, 
	ListPrice MONEY NOT NULL, 
	Color NVARCHAR(15), 
	SubCategoryName NVARCHAR(50) NOT NULL, 
	CategoryName NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_dimProduct PRIMARY KEY (ProductID),
);

CREATE TABLE FactOrders (
	ProductID INT NOT NULL, 
	CustomerID INT, 
	OrderDate DATETIME, 
	ShipDate DATETIME, 
	OrderQty SMALLINT, 
	UnitPrice MONEY, 
	UnitPriceDiscount MONEY, 
	LineTotal DECIMAL(38, 6),

	CONSTRAINT fk_orders_product FOREIGN KEY (ProductID) REFERENCES DimProduct(ProductID),
	CONSTRAINT fk_orders_customer FOREIGN KEY (CustomerID) REFERENCES DimCustomer(CustomerID),
);

INSERT INTO DimCustomer (CustomerID, FirstName, LastName, TerritoryName, CountryRegionCode, [Group])
SELECT
	c.CustomerID,
	p.FirstName,
	p.LastName,
	t.[Name],
	t.CountryRegionCode,
	t.[Group]

FROM AdventureWorks2022.Sales.Customer c
JOIN AdventureWorks2022.Person.Person p ON c.PersonID = p.BusinessEntityID
JOIN AdventureWorks2022.Sales.SalesTerritory t ON c.TerritoryID = t.TerritoryID;

INSERT INTO DimProduct (ProductID, [Name], ListPrice, Color, SubCategoryName, CategoryName)
SELECT
	p.ProductID,
	p.[Name],
	p.ListPrice,
	p.Color,
	ps.[Name],
	pc.[Name]
FROM AdventureWorks2022.Production.[Product] p
JOIN AdventureWorks2022.Production.ProductSubcategory ps ON p.ProductSubcategoryID = ps.ProductSubcategoryID
JOIN AdventureWorks2022.Production.ProductCategory pc ON  ps.ProductCategoryID = pc.ProductCategoryID;

INSERT INTO FactOrders (ProductID, CustomerID, OrderDate, ShipDate, OrderQty, UnitPrice, UnitPriceDiscount, LineTotal)
SELECT
	d.ProductID,
	h.CustomerID,
	h.OrderDate,
	h.ShipDate,
	d.OrderQty,
	d.UnitPrice,
	d.UnitPriceDiscount,
	d.LineTotal
FROM AdventureWorks2022.Sales.SalesOrderDetail d
JOIN AdventureWorks2022.Sales.SalesOrderHeader h ON d.SalesOrderID = h.SalesOrderID;