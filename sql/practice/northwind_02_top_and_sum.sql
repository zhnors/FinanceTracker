--1. Write a query to get most expense and least expensive Product list (name and unit 
--price).
--2. Write a query to get Product list (name, unit price) of ten most expensive products.
--3. For each employee that served the order (identified by EmployeeID), calculate a total 
--Freight

SELECT TOP(1)
ProductName,
UnitPrice
FROM Products
ORDER BY UnitPrice

SELECT TOP(1)
ProductName,
UnitPrice
FROM Products
ORDER BY UnitPrice DESC

SELECT TOP(10)
ProductName,
UnitPrice
FROM Products
ORDER BY UnitPrice DESC

SELECT EmployeeID,
SUM(Freight) AS TotalFreight
FROM Orders
GROUP BY EmployeeID
ORDER BY EmployeeID
