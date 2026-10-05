--1. Create a report showing the first and last name of all sales representatives who are from  Seattle or Redmond.  
select CONCAT(FirstName,' ',LastName) as FullName
from Employees
where City IN('Seattle','Redmond') AND Title LIKE 'Sales Representative'

--2. Create a report that shows the company name, contact title, city and country of all  customers 
--in Mexico or in any city in Spain except Madrid.  

SELECT CompanyName,
ContactTitle,
City,
Country
FROM Customers
WHERE Country IN ('Mexico','Spain') AND City NOT LIKE 'Madrid'


--1.Show first and last names of the employees as well as the count of orders each of them have received during the year 1997.  

SELECT COUNT(O.OrderID) AS AmountOfOrders,
E.FirstName+' '+E.LastName AS FullName
FROM Orders O INNER JOIN Employees E 
ON O.EmployeeID = E.EmployeeID
WHERE YEAR(O.OrderDate) = 1997
GROUP BY E.FirstName+' '+E.LastName,E.EmployeeID
ORDER BY COUNT(O.OrderID)

--2.Show first and last names of the employees as well as the count of their orders shipped after required date during the year 1997.  

SELECT COUNT(O.OrderID) AS AmountOfOrders,
E.FirstName+' '+E.LastName AS FullName
FROM Orders O INNER JOIN Employees E 
ON O.EmployeeID = E.EmployeeID
WHERE YEAR(O.OrderDate) = 1997 AND O.RequiredDate < O.ShippedDate
GROUP BY E.FirstName+' '+E.LastName
ORDER BY COUNT(O.OrderID)

--3.Create a report showing the information about employees and orders, whenever they had orders or not.    
SELECT E.FirstName+' '+E.LastName AS FullName,
COUNT(O.OrderID) AS AmountOfOrders
FROM Employees E LEFT JOIN Orders O
ON E.EmployeeID = O.EmployeeID
GROUP BY E.FirstName+' '+E.LastName
ORDER BY COUNT(O.OrderID) DESC

--1. Show the list of French customers’ names who used to order non-French products.

SELECT C.ContactName,
C.Country,
S.Country
FROM Customers C INNER JOIN Orders O
ON C.CustomerID = O.CustomerID
INNER JOIN [Order Details] OD
ON O.OrderID = OD.OrderID
INNER JOIN Products P
ON OD.ProductID = P.ProductID
INNER JOIN Suppliers S
ON P.SupplierID = S.SupplierID
WHERE C.Country LIKE 'France' AND S.Country NOT LIKE 'France'

--2. Show the list of suppliers, products and its category.

SELECT S.CompanyName,
P.ProductName,
C.CategoryName
FROM Suppliers S INNER JOIN Products P
ON S.SupplierID =P.SupplierID
INNER JOIN Categories C
ON P.CategoryID = C.CategoryID

--3. Create a report that shows all  information about suppliers and products.   

SELECT *
FROM Suppliers S INNER JOIN Products P
ON S.SupplierID =P.SupplierID
