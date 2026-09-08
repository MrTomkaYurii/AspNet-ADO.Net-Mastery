-- Схема демонстраційної бази для прикладу AdoNet-ThreeLayer-Ex-01.
-- Той самий скрипт виконує DatabaseBootstrapper під час старту API,
-- тому запускати вручну не обов'язково.

IF DB_ID(N'AdoNet_ThreeLayer_Ex_01') IS NULL
    CREATE DATABASE [AdoNet_ThreeLayer_Ex_01];
GO

USE [AdoNet_ThreeLayer_Ex_01];
GO

IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
CREATE TABLE dbo.Students
(
    Id    INT           IDENTITY(1,1) CONSTRAINT PK_Students PRIMARY KEY,
    Name  NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Students)
INSERT INTO dbo.Students (Name, Email) VALUES
    (N'John Doe',       N'john.doe@email.com'),
    (N'Jane Smith',     N'jane.smith@email.com'),
    (N'Bob Johnson',    N'bob.johnson@email.com'),
    (N'Alice Williams', N'alice.williams@email.com'),
    (N'Tom Brown',      N'tom.brown@email.com');
GO
