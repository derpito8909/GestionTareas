IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'gestionTareas')
BEGIN
    CREATE DATABASE gestionTareas;    
END
ELSE 
BEGIN
    PRINT 'La base de datos \"gestionTareas\" ya existe. Usando la base de datos existente.';
END;
GO

USE gestionTareas;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL  
BEGIN
    CREATE TABLE dbo.Users (
        Id        INT IDENTITY(1,1) PRIMARY KEY, 
        Name      NVARCHAR(100) NOT NULL, 
        Email     NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME     NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT(GETDATE()) 
    );
    
    ALTER TABLE dbo.Users 
        ADD CONSTRAINT UQ_Users_Email UNIQUE (Email);
END
ELSE 
BEGIN
    PRINT 'La tabla \"Users\" ya existe. No se realizará ninguna acción sobre esta tabla.';
END;

IF OBJECT_ID(N'dbo.Tasks', N'U') IS NULL  
BEGIN
    CREATE TABLE dbo.Tasks (
        Id             INT IDENTITY(1,1) PRIMARY KEY, 
        Title          NVARCHAR(200) NOT NULL,
        Description    NVARCHAR(1000) NULL, 
        Status         NVARCHAR(20) NOT NULL, 
        UserId         INT NOT NULL, 
        CreatedAt      DATETIME    NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT(GETDATE()),
        AdditionalInfo NVARCHAR(MAX) NULL,         
       
        CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),       
        CONSTRAINT CK_Tasks_Status CHECK (Status IN ('Pending', 'InProgress', 'Done')),        
        CONSTRAINT CK_Tasks_AdditionalInfo_JSON CHECK (
            AdditionalInfo IS NULL OR ISJSON(AdditionalInfo) = 1
        )
    );
END
ELSE 
BEGIN
    PRINT 'La tabla \"Tasks\" ya existe. No se realizará ninguna acción sobre esta tabla.';
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = N'IX_Tasks_User_Status_Date' AND object_id = OBJECT_ID(N'dbo.Tasks')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tasks_User_Status_Date 
        ON dbo.Tasks(UserId, Status, CreatedAt);    
END
ELSE 
BEGIN
    PRINT 'El índice \"IX_Tasks_User_Status_Date\" ya existe.';
END;
