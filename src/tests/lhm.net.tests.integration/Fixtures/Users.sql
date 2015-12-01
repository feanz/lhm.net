CREATE TABLE [Users] (
    [ID] INTEGER NOT NULL IDENTITY(1, 1),
    [Reference] int NOT NULL,
	[Application] NVARCHAR(255) NOT NULL,    	
    [Username] NVARCHAR(255) NOT NULL,    	
	[CreatedAt] datetime NULL,
    [Comment] varchar(20),
    [Description] nvarchar(4000) NULL,
    PRIMARY KEY ([ID])
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_Group_Username] ON [dbo].[Users]
(
	[Application] ASC,
	[Username] ASC
);

CREATE NONCLUSTERED INDEX [IX_Username] ON [dbo].[Users]
(	
	[Username] ASC
);