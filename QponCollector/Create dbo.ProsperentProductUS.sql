USE [intermediate-qponcrush_db]
GO

/****** Object: Table [dbo].[ProsperentProductUS] Script Date: 9/29/2015 10:27:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProsperentProduct] (
    [catalogId]     NVARCHAR (50)  NOT NULL,
    [productId]     NVARCHAR (50)  NULL,
    [affiliate_url] NVARCHAR (250) NULL,
    [image_url]     NVARCHAR (250) NULL,
    [keyword]       NVARCHAR (250) NULL,
    [description]   NVARCHAR (250) NULL,
    [category]      NVARCHAR (250) NULL,
    [merchant]      NVARCHAR (250) NULL,
    [brand]         NVARCHAR (250) NULL,
    [upc]           NVARCHAR (250) NULL,
    [isbn]          NVARCHAR (250) NULL,
    [sales]         NVARCHAR (250) NULL,
    [minPrice]      MONEY          NULL,
    [maxPrice]      MONEY          NULL,
    [minPriceSale]  MONEY          NULL,
    [maxPriceSale]  MONEY          NULL,
    [groupCount]    NUMERIC (18)   NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	NVARCHAR (31)	NULL,
	CONSTRAINT pkProsperentProduct PRIMARY KEY (catalogId)
);
GO


CREATE TABLE [dbo].[ProsperentCoupons] (
    [catalogId]     NVARCHAR (50)  NOT NULL,
    [merchantId]     NVARCHAR (250)  NULL,
    [affiliate_url] NVARCHAR (250) NULL,
    [image_url]     NVARCHAR (250) NULL,
    [brand]         NVARCHAR (250) NULL,
    [keyword]       NVARCHAR (250) NULL,
    [description]   NVARCHAR (250) NULL,
    [category]      NVARCHAR (250) NULL,
    [dealType]         NVARCHAR (250) NULL,
    [start_date]       NVARCHAR (250) NULL,
    [expiration_date]  NVARCHAR (250) NULL,
    [price]      MONEY          NULL,
    [priceSale]      MONEY          NULL,
    [dollarsOff]  MONEY          NULL,
    [percentOff]  MONEY          NULL,
    [couponCode]         NVARCHAR (250) NULL,
    [promo]         NVARCHAR (250) NULL,
    [restrictions]         NVARCHAR (250) NULL,
    [rank]         REAL NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	NVARCHAR (31)	NULL,
	CONSTRAINT pkProsperentCoupons PRIMARY KEY (catalogId)
);

GO

CREATE TABLE [dbo].[ProsperentMerchant] (
    [merchant]     NVARCHAR (50)  NOT NULL,
    [merchantId]     int  NOT NULL,
    [merchantIds]     NVARCHAR (250)  NULL,
    [logoUrl] NVARCHAR (250) NULL,
    [image_url]     NVARCHAR (250) NULL,
    [domain]         NVARCHAR (250) NULL,
    [category]       NVARCHAR (250) NULL,
    [productDatafeed]   int NULL,
    [numProducts]   int NULL,
    [numProductsCA]   int NULL,
    [numProductsUK]   int NULL,
    [numCouponsUs]   int NULL,
    [numLocalDealsUS]   int NULL,
    [numTravelOffersUS]   int NULL,
    [minPaymentPercentage]   real NULL,
    [maxPaymentPercentage]   real NULL,
    [averagePaymentPercentage]   real NULL,
    [conversionRate]   real NULL,
    [epc]   real NULL,
    [merchantWeight]   real NULL,
    [dateActive]       NVARCHAR (250) NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	NVARCHAR (31)	NULL,
	CONSTRAINT pkProsperentMerchant PRIMARY KEY (merchantId)
);

GO
