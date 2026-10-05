--3.Show the names of products, that have the biggest price.

SELECT ProductName ,UnitPrice
FROM Products
WHERE UnitPrice = (
	SELECT MAX(UnitPrice)
	FROM Products)

--1.Show first and last names of the employees who have the biggest freight.

SELECT E.LastName,
	E.FirstName,
	O.Freight
FROM Employees E 
JOIN Orders O 
	ON E.EmployeeID = O.EmployeeID
WHERE O.Freight = (
	SELECT MAX(O2.Freight)
	FROM Employees E2 JOIN Orders O2 
		ON E2.EmployeeID = O2.EmployeeID)

--2.Show first, last names of the employees, their freight who have the freight bigger then avarage.

SELECT CONCAT(E.LastName,' ',E.FirstName) AS FullName,
	O.Freight
FROM Employees E 
JOIN Orders O 
	ON E.EmployeeID = O.EmployeeID
WHERE O.Freight > (
	SELECT ROUND(AVG(O2.Freight),2)
	FROM Employees E2 
	JOIN Orders O2 
		ON E2.EmployeeID = O2.EmployeeID)

--5.Show the name of supplier  who delivered the cheapest product.

SELECT S.ContactName 
FROM Suppliers S 
JOIN Products P
	ON S.SupplierID = P.SupplierID
WHERE P.UnitPrice = (
	SELECT  MIN(P2.UnitPrice)
	FROM Suppliers S2 JOIN Products P2
	ON S2.SupplierID = P2.SupplierID)

--6.Show the name of the category in which the average price of  a certain product is greater than the grand average in the whole stock.

SELECT C.CategoryName
FROM Categories C JOIN Products P
ON C.CategoryID = P.CategoryID
GROUP BY C.CategoryName
HAVING AVG(P.UnitPrice) > (
SELECT AVG(P2.UnitPrice)
FROM Products P2)


--9.Show customers whose maximum freight level is less than the average for all customers.

SELECT C.ContactName 
FROM Customers C JOIN Orders O
	ON C.CustomerID = O.CustomerID
GROUP BY C.ContactName
HAVING MAX(O.Freight) <		(
	SELECT AVG(O2.Freight)
	FROM Customers C2 JOIN Orders O2
	ON C2.CustomerID = O2.CustomerID)

--10.Show the categories of products for which the average discount is higher than the average discount for all products

SELECT C.CategoryName 
FROM Categories C JOIN Products P ON C.CategoryID = P.CategoryID
JOIN [Order Details] OD ON P.ProductID = OD.ProductID
GROUP BY C.CategoryName
HAVING AVG(OD.Discount) > (
SELECT AVG(OD2.Discount)
FROM [Order Details] OD2)

--12.Show first and last names of employees who shipped orders in cities of USA.

SELECT DISTINCT CONCAT(E.LastName,' ',E.FirstName) AS FullName
FROM Employees E JOIN Orders O 
ON E.EmployeeID = O.EmployeeID
WHERE O.ShipCountry = 'USA'

--15.Show the name of customers that prefer to order non-domestic products.

SELECT DISTINCT C.ContactName
FROM Customers C JOIN Orders O ON C.CustomerID = O.CustomerID
JOIN [Order Details] OD ON O.OrderID = OD.OrderID
JOIN Products P ON OD.ProductID = P.ProductID
JOIN Suppliers S ON P.SupplierID = S.SupplierID
WHERE C.Country <> S.Country

--25.Create a report that shows all companies by name that sell products in the Dairy Products category.

SELECT DISTINCT S.CompanyName
FROM Suppliers S JOIN Products P ON S.SupplierID = P.SupplierID
JOIN Categories C ON P.CategoryID = C.CategoryID
WHERE C.CategoryName = 'Dairy Products'

--HW
--1.Create a report that shows the product name and supplier id for all products supplied by Exotic Liquids, Grandma Kelly's Homestead, 
--and Tokyo Traders.

SELECT ProductName,
	SupplierID
FROM Products
WHERE SupplierID IN (
	SELECT SupplierID
	FROM Suppliers
	WHERE CompanyName IN('Exotic Liquids', 'Grandma Kelly''s Homestead', 'Tokyo Traders'))

