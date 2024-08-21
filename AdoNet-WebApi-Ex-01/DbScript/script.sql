USE [master]
GO
CREATE DATABASE [AdoNet-WebApi-Ex-01];

GO

USE [AdoNet-WebApi-Ex-01]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Students](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Studnets] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

USE [AdoNet-WebApi-Ex-01]
GO

INSERT INTO [dbo].[Students]
        ([Name]
        ,[Email])
    VALUES
    ('John Doe', 'john.doe@email.com'),
    ('Jane Smith', 'jane.smith@email.com'),
    ('Bob Johnson', 'bob.johnson@email.com'),
    ('Alice Williams', 'alice.williams@email.com'),
    ('Tom Brown', 'tom.brown@email.com'),
    ('Sara Davis', 'sara.davis@email.com'),
    ('Mark Lee', 'mark.lee@email.com'),
    ('Jenny Kim', 'jenny.kim@email.com'),
    ('David Martin', 'david.martin@email.com'),
    ('Karen Jones', 'karen.jones@email.com'),
    ('Mike Adams', 'mike.adams@email.com'),
    ('Laura Wilson', 'laura.wilson@email.com'),
    ('Chris Davis', 'chris.davis@email.com'),
    ('Lisa Thompson', 'lisa.thompson@email.com'),
    ('Brian Johnson', 'brian.johnson@email.com');