--1. Calculate the greatest, the smallest and the average age among the employees from 
--London.
--2. Calculate the greatest, the smallest and the average age of the employees for each city.
--3. Show the list of cities in which the average age of employees is greater than 60 (the 
--average age is also to be shown)
--4. Show the first and last name(s) of the eldest employee(s).
--5. Show first, last names and ages of 3 eldest employees

SELECT 
MAX(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) AS GreatestAge,
MIN(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) AS SmallestAge,
AVG(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) AS AverageAge
FROM Employees
WHERE City LIKE 'London'

SELECT City,
MAX(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) AS GreatestAge,
MIN(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) AS SmallestAge,
AVG(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) AS AverageAge
FROM Employees
WHERE City IS NOT NULL
GROUP BY City

SELECT City,
AVG(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) AS AverageAge
FROM Employees
WHERE City IS NOT NULL
GROUP BY City
HAVING AVG(YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate)) > 50

SELECT TOP(1) FirstName +' '+ LastName AS FullName ,
YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate) AS Age
FROM Employees
ORDER BY YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate) DESC


SELECT TOP(3) FirstName +' '+ LastName AS FullName ,
YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate) AS Age
FROM Employees
ORDER BY YEAR(CURRENT_TIMESTAMP) - YEAR(BirthDate) DESC
