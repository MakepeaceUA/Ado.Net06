CREATE TABLE VegFruTable 
(
    ID INT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Type VARCHAR(10) NOT NULL,
    Calories INT NOT NULL
);

INSERT INTO VegFruTable(ID,Name, Type, Calories) VALUES
(1,'������', 'Fruit', 60),
(2,'�����', 'Fruit', 90),
(3,'������', 'Vegetable', 15),
(4,'�������', 'Vegetable', 20)