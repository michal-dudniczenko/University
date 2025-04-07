--SELECT
--	[SalesPersonID],
--	[CustomerID],
--    COALESCE(FORMAT([2011], '0.00'), 'brak') AS [2011],
--	COALESCE(FORMAT([2012], '0.00'), 'brak') AS [2012],
--	COALESCE(FORMAT([2013], '0.00'), 'brak') AS [2013],
--	COALESCE(FORMAT([2014], '0.00'), 'brak') AS [2014]
--FROM (
--SELECT 
--	oh.SalesPersonID AS [SalesPersonID],
--	oh.CustomerID AS [CustomerID],
--	YEAR(oh.OrderDate) AS [Rok],
--	oh.SubTotal AS [Suma]
--FROM Sales.SalesOrderHeader oh
--WHERE SalesPersonID IS NOT NULL
--) AS Src
--PIVOT (
--	SUM(Suma) FOR Rok IN ([2011], [2012], [2013], [2014]) 
--) AS Pvt
--ORDER BY [SalesPersonID] ASC, [CustomerID] ASC;


--SELECT
--	[Name],
--	[CustomerID],
--	ROUND([2012], 2) AS [2012], 
--	ROUND([2013], 2) AS [2013], 
--	ROUND([2014], 2) AS [2014]
--FROM (
--	SELECT
--		COALESCE(CONCAT(p.LastName, ', ', p.FirstName), s.[Name]) AS [Name],
--		oh.CustomerID AS [CustomerID],
--		YEAR(oh.OrderDate) AS [Rok],
--		oh.SubTotal AS [Suma]
--	FROM Sales.SalesOrderHeader oh
--	JOIN Sales.Customer c ON c.CustomerID = oh.CustomerID
--	LEFT JOIN Person.Person p ON p.BusinessEntityID = c.PersonID
--	LEFT JOIN Sales.Store s ON s.BusinessEntityID = c.StoreID
--) AS Src
--PIVOT (
--	AVG([Suma]) FOR [Rok] IN ([2012], [2013], [2014])
--) AS Pvt
--ORDER BY [CustomerID] ASC;


--SELECT 
--    COALESCE(CONCAT(p.LastName, ', ', p.FirstName), s.[Name]) AS [Name],
--    oh.CustomerID AS [CustomerID],
--    ROUND(AVG(CASE WHEN YEAR(oh.OrderDate) = 2012 THEN oh.SubTotal END), 2) AS [2012],
--    ROUND(AVG(CASE WHEN YEAR(oh.OrderDate) = 2013 THEN oh.SubTotal END), 2) AS [2013],
--    ROUND(AVG(CASE WHEN YEAR(oh.OrderDate) = 2014 THEN oh.SubTotal END), 2) AS [2014]
--FROM Sales.SalesOrderHeader oh
--JOIN Sales.Customer c ON c.CustomerID = oh.CustomerID
--LEFT JOIN Person.Person p ON p.BusinessEntityID = c.PersonID
--LEFT JOIN Sales.Store s ON s.BusinessEntityID = c.StoreID
--GROUP BY COALESCE(CONCAT(p.LastName, ', ', p.FirstName), s.[Name]), oh.CustomerID
--ORDER BY [CustomerID] ASC;


--SELECT 
--	[ProdID],
--	[Name],
--	[2013],
--	[2014],
--	ROUND([2014] - [2013], 2) AS [Ró¿nica]
--FROM (
--	SELECT
--		th.ProductID AS [ProdID],
--		p.[Name] AS [Name],
--		YEAR(th.TransactionDate) AS [Rok], 
--		th.ActualCost AS [Cost]
--	FROM Production.TransactionHistory th
--	JOIN Production.[Product] p ON p.ProductID = th.ProductID
--) AS Src
--PIVOT (
--	SUM([Cost]) FOR [Rok] IN ([2013], [2014])
--) AS Pvt
--ORDER BY [Ró¿nica] DESC;


