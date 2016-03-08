/*
UpdateStatus codes
1 - newly inserted
2 - accepted 
3 - rejected
4 - push back/later

*/
USE [intermediate-qponcrush_db]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[UserList]
GO

CREATE TABLE [dbo].[UserList] (
	[userid] int not null,
	[userName] nvarchar(50) not null,
	[isActive] integer not null,
	[password] nvarchar(50)
	constraint pkUserList primary key (userId)
);

DROP TABLE [dbo].[ListInt]
GO

CREATE TABLE [dbo].[ListInt] (
	[intValue] int not null,
	[stringKey] nvarchar(50) not null,
	[listType] int not null,
	constraint pkListInt primary key (listType, intValue)
);


DROP TABLE [dbo].[ProsperentProduct]
GO

CREATE TABLE [dbo].[ProsperentProduct] (
    [catalogId]     NVARCHAR (50)  NOT NULL,
    [productId]     NVARCHAR (50)  NULL,
    [affiliate_url] NVARCHAR (250) NULL,
    [image_url]     NVARCHAR (250) NULL,
    [keyword]       NVARCHAR (250) NULL,
    [description]   NVARCHAR (250) NULL,
    [category]      NVARCHAR (250) NULL,
    [price]			MONEY,
	[price_sale]	MONEY,
	[percentOff]	MONEY,
	[currency]       NVARCHAR (50),
    [merchant]       NVARCHAR (250),
    [merchantId]     NVARCHAR (50),
    [brand]         NVARCHAR (250) NULL,
    [upc]           NVARCHAR (250) NULL,
    [isbn]          NVARCHAR (250) NULL,
    [sales]         int NULL,
	[baseCouponId]	NVARCHAR(100) NULL,
	[qponCategory]	NVARCHAR(100) NULL,
	[expiryDate]	DATE NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	int	NULL,
	CONSTRAINT pkProsperentProduct PRIMARY KEY (catalogId)
);
GO
/*
ALTER TABLE [dbo].[ProsperentProduct] ADD [baseCouponId]	NVARCHAR(100) NULL;
ALTER TABLE [dbo].[ProsperentProduct] ADD [qponCategory]	NVARCHAR(100) NULL;
ALTER TABLE [dbo].[ProsperentProduct] ADD [expiryDate]	DATE NULL;
*/
DROP PROCEDURE [dbo].[spProsperentProduct]
GO

create PROCEDURE [dbo].[spProsperentProduct]
(
	@updateAction  INTEGER,
    @catalogId     NVARCHAR (50),
    @productId     NVARCHAR (50),
    @affiliate_url NVARCHAR (250),
    @image_url     NVARCHAR (250),
    @keyword       NVARCHAR (250),
    @description   NVARCHAR (250),
    @category      NVARCHAR (250),
    @price			MONEY,
	@price_sale		MONEY,
	@percentOff		MONEY,
	@currency       NVARCHAR (50),
    @merchant       NVARCHAR (250),
    @merchantId     NVARCHAR (50),
    @brand         NVARCHAR (250),
    @upc           NVARCHAR (250),
    @isbn          NVARCHAR (250),
    @sales         NVARCHAR (250),
	@createdOn		DATE,
	@updatedOn		DATE,
	@updateStatus	INTEGER,
	@updateUser	int
)
AS
BEGIN
	DECLARE @existCount int
	

	if (@catalogId IS NULL)
		RETURN;

	SELECT @existCount = COUNT(0) FROM [dbo].[ProsperentProduct] where catalogId = @catalogId

	if (@updateAction = 3) BEGIN
		if (@existCount = 0) BEGIN
			return
		END
		DELETE FROM [dbo].[ProsperentProduct] where catalogId = @catalogId
		RETURN
	END
	if (@existCount > 0) BEGIN
		SET @updateStatus = 1
		UPDATE [dbo].[ProsperentProduct] SET
			--catalogId = @catalogId,
			productId = @productId,
			affiliate_url = @affiliate_url,
			image_url = @image_url,
			keyword = @keyword,
			description = @description,
			category = @category,
			brand = @brand,
			upc = @upc,
			isbn = @isbn,
			sales = @sales,
			price = @price,
			percentOff = @percentOff,
			price_sale = @price_sale,
			merchant = @merchant,
			merchantId = @merchantId,
			currency = @currency,
			--createdOn = NOW(), --//@createdOn,
			updatedOn = GETDATE(),
			updateStatus = IIF(ISNULL(percentOff, 0) <> ISNULL(@percentOff, 0) OR ISNULL(price_sale, 0) <> ISNULL(@price_sale, 0), ISNULL(@updateStatus, updateStatus), updateStatus),
			updateUser = @updateUser
			where
			catalogId = @catalogId
	END else BEGIN
		SET @updateStatus = 1
		INSERT INTO [dbo].[ProsperentProduct] (
			catalogId, productId, affiliate_url, image_url, keyword, description,
			category,merchant,brand,upc,isbn,sales,
			merchantId, price, price_sale, percentOff, currency,
			createdOn,updatedOn,updateStatus,updateUser
			)
		VALUES (
			@catalogId, @productId, @affiliate_url, @image_url, @keyword, @description,
			@category,@merchant,@brand,@upc,@isbn,@sales,
			@merchantId, @price, @price_sale, @percentOff, @currency,
			GETDATE(),GETDATE(),@updateStatus,@updateUser
			)
	END
