SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.Customers', 'AspNetUserId') IS NULL
BEGIN
    ALTER TABLE dbo.Customers ADD AspNetUserId nvarchar(450) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Customers_AspNetUserId' AND object_id = OBJECT_ID('dbo.Customers'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Customers_AspNetUserId
        ON dbo.Customers(AspNetUserId)
        WHERE AspNetUserId IS NOT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Customers_AspNetUsers_AspNetUserId')
BEGIN
    ALTER TABLE dbo.Customers
        ADD CONSTRAINT FK_Customers_AspNetUsers_AspNetUserId
        FOREIGN KEY (AspNetUserId) REFERENCES dbo.AspNetUsers(Id);
END
GO

IF COL_LENGTH('dbo.TravelPackages', 'CoverImageUrl') IS NULL
BEGIN
    ALTER TABLE dbo.TravelPackages ADD CoverImageUrl nvarchar(500) NULL;
END
GO

IF OBJECT_ID('dbo.ContactInquiries', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContactInquiries
    (
        InquiryId   int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ContactInquiries PRIMARY KEY,
        FullName    nvarchar(150) NOT NULL,
        Email       nvarchar(150) NOT NULL,
        Phone       nvarchar(30)  NULL,
        Subject     nvarchar(200) NULL,
        Message     nvarchar(max) NOT NULL,
        CreatedDate datetime2     NOT NULL CONSTRAINT DF_ContactInquiries_CreatedDate DEFAULT (SYSUTCDATETIME()),
        IsRead      bit           NOT NULL CONSTRAINT DF_ContactInquiries_IsRead DEFAULT (0)
    );
END
GO
