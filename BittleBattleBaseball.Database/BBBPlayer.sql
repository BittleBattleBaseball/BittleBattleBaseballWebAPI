﻿CREATE TABLE [dbo].[BBBPlayer]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Email] NVARCHAR(100) NOT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [DateJoined] DATETIME NOT NULL
)