END;

GO

DROP TABLE [dbo].[ProsperentCoupons]
GO

CREATE TABLE [dbo].[ProsperentCoupons] (
    [catalogId]     NVARCHAR (50)  NOT NULL,
    [merchantId]     NVARCHAR (50)  NOT NULL,
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
    [couponCode]         NVARCHAR (50) NOT NULL,
    [promo]         NVARCHAR (250) NULL,
    [restrictions]         NVARCHAR (250) NULL,
    [rank]         REAL NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	int,
	CONSTRAINT pkProsperentCoupons PRIMARY KEY (catalogId, merchantId, couponCode)
);

GO

DROP PROCEDURE [dbo].[spProsperentCoupons] 
GO

CREATE PROCEDURE [dbo].[spProsperentCoupons] (
	@updateAction  INTEGER,
    @catalogId     NVARCHAR (50),
    @merchantId     NVARCHAR (50),
    @affiliate_url NVARCHAR (250),
    @image_url     NVARCHAR (250),
    @brand         NVARCHAR (250),
    @keyword       NVARCHAR (250),
    @description   NVARCHAR (250),
    @category      NVARCHAR (250),
    @dealType         NVARCHAR (250),
    @start_date       NVARCHAR (250),
    @expiration_date  NVARCHAR (250),
    @price      MONEY,
    @priceSale      MONEY,
    @dollarsOff  MONEY,
    @percentOff  MONEY,
    @couponCode         NVARCHAR (250),
    @promo         NVARCHAR (250),
    @restrictions         NVARCHAR (250),
    @rank         REAL,
	@createdOn		DATE,
	@updatedOn		DATE,
	@updateStatus	INTEGER,
	@updateUser	int
)
AS
BEGIN
	DECLARE @existCount int
	SELECT @existCount = COUNT(0) FROM [dbo].[ProsperentCoupons] where 
		catalogId = @catalogId AND
		merchantId = @merchantId AND
		couponCode = @couponCode;

	if (@updateAction = 3) BEGIN
		if (@existCount = 0) BEGIN
			return;
		END
		DELETE FROM [dbo].[ProsperentCoupons] where 
			catalogId = @catalogId AND
			merchantId = @merchantId AND
			couponCode = @couponCode;
		return;
	END
	if (@existCount > 0) BEGIN
		UPDATE [dbo].[ProsperentCoupons] SET
		catalogId = @catalogId,
		merchantId = @merchantId,
		affiliate_url = @affiliate_url,
		image_url = @image_url,
		brand = @brand,
		keyword = @keyword,
		description = @description,
		category = @category,
		dealType = @dealType,
		start_date = @start_date,
		expiration_date = @expiration_date,
		price = @price,
		priceSale = @priceSale,
		dollarsOff = @dollarsOff,
		percentOff = @percentOff,
		couponCode = @couponCode,
		promo = @promo,
		restrictions = @restrictions,
		rank = @rank,
		updatedOn	 = GETDATE(),
		updateStatus = ISNULL(@updateStatus, updateStatus),
		updateUser	 = @updateUser
		WHERE 
		catalogId = @catalogId AND
		merchantId = @merchantId AND
		couponCode = @couponCode

	END else BEGIN
		SET @updateStatus = 1
		INSERT INTO [dbo].[ProsperentCoupons] (
			catalogId, merchantId, affiliate_url, image_url, brand, keyword, 
			description, category, dealType, start_date, expiration_date, 
			price, priceSale, dollarsOff, percentOff, couponCode, promo, 
			restrictions, rank, createdOn, updatedOn, updateStatus, updateUser)
			VALUES (
				@catalogId, @merchantId, @affiliate_url, @image_url, @brand, @keyword, 
				@description, @category, @dealType, @start_date, @expiration_date, 
				@price, @priceSale, @dollarsOff, @percentOff, @couponCode, @promo, 
				@restrictions, @rank, GETDATE(), GETDATE(), @updateStatus, @updateUser)
	END; 
