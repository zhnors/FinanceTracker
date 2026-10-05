--1. Write a query to get Product name and quantity/unit.

SELECT ProductName,
QuantityPerUnit
FROM Products

--2. Write a query to get current Product list (Product ID and name).

SELECT ProductID,
ProductName
FROM Products
WHERE Discontinued = 0

--3. Write a query to get discontinued Product list (Product ID and name).

SELECT ProductID,
ProductName
FROM Products
WHERE Discontinued = 1

--4. Write a query to get most expense and least expensive Product list (name and unit price).

SELECT TOP(1) ProductName,
UnitPrice
FROM Products
ORDER BY UnitPrice DESC

SELECT TOP(1) ProductName,
UnitPrice
FROM Products
ORDER BY UnitPrice 

--5. Write a query to get Product list (id, name, unit price) where current products cost less than $20.

SELECT ProductID,
ProductName,
UnitPrice
FROM Products
WHERE UnitPrice < 20 AND Discontinued = 0

--6. Write a query to get Product list (id, name, unit price) where products cost between $15 and $25.

SELECT ProductID,
ProductName,
UnitPrice
FROM Products
WHERE UnitPrice BETWEEN 15 AND 25

--7. Write a query to get Product list (name, unit price) of above average price.

SELECT ProductID,
ProductName,
UnitPrice
FROM Products
WHERE UnitPrice > (SELECT AVG(UnitPrice) FROM Products)

--8. Write a query to get Product list (name, unit price) of ten most expensive products.

SELECT TOP(10)
ProductName,
UnitPrice
FROM Products
ORDER BY UnitPrice DESC

--9. Write a query to count current and discontinued products.

SELECT Discontinued,
COUNT(ProductID) AS ProductCount
FROM Products
GROUP BY Discontinued

--10. Write a query to get Product list (name, units on order , units in stock) of stock is less than the quantity 
--on order

SELECT ProductName,
UnitsOnOrder,
UnitsInStock
FROM Products
WHERE UnitsInStock < UnitsOnOrder
