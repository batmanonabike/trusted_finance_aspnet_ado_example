USE trusted_finance
GO

DROP TABLE IF EXISTS dbo.Books
GO

CREATE TABLE dbo.Books(
	BookId int IDENTITY PRIMARY KEY,
	Title nvarchar(400) NOT NULL,
	Author nvarchar(200) NOT NULL,
	PublishDate date NOT NULL,
	Genre nvarchar(50) NOT NULL,
	Price decimal(18,2) NOT NULL)
GO