END;


GO


DROP TABLE [dbo].[ProsperentMerchant] 
GO

CREATE TABLE [dbo].[ProsperentMerchant] (
    [merchant]     NVARCHAR (50)  NOT NULL,
    [merchantId]     NVARCHAR (50) NOT NULL,
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
	[vendorId] nvarchar(100) NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	int,
	CONSTRAINT pkProsperentMerchant PRIMARY KEY (merchantId)
);

/*
ALTER TABLE [dbo].[ProsperentMerchant] ADD [vendorId] nvarchar(100) NULL;
*/
GO

DROP PROCEDURE [dbo].[spProsperentMerchant]
GO

CREATE PROCEDURE [dbo].[spProsperentMerchant] (
	@updateAction  INTEGER,
    @merchant     NVARCHAR (50),
    @merchantId     NVARCHAR (50),
    @logoUrl NVARCHAR (250),
    @image_url     NVARCHAR (250),
    @domain         NVARCHAR (250),
    @category       NVARCHAR (250),
    @productDatafeed   int,
    @numProducts   int,
    @numProductsCA   int,
    @numProductsUK   int,
    @numCouponsUs   int,
    @numLocalDealsUS   int,
    @numTravelOffersUS   int,
    @minPaymentPercentage   real,
    @maxPaymentPercentage   real,
    @averagePaymentPercentage   real,
    @conversionRate   real,
    @epc   real,
    @merchantWeight   real,
	--@vendorId nvarchar(100),
    @dateActive       NVARCHAR (250),
	@createdOn		DATE,
	@updatedOn		DATE,
	@updateStatus	INTEGER,
	@updateUser	int
)
AS
BEGIN
	DECLARE @existCount int
	SELECT @existCount = COUNT(0) FROM [dbo].[ProsperentMerchant] where 
		merchantId = @merchantId 
	if (@updateAction = 3) BEGIN
		if (@existCount = 0) BEGIN
			return;
		END
		DELETE FROM [dbo].[ProsperentMerchant] where 
			merchantId = @merchantId ;
		return
	END
	if (@existCount > 0) BEGIN
		UPDATE [dbo].[ProsperentMerchant] SET
			merchant = @merchant,
			merchantId = @merchantId,
			logoUrl = @logoUrl,
			image_url = @image_url,
			domain = @domain,
			category = @category,
			productDatafeed = @productDatafeed,
			numProducts = @numProducts,
			numProductsCA = @numProductsCA,
			numProductsUK = @numProductsUK,
			numCouponsUs = @numCouponsUs,
			numLocalDealsUS = @numLocalDealsUS,
			numTravelOffersUS = @numTravelOffersUS,
			minPaymentPercentage = @minPaymentPercentage,
			maxPaymentPercentage = @maxPaymentPercentage,
			averagePaymentPercentage = @averagePaymentPercentage,
			conversionRate = @conversionRate,
			epc = @epc,
			merchantWeight = @merchantWeight,
			dateActive = @dateActive,
			--vendorId = @vendorId,
			--createdOn		 = GETDATE(),
			updatedOn		 = GETDATE(),
			updateStatus = ISNULL(@updateStatus, updateStatus),
			updateUser	 = @updateUser
			WHERE
			merchantId = @merchantId;
	END else BEGIN
		SET @updateStatus = 1
		INSERT INTO [dbo].[ProsperentMerchant] (
			merchant, merchantId, logoUrl, image_url, 
			domain, category, productDatafeed, numProducts, numProductsCA, 
			numProductsUK, numCouponsUs, numLocalDealsUS, numTravelOffersUS, 
			minPaymentPercentage, maxPaymentPercentage, averagePaymentPercentage, 
			conversionRate, epc, merchantWeight, dateActive, 
			--vendorId,
			createdOn, 
			updatedOn, updateStatus, updateUser
			) VALUES (
			@merchant, @merchantId, @logoUrl, @image_url, 
			@domain, @category, @productDatafeed, @numProducts, @numProductsCA, 
			@numProductsUK, @numCouponsUs, @numLocalDealsUS, @numTravelOffersUS, 
			@minPaymentPercentage, @maxPaymentPercentage, @averagePaymentPercentage, 
			@conversionRate, @epc, @merchantWeight, @dateActive, 
			--@vendorId,
			GETDATE(), GETDATE(), @updateStatus, @updateUser)
	END

