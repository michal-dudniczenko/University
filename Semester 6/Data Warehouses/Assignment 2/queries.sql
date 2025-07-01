SELECT
	CONCAT(c.LastName, ', ', c.FirstName) AS [Nazwisko, imi�],
	p.CategoryName AS [Kategoria produktu],
	p.[Name] AS [Nazwa produktu],
	o.UnitPrice AS [Cena]
FROM FactOrders o
JOIN DimProduct p ON o.ProductID = p.ProductID
JOIN DimCustomer c ON o.CustomerID = c.CustomerID;

WITH PelnePodsumowanie AS (
SELECT
	YEAR(oh.OrderDate) AS [Rok],
	oh.CustomerID AS [Klient Id],
	CONCAT(p.LastName, ', ', p.FirstName) AS [Nazwisko],
	COUNT(oh.SalesOrderID) AS [Liczba zam.],
	SUM(oh.TotalDue) AS [Kwota],
	DENSE_RANK() OVER (PARTITION BY YEAR(oh.OrderDate) ORDER BY COUNT(oh.SalesOrderID) DESC) AS [Ranking]
FROM Sales.SalesOrderHeader oh
JOIN Sales.Customer c ON oh.CustomerID = c.CustomerID
LEFT JOIN Person.Person p ON p.BusinessEntityID = c.PersonID
GROUP BY YEAR(oh.OrderDate), oh.CustomerID, CONCAT(p.LastName, ', ', p.FirstName)
HAVING SUM(oh.TotalDue) > 130000

)
SELECT [Rok], [Klient Id], [Nazwisko], [Liczba zam.], ROUND([Kwota], 2)
FROM PelnePodsumowanie
WHERE Ranking = 1
ORDER BY [Rok] ASC, [Liczba zam.] DESC, [Kwota] DESC;

SELECT
	CAST(MIN(OrderDate) AS DATE) AS [Pierwsze],
	CAST(MAX(OrderDate) AS DATE) AS [Ostatnie],
	DATEDIFF(DAY, MIN(OrderDate), MAX(OrderDate)) AS [Liczba dni]
FROM Sales.SalesOrderHeader;

SELECT
	CustomerID AS [Klient],
	CAST(MIN(OrderDate) AS DATE) AS [Od],
	CAST(MAX(OrderDate) AS DATE) AS [Do],
	DATEDIFF(DAY, MIN(OrderDate), MAX(OrderDate)) AS [Liczba dni],
	COUNT(SalesOrderID) AS [Liczba zam.]
FROM Sales.SalesOrderHeader
GROUP BY CustomerID
ORDER BY [Klient];

SELECT 
   SalesPersonID AS [Pracownik],
   YEAR(OrderDate) AS [Rok],
   COUNT(*) AS [Liczba],
   SUM(COUNT(*)) OVER (PARTITION BY SalesPersonID ORDER BY YEAR(OrderDate) ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS [Razem]
FROM Sales.SalesOrderHeader
WHERE SalesPersonID IS NOT NULL
GROUP BY SalesPersonID, YEAR(OrderDate)
ORDER BY [Pracownik] ASC, [Rok] ASC;

SELECT 
	YEAR(oh.OrderDate) AS [Year],
	pc.[Name] AS [Category],
	CAST(SUM(od.LineTotal) AS DECIMAL(20, 2)) AS [Kwota],
	COUNT(od.ProductID) AS [Liczba zam�wie�]
FROM Sales.SalesOrderDetail od
JOIN Production.[Product] p ON p.ProductID = od.ProductID
JOIN Sales.SalesOrderHeader oh ON oh.SalesOrderID = od.SalesOrderID
JOIN Production.ProductSubcategory ps ON ps.ProductSubcategoryID = p.ProductSubcategoryID
JOIN Production.ProductCategory pc ON pc.ProductCategoryID = ps.ProductCategoryID
GROUP BY YEAR(oh.OrderDate), pc.[Name]
ORDER BY [Category] ASC, [Year] ASC;

