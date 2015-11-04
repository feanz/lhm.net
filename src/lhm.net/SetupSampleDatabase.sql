CREATE TABLE [Department] (
    [ID] INTEGER NOT NULL IDENTITY(1, 1),
    [Name] VARCHAR(50) NULL,    
    PRIMARY KEY ([ID])
);

CREATE TABLE [Position] (
    [ID] INTEGER NOT NULL,
    [Name] VARCHAR(50) NULL,   
    PRIMARY KEY ([ID])
);

CREATE TABLE [Staff] (
    [ID] INTEGER NOT NULL IDENTITY(1, 1),
    [Username] NVARCHAR(255) NULL,
    [Email] NVARCHAR(255) NULL,
    [FirstName] NVARCHAR(255) NULL,
    [LastName] NVARCHAR(255) NULL,
    [Telephone] NVARCHAR(100) NULL,
    [IsVIP] int NULL,
	[DepartmentID] INT NOT NULL,
	[PositionID] INT NOT NULL,
    PRIMARY KEY ([ID])
);

ALTER TABLE [Staff]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Staff_dbo.Department] FOREIGN KEY(DepartmentID)
REFERENCES [dbo].[Department] ([ID])

ALTER TABLE [Staff]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Staff_dbo.Position] FOREIGN KEY(PositionID)
REFERENCES [dbo].[Position] ([ID])

INSERT INTO [Department] (NAME) VALUES ('Shipping'),('Sales'),('IT')

INSERT INTO [Position] (ID,NAME) VALUES (1,'Director'),(2,'Manager'),(3,'IT Support')