END

GO



DROP TABLE [dbo].[FmtcDeals]
GO

CREATE TABLE [dbo].[FmtcDeals] (
    [nCouponID]     int NOT NULL,
    [cMerchant]     NVARCHAR (150)  NULL,
    [nMerchantID]	int NULL,
    [nMasterMerchantID]     int NULL,
	[cNetwork]		NVARCHAR(50) NULL,
	[cStatus]		NVARCHAR(50) NULL,
	[cLabel]		NVARCHAR(50) NULL,
	[cImage]		NVARCHAR (250) NULL,
	[cRestrictions]	NVARCHAR (250) NULL,
	[cCode]			NVARCHAR (250) NULL,
	[dtStartDate]	DATE NULL,
	[dtEndDate]		DATE NULL,
	[cLastUpdated]	DATE NULL,
	[cCreated]		DATE NULL,
	[cAffiliateURL]	NVARCHAR (250) NULL,
	[cDirectURL]	NVARCHAR (250) NULL,
	[cSkimlinksURL]	NVARCHAR (250) NULL,
	[cFMTCURL]		NVARCHAR (250) NULL,
	[aTypes]		NVARCHAR (250) NULL,
	[fSalePrice]	MONEY NULL,
	[fWasPrice]		MONEY NULL,
	[fDiscount]		MONEY NULL,
	[nPercent]		MONEY NULL,
	[fmtcCategory]  NVARCHAR(250) NULL,
	[baseCouponId]	NVARCHAR(100) NULL,
	[qponCategory]	NVARCHAR(100) NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	int	NULL,
	CONSTRAINT pkFmtcDeals PRIMARY KEY (nCouponId)
);
GO
/*
ALTER TABLE [dbo].[FmtcDeals] ADD [baseCouponId]	NVARCHAR(100) NULL;
ALTER TABLE [dbo].[FmtcDeals] ADD [qponCategory]	NVARCHAR(100) NULL;
ALTER TABLE [dbo].[FmtcDeals] ADD [fmtcCategory]  NVARCHAR(250) NULL;
*/
DROP PROCEDURE [dbo].[spFmtcDeals]
GO

