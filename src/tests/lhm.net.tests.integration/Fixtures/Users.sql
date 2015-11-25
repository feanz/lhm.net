CREATE TABLE [Users] (
    [ID] INTEGER NOT NULL IDENTITY(1, 1),
    [Reference] int NOT NULL,
    [Username] NVARCHAR(255) NOT NULL,    
	[Group] NVARCHAR(255) NOT NULL,    
	[CreatedAt] datetime NOT NULL,
    [Comment] varchar(20) DEFAULT NULL,
    [Description] text,
    PRIMARY KEY ([ID])
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_Group_Username] ON [dbo].[Users]
(
	[GROUP] ASC,
	[Username] ASC
);

CREATE NONCLUSTERED INDEX [IX_UsernameCreatedAt] ON [dbo].[Users]
(
	[Username] ASC,
	[CreatedAt] ASC
);
