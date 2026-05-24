-- سكربت إنشاء قاعدة البيانات والجداول
-- إعداد المتدرب: EZZALDEEN

-- 1. إنشاء قاعدة البيانات
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BooksDB')
BEGIN
    CREATE DATABASE BooksDB;
END
GO

USE BooksDB;
GO

-- 2. إنشاء جدول الفئات
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL
    );
END
GO

-- 3. إنشاء جدول الكتب
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Books]') AND type in (N'U'))
BEGIN
    CREATE TABLE Books (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(200) NOT NULL,
        Author NVARCHAR(100) NOT NULL,
        CategoryId INT,
        FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
    );
END
GO

-- 4. إضافة بيانات تجريبية (اختياري)
IF NOT EXISTS (SELECT 1 FROM Categories)
BEGIN
    INSERT INTO Categories (Name) VALUES ('Programming'), ('Databases'), ('Web Development');
END

IF NOT EXISTS (SELECT 1 FROM Books)
BEGIN
    INSERT INTO Books (Title, Author, CategoryId) VALUES 
    ('C# Fundamentals', 'MAHMOUD', 1),
    ('SQL Server 2022 Guide', 'EZZALDEEN', 2),
    ('ASP.NET Core Web API', 'EZZALDEEN_ALRABBI', 3);
END
GO
