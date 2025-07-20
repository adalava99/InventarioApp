--Crear y usar base de datos de productos
CREATE DATABASE ProductosDb;
GO

USE ProductosDb;
GO

--Crear tabla de productos
CREATE TABLE productos(
	id BIGINT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR(255) NOT NULL,
	descripcion VARCHAR(255) NOT NULL,
	categoria VARCHAR(255) NOT NULL,
	imagen VARCHAR(255),
	precio FLOAT NOT NULL,
	stock INT NOT NULL CHECK (stock >= 0),
	createdAt DATETIME NOT NULL DEFAULT GETDATE(),
	updatedAt DATETIME NULL
	);
GO

--Insercion dummy data
INSERT INTO productos (nombre, descripcion, categoria, imagen, precio, stock) VALUES
('Celular', 'iPhone X morado', 'Tecnologia', 'http://imageurl.com/iphone', 1000.50, 1),
('Laptop', 'Dell Inspiron 15', 'Tecnologia', 'http://imageurl.com/dell', 850.00, 3),
('Camiseta', 'Camiseta negra de algodon', 'Ropa', 'http://imageurl.com/camiseta', 15.99, 50),
('Pantalon', 'Jean azul talla M', 'Ropa', 'http://imageurl.com/pantalon', 29.99, 40),
('Lampara', 'Lampara LED de escritorio', 'Hogar', 'http://imageurl.com/lampara', 22.49, 10);
GO



--Crear y usar base de datos de transacciones
CREATE DATABASE  TransaccionesDb;
GO

USE TransaccionesDb;
GO

--Crear Tabla de Transacciones
CREATE TABLE transacciones(
	id BIGINT IDENTITY(1,1) PRIMARY KEY,
	fecha DATETIME NOT NULL DEFAULT GETDATE(),
	tipoTransaccion VARCHAR(20) CHECK (tipoTransaccion IN ('Venta', 'Compra')),
	productoId INT NOT NULL,
	cantidad INT NOT NULL,
	precioUnitario FLOAT NOT NULL,
	precioTotal FLOAT NOT NULL,
	detalle VARCHAR(255) NOT NULL,
	updatedAt DATETIME NULL
	);
GO

--Insercion dummy data
INSERT INTO transacciones (tipoTransaccion, productoId, cantidad, precioUnitario, precioTotal, detalle)
VALUES 
('Venta', 1, 1, 1000.50, 1000.50, 'Venta de iPhone X'),
('Compra', 2, 2, 850.00, 1700.00, 'Compra de laptops Lenovo'),
('Venta', 4, 3, 35.00, 105.00, 'Venta de audifonos Bluetooth');
GO

