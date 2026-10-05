--1.Show the total ordering sum calculated for each country of customer.

SELECT C.Country,
ROUND(SUM(OD.Quantity * OD.UnitPrice* (1 - OD.Discount)),2) AS TotalOrderingSum
FROM Customers C JOIN Orders O ON C.CustomerID = O.CustomerID
JOIN [Order Details] OD ON O.OrderID = OD.OrderID
GROUP BY C.Country

--2.Show the list of product categories along with total ordering sums calculated for the orders made for the products of each category,
--during the year 1997.

SELECT C.CategoryName,
ROUND(SUM(OD.Quantity * OD.UnitPrice* (1-OD.Discount)),2) AS TotalOrderingSum
FROM Orders O JOIN [Order Details] OD ON O.OrderID = OD.OrderID
JOIN Products P ON OD.ProductID = P.ProductID
JOIN Categories C ON P.CategoryID = C.CategoryID 
WHERE YEAR(O.OrderDate) = 1997
GROUP BY C.CategoryName

--3.Show the list of product names along with unit prices and the history of unit prices taken from the orders 
--(show ‘Product name – Unit price – Historical price’). The duplicate records should be eliminated. 
--If no orders were made for a certain product, then the result for this product should look like ‘Product name – Unit price – NULL’.
--Sort the list by the product name.

SELECT DISTINCT P.ProductName,
P.UnitPrice,
OD.UnitPrice AS HistoricalPrice
FROM Products P LEFT JOIN [Order Details] OD ON P.ProductID = OD.ProductID
ORDER BY P.ProductName
