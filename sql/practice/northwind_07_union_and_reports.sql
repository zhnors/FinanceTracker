--We’d like to show a list of the Orders that were made, including the Shipper that was used. 
--Show the OrderID, OrderDate (date only with alias ShortDate), and CompanyName of the Shipper, and sort by OrderID. 
--Show only those rows with an OrderID of less than 10260.

SELECT O.OrderID,
CAST(O.OrderDate AS date) AS ShortDate,
S.CompanyName
FROM Orders O INNER JOIN Shippers S
ON O.ShipVia = S.ShipperID
WHERE O.OrderID < 10260
ORDER BY O.OrderID


--3.Create a report that shows the  ContactName of customer, TotalSum (alias for the calculated column UnitPrice*Quantity*(1-Discount)) 
--From the Order Details, and Customers table with the discount given on every purchase. Show only VIP customers with TotalSum greater 
--than 10000.

SELECT C.ContactName,
ROUND(SUM(OD.Quantity * OD.UnitPrice* (1 - OD.Discount)),2) AS TotalSum
FROM Customers C JOIN Orders O ON C.CustomerID = O.CustomerID
JOIN [Order Details] OD ON O.OrderID = OD.OrderID
GROUP BY C.CustomerID , C.ContactName
HAVING ROUND(SUM(OD.Quantity * OD.UnitPrice* (1 - OD.Discount)),2) > 10000


--7.Create a report that shows the number of employees (alias numEmployees) and number of customers (alias numCompanies) 
--from each city that has employees in it.
--The result should be ordered by numEmployees.

SELECT COUNT(DISTINCT EmployeeID) AS numEmployees,
COUNT(DISTINCT CustomerID) AS numCompanies,
E.City
FROM Employees E LEFT JOIN Customers C ON E.City = C.City
GROUP BY E.City
ORDER BY COUNT(EmployeeID)



--8.Get the lastname and firstname of employee (alias Name), company names and phone numbers (alias Phone) of all employees, customers,
--and suppliers, who are situated in London.
--Add the column (alias Type) to the result set which should specify what type of counterparty (employee, customer, or supplier) it is.

SELECT FirstName +' '+ LastName AS Name,
'Employee' as Type,
HomePhone as Phone,
NULL AS CompanyName
FROM Employees
WHERE City = 'London'
UNION
SELECT ContactName AS Name,
'Customer' as Type,
Phone,
CompanyName
FROM Customers
WHERE City = 'London'
UNION
SELECT ContactName AS Name,
'Supplier' as Type,
Phone,
CompanyName
FROM Suppliers
WHERE City = 'London'

--9.Write the query which would show the list of employees (FirstName and LastName) and their total sales with discount (alias TotalSales)
--Who have sold more than 200 positions of products.

SELECT E.FirstName + ' ' +E.LastName AS FullName,
ROUND(SUM(OD.Quantity * OD.UnitPrice* (1 - OD.Discount)),2) AS TotalSales
FROM Employees E  JOIN Orders O ON E.EmployeeID = O.EmployeeID
JOIN [Order Details] OD ON O.OrderID = OD.OrderID
GROUP BY E.EmployeeID , E.FirstName + ' ' +E.LastName
HAVING SUM(OD.Quantity) > 200

--10.Write the query which would show the names of employees who sell the products of more than 25 suppliers during the 2016 year.

SELECT E.FirstName + ' ' +E.LastName AS FullName
FROM Employees E  JOIN Orders O ON E.EmployeeID = O.EmployeeID
JOIN [Order Details] OD ON O.OrderID = OD.OrderID
JOIN Products P ON OD.ProductID = P.ProductID
WHERE YEAR(O.OrderDate) = 2016
GROUP BY E.EmployeeID , E.FirstName + ' ' +E.LastName
HAVING COUNT(DISTINCT P.SupplierID) > 25