--3.Create a report that shows the orders placed by all the customers excluding the customers who belongs to London city.

SELECT OrderID
FROM Orders
WHERE CustomerID IN (
	SELECT CustomerID
	FROM Customers
	WHERE City <> 'London')

--5.Create a report that shows all the orders where the employee’s city and order’s ship city are same.

SELECT O.OrderID
FROM Orders O
JOIN Employees E ON O.EmployeeID = E.EmployeeID
WHERE O.ShipCity = E.City

--17.Show the info about orders, that contain the cheapest products from USA.

SELECT O.OrderID
FROM Orders O
JOIN [Order Details] OD ON O.OrderID = OD.OrderID
WHERE OD.UnitPrice =(
	SELECT MIN(OD.UnitPrice)
	FROM [Order Details] OD 
	JOIN Products P ON OD.ProductID = P.ProductID
	JOIN Suppliers S ON P.SupplierID = S.SupplierID
	WHERE S.Country = 'USA')

	SELECT  P.ProductName , MIN(P.UnitPrice)
	FROM Suppliers S
	JOIN Products P ON S.SupplierID = P.SupplierID
	WHERE S.Country = 'USA' 
	GROUP BY P.ProductName
	


--18.Show the info about customers that prefer to order meat products and never order drinks.

SELECT C.ContactName
FROM Customers C
WHERE EXISTS (
	SELECT 1
	FROM Orders O
	JOIN [Order Details] OD ON O.OrderId = OD.OrderId
	JOIN Products P ON OD.ProductID = P.ProductID
	JOIN Categories CAT ON P.CategoryID = CAT.CategoryID
	WHERE C.CustomerID = O.CustomerID
		AND CAT.CategoryName = 'Meat/Poultry')
	AND NOT EXISTS (
	SELECT 1
	FROM Orders O
	JOIN [Order Details] OD ON O.OrderId = OD.OrderId
	JOIN Products P ON OD.ProductID = P.ProductID
	JOIN Categories CAT ON P.CategoryID = CAT.CategoryID
	WHERE C.CustomerID = O.CustomerID
		AND CAT.CategoryName = 'Beverages')	

--Додатково
--10.Write the query that should return the EmployeeID,  OrderID, and OrderDate. The criteria for the report is that the order must be the last 
--for each employee (maximum OrderDate)
--ВАР1

SELECT E.EmployeeID,
	O.OrderID,
	O.OrderDate
FROM Employees E
JOIN Orders O ON E.EmployeeID = O.EmployeeID
WHERE O.OrderDate = (
	SELECT MAX(O2.OrderDate)
	FROM Orders O2
	WHERE  O2.EmployeeID = E.EmployeeID )
ORDER BY E.EmployeeID

--1.Janet Leverling, one of the salespeople, has come to you with a request. She thinks that she accidentally double-entered a line item on an order,
--with a different ProductID, but the same quantity. She remembers that the quantity was 60 or more. Show all the OrderIDs that match this, 
--in order of OrderID.

SELECT DISTINCT O.OrderID
FROM Employees E
JOIN Orders O ON E.EmployeeID = O.EmployeeID
JOIN [Order Details] OD ON O.OrderID = OD.OrderID
JOIN [Order Details] OD2 ON OD.OrderID = OD2.OrderID
WHERE E.FirstName = 'Janet' 
	AND E.LastName = 'Leverling' 
	AND OD.Quantity  >= 60
	AND OD.Quantity = OD2.Quantity
	AND OD.ProductID < OD2.ProductID
ORDER BY O.OrderID

--6.For the category 'Dairy Products' get the list of products sold and the total sales amount including discount (alias ProductSales) 
--during the 1st quarter of 2016 year. 
--8.Some salespeople have more orders arriving late than others. Maybe they're not following up on the order process, and need more training. 
--Andrew, the VP of sales, has been doing some more thinking some more about the problem of late orders. He realizes that just looking at the number of 
--orders arriving late for each salesperson isn't a good idea. It needs to be compared against the total number of orders per salesperson.
 