CREATE PROCEDURE [dbo].[spFmtcDeals] (
	@updateAction  INTEGER,
    @nCouponID     int,
    @cMerchant     NVARCHAR (150),
    @nMerchantID	int,
    @nMasterMerchantID     int,
	@cNetwork		NVARCHAR(50),
	@cStatus		NVARCHAR(50),
	@cLabel		NVARCHAR(50),
	@cImage		NVARCHAR (250),
	@cRestrictions	NVARCHAR (250),
	@cCode			NVARCHAR (250),
	@dtStartDate	DATE,
	@dtEndDate		DATE,
	@cLastUpdated	DATE,
	@cCreated		DATE,
	@cAffiliateURL	NVARCHAR (250),
	@cDirectURL	NVARCHAR (250),
	@cSkimlinksURL	NVARCHAR (250),
	@cFMTCURL		NVARCHAR (250),
	@aTypes		NVARCHAR (250),
	@fmtcCategory	NVARCHAR (250),
	@fSalePrice	MONEY,
	@fWasPrice		MONEY,
	@fDiscount		MONEY,
	@nPercent		MONEY,
	@createdOn		DATE,
	@updatedOn		DATE,
	@updateStatus	INTEGER,
	@updateUser	int
)
AS
BEGIN
	DECLARE @existCount int
	SELECT @existCount = COUNT(0) FROM [dbo].[FmtcDeals] where 
		nCouponID = @nCouponID 
	if (@updateAction = 3) BEGIN
		if (@existCount = 0) BEGIN
			return;
		END
		DELETE FROM [dbo].[FmtcDeals] where 
			nCouponID = @nCouponID;
		return
	END
	if (@existCount > 0) BEGIN
		UPDATE [dbo].[FmtcDeals] SET
		cMerchant = @cMerchant,
		nMerchantID = @nMerchantID ,
		nMasterMerchantID = @nMasterMerchantID, 
		cNetwork = @cNetwork, 
		cStatus = @cStatus, 
		cLabel = @cLabel, 
		cImage = @cImage, 
		cRestrictions = @cRestrictions, 
		cCode = @cCode, 
		dtStartDate = @dtStartDate, 
		dtEndDate = @dtEndDate, 
		cLastUpdated = @cLastUpdated, 
		cCreated = @cCreated, 
		cAffiliateURL = @cAffiliateURL, 
		cDirectURL = @cDirectURL, 
		cSkimlinksURL = @cSkimlinksURL, 
		cFMTCURL = @cFMTCURL, 
		aTypes = @aTypes, 
		fmtcCategory = @fmtcCategory,
		fSalePrice = @fSalePrice, 
		fWasPrice = @fWasPrice, 
		fDiscount = @fDiscount, 
		nPercent = @nPercent, 
		createdOn = @createdOn, 
		updatedOn = @updatedOn, 
		updateStatus = ISNULL(@updateStatus, updateStatus),
		updateUser = @updateUser
		where  
		nCouponID = @nCouponID
	END ELSE BEGIN
		SET @updateStatus = 1
		INSERT INTO [dbo].[FmtcDeals] (
			nCouponID, cMerchant, nMerchantID, nMasterMerchantID, cNetwork, 
			cStatus, cLabel, cImage, cRestrictions, cCode, dtStartDate, dtEndDate, 
			cLastUpdated, cCreated, cAffiliateURL, cDirectURL, cSkimlinksURL, cFMTCURL, 
			aTypes, fmtcCategory, fSalePrice, fWasPrice, fDiscount, nPercent, createdOn, updatedOn, 
			updateStatus, updateUser
		) VALUES (
			@nCouponID, @cMerchant, @nMerchantID, @nMasterMerchantID, @cNetwork, 
			@cStatus, @cLabel, @cImage, @cRestrictions, @cCode, @dtStartDate, @dtEndDate, 
			@cLastUpdated, @cCreated, @cAffiliateURL, @cDirectURL, @cSkimlinksURL, @cFMTCURL, 
			@aTypes, @fmtcCategory, @fSalePrice, @fWasPrice, @fDiscount, @nPercent, @createdOn, @updatedOn, 
			@updateStatus, @updateUser
		)
	END
END

GO

DROP TABLE [dbo].[FmtcMerchants] 
GO

CREATE TABLE [dbo].[FmtcMerchants] (
    [nMerchantID]     int NOT NULL,
    [nMasterMerchantID] int,
	[nSkimlinksID]		int,
	[cName] NVARCHAR (250) NULL,
	[cNetwork] NVARCHAR (50) NULL,
	[cProgramID] NVARCHAR (50) NULL,
	[nParentMerchantID] int,
	[cAffiliateURL] NVARCHAR (250) NULL,
	[cSkimlinksURL] NVARCHAR (250) NULL,
	[cFMTCURL] NVARCHAR (250) NULL,
	[cHomepageURL] NVARCHAR (250) NULL,
	[dtCreated] DateTime NULL,
	[dtLastUpdated] DateTime NULL,
    [logoUrl] NVARCHAR (250) NULL,
	[vendorId] nvarchar(100) NULL,
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	int,
	CONSTRAINT pkFmtcMerchants PRIMARY KEY (nMerchantID)
);

GO
DROP PROCEDURE [dbo].[spFmtcMerchants] 
GO

