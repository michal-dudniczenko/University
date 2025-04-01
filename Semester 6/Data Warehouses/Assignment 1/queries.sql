SELECT
	SalesOrderID AS "Identyfikator",
	YEAR(OrderDate) AS "Rok",
	ROUND(TotalDue, 2) AS "Kwota"
FROM Sales.SalesOrderHeader
WHERE OrderDate < DATEADD(YEAR, 1, (SELECT TOP 1 OrderDate FROM Sales.SalesOrderHeader ORDER BY 1 ASC));


WITH Zamowienia AS (
	SELECT 
		CustomerID, 
		COUNT(*) AS LiczbaZamowien
	FROM Sales.SalesOrderHeader
	GROUP BY CustomerID
)
SELECT
	CASE
		WHEN LiczbaZamowien < 10 THEN '0-9'
		WHEN LiczbaZamowien <20 THEN '10-19'
		ELSE '20 ...'
	END AS Grupa,
	COUNT(*) AS "Liczba Klientów"
FROM Zamowienia
GROUP BY
	CASE
		WHEN LiczbaZamowien < 10 THEN '0-9'
		WHEN LiczbaZamowien < 20 THEN '10-19'
		ELSE '20 ...'
	END
;

WITH Zamowienia AS (
    SELECT 
        CustomerID, 
        COUNT(*) AS LiczbaZamowien
    FROM Sales.SalesOrderHeader
    GROUP BY CustomerID
)
SELECT 
	'' AS Grupa,
    SUM(CASE WHEN LiczbaZamowien < 10 THEN 1 ELSE 0 END) AS [0-9],
    SUM(CASE WHEN LiczbaZamowien BETWEEN 10 AND 19 THEN 1 ELSE 0 END) AS [10-19],
    SUM(CASE WHEN LiczbaZamowien >= 20 THEN 1 ELSE 0 END) AS [20 ...]
FROM Zamowienia;



SELECT 
	r.Name AS Czynnik,
	COUNT(*) AS Dotyczy
FROM Sales.SalesOrderHeaderSalesReason hr
JOIN Sales.SalesReason r ON r.SalesReasonID = hr.SalesReasonID
GROUP BY r.Name
ORDER BY 2 DESC;