--SELECT DISTINCT 
--    oh.SalesPersonID AS [pracID],
--    CONCAT(p.LastName, ', ', p.FirstName) AS [nazwisko],
--    YEAR(oh.OrderDate) AS [rokZam],
--    ROUND(SUM(SubTotal) OVER (PARTITION BY oh.SalesPersonID, YEAR(oh.OrderDate)), 2) AS [Kwota],
--    COUNT(oh.SalesOrderID) OVER (PARTITION BY oh.SalesPersonID, YEAR(oh.OrderDate)) AS [Liczba zamówieñ]
--FROM sales.SalesOrderHeader oh
--JOIN HumanResources.Employee e ON oh.SalesPersonID = e.BusinessEntityID
--JOIN Person.Person p ON e.BusinessEntityID = p.BusinessEntityID
--WHERE oh.SalesPersonID IS NOT NULL;


--SELECT 
--    oh.SalesPersonID AS [pracID],
--    FORMAT(oh.OrderDate, 'yyyy-MM') AS [Miesi¹c],
--    COUNT(oh.SalesOrderID) AS [Liczba zamówieñ],
--    SUM(COUNT(oh.SalesOrderID)) OVER (
--        PARTITION BY oh.SalesPersonID 
--        ORDER BY FORMAT(oh.OrderDate, 'yyyy-MM') 
--        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
--    ) AS [Dotychczas]
--FROM Sales.SalesOrderHeader oh
--WHERE oh.SalesPersonID IS NOT NULL
--GROUP BY oh.SalesPersonID, FORMAT(oh.OrderDate, 'yyyy-MM')
--ORDER BY [pracID], [Miesi¹c];


--SELECT
--	s.[Name] AS [Nazwa sklepu],
--	st.[Name] AS [Terytorium],
--	st.[Group] AS [Grupa],
--	CONCAT(p.BusinessEntityID, ' - ', p.LastName, ', ', p.FirstName) AS [Sprzedawca],
--	ROUND(SUM(oh.TotalDue), 2) AS [Kwota]
--FROM Sales.SalesOrderHeader oh
--JOIN Sales.SalesTerritory st ON st.TerritoryID = oh.TerritoryID
--JOIN Person.Person p ON p.BusinessEntityID = oh.SalesPersonID
--JOIN Sales.Customer c ON c.CustomerID = oh.CustomerID
--JOIN Sales.Store s ON s.BusinessEntityID = c.StoreID
--WHERE st.[Name] IN ('France', 'Germany')
--GROUP BY GROUPING SETS (
--	(s.[Name]),
--	(st.[Name]),
--	(st.[Group]),
--	(CONCAT(p.BusinessEntityID, ' - ', p.LastName, ', ', p.FirstName))
--);


--SELECT
--	[Rok],
--	[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
--FROM (
--	SELECT
--		YEAR(oh.OrderDate) AS [Rok],
--		MONTH(OH.OrderDate) AS [Miesi¹c],
--		oh.SalesOrderID AS [SaleID]
--	FROM Sales.SalesOrderHeader oh
--) AS Src
--PIVOT (
--	COUNT([SaleID]) FOR [Miesi¹c] IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
--) AS Pvt
--ORDER BY [Rok] ASC;


--SELECT * 
--FROM (
--    SELECT 
--        oh.CustomerID AS [Klient],
--        oh.SubTotal AS [Kwota],
--        ROW_NUMBER() OVER (PARTITION BY oh.CustomerID ORDER BY oh.OrderDate DESC) AS [rank]
--    FROM Sales.SalesOrderHeader oh
--) Src
--PIVOT (
--    MAX(Kwota) FOR [rank] IN ([1], [2], [3])
--) Pvt
--ORDER BY Klient;


--SELECT * 
--FROM (
--    SELECT 
--        oh.CustomerID AS [KlientID],
--		CONCAT(p.LastName, ', ', p.FirstName) AS [Osoba],
--        oh.SalesOrderNumber [Nr],
--        ROW_NUMBER() OVER (PARTITION BY oh.CustomerID ORDER BY oh.OrderDate DESC) AS [rank]
--    FROM Sales.SalesOrderHeader oh
--	JOIN Sales.Customer c ON c.CustomerID = oh.CustomerID
--	JOIN Person.Person p ON p.BusinessEntityID = c.PersonID
--) Src
--PIVOT (
--    MAX([Nr]) FOR [rank] IN ([1], [2], [3], [4])
--) Pvt
--ORDER BY [Osoba] ASC; 