CREATE PROCEDURE [dbo].[spFmtcMerchants] (
	@updateAction  INTEGER,
	@nMerchantID     int,
	@nMasterMerchantID int,
	@nSkimlinksID		int,
	@cName NVARCHAR (250),
	@cNetwork NVARCHAR (50),
	@cProgramID NVARCHAR (50),
	@nParentMerchantID int,
	@cAffiliateURL NVARCHAR (250),
	@cSkimlinksURL NVARCHAR (250),
	@cFMTCURL NVARCHAR (250),
	@cHomepageURL NVARCHAR (250),
	@dtCreated DateTime,
	@dtLastUpdated DateTime,
	@logoUrl NVARCHAR (250),
--	@vendorId nvarchar(100),
	@createdOn		DATE,
	@updatedOn		DATE,
	@updateStatus	INTEGER,
	@updateUser	int
)
AS
BEGIN
	DECLARE @existCount int
	SELECT @existCount = COUNT(0) FROM [dbo].[FmtcMerchants] where 
		nMerchantID = @nMerchantID 
	if (@updateAction = 3) BEGIN
		if (@existCount = 0) BEGIN
			return;
		END
		DELETE FROM [dbo].[FmtcMerchants] where 
			nMerchantID = @nMerchantID;
		return
	END
	if (@existCount > 0) BEGIN
		UPDATE [dbo].[FmtcMerchants] SET
		nMasterMerchantID = @nMasterMerchantID,
		nSkimlinksID = @nSkimlinksID,
		cName = @cName,
		cNetwork = @cNetwork,
		cProgramID = @cProgramID,
		nParentMerchantID = @nParentMerchantID,
		cAffiliateURL = @cAffiliateURL,
		cSkimlinksURL = @cSkimlinksURL,
		cFMTCURL = @cFMTCURL,
		cHomepageURL = @cHomepageURL,
		dtCreated = @dtCreated,
		dtLastUpdated = @dtLastUpdated,
		logoUrl = @logoUrl,
		--vendorId = @vendorId,
		createdOn = @createdOn,
		updatedOn = @updatedOn,
		updateStatus = ISNULL(@updateStatus, updateStatus),
		updateUser = @updateUser
		WHERE nMerchantID = @nMerchantID
	END ELSE BEGIN
		INSERT INTO [dbo].[FmtcMerchants] (
		nMerchantID, nMasterMerchantID, nSkimlinksID, cName, cNetwork, cProgramID, nParentMerchantID, cAffiliateURL, cSkimlinksURL, cFMTCURL, cHomepageURL, 
		dtCreated, dtLastUpdated, logoUrl, 
		--vendorId, 
		createdOn, updatedOn, updateStatus, updateUser
		) VALUES (
		@nMerchantID, @nMasterMerchantID, @nSkimlinksID, @cName, @cNetwork, @cProgramID, @nParentMerchantID, @cAffiliateURL, @cSkimlinksURL, @cFMTCURL, @cHomepageURL, 
		@dtCreated, @dtLastUpdated, @logoUrl, 
		--@vendorId, 
		@createdOn, @updatedOn, 1, @updateUser)
	END
END

GO


DROP TABLE [dbo].[CategoryMap] 
GO

CREATE TABLE [dbo].[CategoryMap]  (
	qponCategory nvarchar(60) not null,
	keyword nvarchar(250) not null,
	imageUrl nvarchar(250) null,
	overwriteImageUrl int not null default 0,
	constraint pkCategoryMap primary key (qponCategory, keyword)
);

/*
alter table [dbo].[CategoryMap] MODIFY keyword nvarchar(250);
alter table [dbo].[CategoryMap] ADD overwriteImageUrl int not null default 0;
*/


/*
select count(0) from dbo.ProsperentProduct
select count(0) from dbo.FmtcDeals 
select count(0) from dbo.FmtcMerchants 
select count(0) from dbo.ProsperentMerchant 

select nMerchantID from dbo.FmtcDeals 
where nMerchantID in (
select nMerchantID from dbo.FmtcMerchants
)
group by nMerchantID
4506
593
4197
8256
8250
4955
1105
2731

delete from dbo.FmtcDeals WHERE nMerchantID not in (
4506, 593, 4197, 8256, 8250, 4955, 1105, 2731);


select count(0) from dbo.FmtcDeals 
update dbo.fmtcMerchant
*/

