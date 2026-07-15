USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[sp_GenerateInvoice_ByQuotation]    Script Date: 15-07-2026 16:33:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GenerateInvoice_ByQuotation]
    @QuotationNumber NVARCHAR(100)
AS
---EXEC sp_GenerateInvoice_ByQuotation  @QuotationNumber = 'PACKY-546';
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @BookingID INT,
        @NewInvoiceNumber NVARCHAR(50),
        @NextNumber INT;

    ------------------------------------------------
    -- 1️⃣ Check Quotation Exists
    ------------------------------------------------
    SELECT @BookingID = BookingID
    FROM Booking
    WHERE QuotationNumber = @QuotationNumber;

    IF @BookingID IS NULL
    BEGIN
        SELECT 
            'Quotation number does not exist' AS Message,
            0 AS StatusCode;
        RETURN;
    END

    ------------------------------------------------
    -- 2️⃣ Check Invoice Already Generated
    ------------------------------------------------
    IF EXISTS (
        SELECT 1 
        FROM Invoice 
        WHERE QuotationNumber = @QuotationNumber
    )
    BEGIN
        SELECT 
            'Invoice already generated for this quotation' AS Message,
            0 AS StatusCode;
        RETURN;
    END

    ------------------------------------------------
    -- 3️⃣ Generate Invoice Number
    ------------------------------------------------
    SELECT @NextNumber = ISNULL(MAX(InvoiceID),0) + 1 
    FROM Invoice;

    SET @NewInvoiceNumber = 
        'GCN-' + 
       
        RIGHT('0000' + CAST(@NextNumber AS NVARCHAR), 4);

    ------------------------------------------------
    -- 4️⃣ Insert Invoice
    ------------------------------------------------
    INSERT INTO Invoice (BookingID, QuotationNumber, InvoiceNumber)
    VALUES (@BookingID, @QuotationNumber, @NewInvoiceNumber);

    ------------------------------------------------
    -- 5️⃣ Return Success
    ------------------------------------------------
    SELECT 
        'Invoice generated successfully' AS Message,
        1 AS StatusCode,
        InvoiceID,
        BookingID,
        QuotationNumber,
        InvoiceNumber,
        CreatedDate
    FROM Invoice
    WHERE InvoiceID = SCOPE_IDENTITY();

END
GO

