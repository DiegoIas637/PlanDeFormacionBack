IF DB_ID(N'PlanDeFormacion') IS NULL
BEGIN
    CREATE DATABASE [PlanDeFormacion];
END;
GO

USE [PlanDeFormacion];
GO

IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserName] NVARCHAR(100) NOT NULL,
        [Password] NVARCHAR(256) NOT NULL,
        [Role] INT NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Users_UserName] ON [dbo].[Users] ([UserName]);
END;
GO

IF OBJECT_ID(N'[dbo].[Products]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Price] DECIMAL(18,2) NOT NULL,
        [Stock] INT NOT NULL,
        [UserId] INT NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_Users_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_Products_UserId] ON [dbo].[Products] ([UserId]);
END;
GO

IF OBJECT_ID(N'[dbo].[Orders]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Orders] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [ProductId] UNIQUEIDENTIFIER NOT NULL,
        [Quantity] INT NOT NULL,
        [AppliedPrice] DECIMAL(18,2) NOT NULL,
        [UserId] INT NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Orders_Products_ProductId] FOREIGN KEY ([ProductId])
            REFERENCES [dbo].[Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
    );

    CREATE INDEX [IX_Orders_ProductId] ON [dbo].[Orders] ([ProductId]);
    CREATE INDEX [IX_Orders_UserId] ON [dbo].[Orders] ([UserId]);
END;
GO
