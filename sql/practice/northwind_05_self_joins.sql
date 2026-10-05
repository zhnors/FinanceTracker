--1. Show the list of French customers’ names who are working in the same cities.

SELECT A.ContactName,
B.ContactName
FROM Customers A INNER JOIN Customers B
ON A.City = B.City
WHERE A.Country ='France' AND B.Country ='France' AND A.CustomerID < B.CustomerID

--2. Show the list of German suppliers’ names who are not working in the same cities.

SELECT A.ContactName,
B.ContactName
FROM Suppliers A INNER JOIN Suppliers B
ON A.Country = B.Country
WHERE A.Country ='Germany' AND B.Country ='Germany' AND A.SupplierID < B.SupplierID AND A.City <> B.City

--3.Show the count of orders made by each customer from France.

SELECT C.ContactName ,
COUNT(O.OrderID) AS AmountOfOrders
FROM Customers C LEFT JOIN Orders O
ON C.CustomerID = O.CustomerID
WHERE C.Country = 'France'
GROUP BY C.ContactName ,C.CustomerID
ORDER BY COUNT(O.OrderID)

--4.Show the list of French customers’ names who have made more than one order.

SELECT C.ContactName ,
COUNT(O.OrderID) AS AmountOfOrders
FROM Customers C LEFT JOIN Orders O
ON C.CustomerID = O.CustomerID
WHERE C.Country = 'France'
GROUP BY C.ContactName ,C.CustomerID
HAVING COUNT(O.OrderID) > 1
ORDER BY COUNT(O.OrderID)

--5.Show the list of customers’ names who used to order the ‘Tofu’ product.

SELECT DISTINCT C.ContactName 
FROM Customers C INNER JOIN Orders O ON C.CustomerID = O.CustomerID
LEFT JOIN [Order Details] OD ON O.OrderID = OD.OrderID
LEFT JOIN Products P ON OD.ProductID = P.ProductID
WHERE P.ProductName = 'Tofu'



--6.Show the list of French customers’ names who used to order non-French products.

SELECT DISTINCT C.ContactName
FROM Customers C INNER JOIN Orders O ON C.CustomerID = O.CustomerID
INNER JOIN [Order Details] OD ON O.OrderID = OD.OrderID
INNER JOIN Products P ON OD.ProductID = P.ProductID
INNER JOIN Suppliers S ON P.SupplierID = S.SupplierID
WHERE C.Country = 'France' AND S.Country <> 'France'

--7.Show the list of French customers’ names who used to order French products.

SELECT DISTINCT C.ContactName
FROM Customers C INNER JOIN Orders O ON C.CustomerID = O.CustomerID
INNER JOIN [Order Details] OD ON O.OrderID = OD.OrderID
INNER JOIN Products P ON OD.ProductID = P.ProductID
INNER JOIN Suppliers S ON P.SupplierID = S.SupplierID
WHERE C.Country = 'France' AND S.Country = 'France'
