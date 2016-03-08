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
	[createdOn]		DATE			NULL,
	[updatedOn]		DATE			NULL,
	[updateStatus]	INTEGER			NULL,
	[updateUser]	NVARCHAR (31)	NULL,
	CONSTRAINT pkProsperentMerchant PRIMARY KEY (merchantId)
);

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
    @dateActive       NVARCHAR (250),
	@createdOn		DATE,
	@updatedOn		DATE,
	@updateStatus	INTEGER,
	@updateUser	NVARCHAR (31)	
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
			--createdOn		 = GETDATE(),
			updatedOn		 = GETDATE(),
			updateStatus	 = @updateStatus,
			updateUser	 = USER
			WHERE
			merchantId = @merchantId;
	END else BEGIN
		INSERT INTO [dbo].[ProsperentMerchant] (
			merchant, merchantId, logoUrl, image_url, 
			domain, category, productDatafeed, numProducts, numProductsCA, 
			numProductsUK, numCouponsUs, numLocalDealsUS, numTravelOffersUS, 
			minPaymentPercentage, maxPaymentPercentage, averagePaymentPercentage, 
			conversionRate, epc, merchantWeight, dateActive, createdOn, 
			updatedOn, updateStatus, updateUser
			) VALUES (
			@merchant, @merchantId, @logoUrl, @image_url, 
			@domain, @category, @productDatafeed, @numProducts, @numProductsCA, 
			@numProductsUK, @numCouponsUs, @numLocalDealsUS, @numTravelOffersUS, 
			@minPaymentPercentage, @maxPaymentPercentage, @averagePaymentPercentage, 
			@conversionRate, @epc, @merchantWeight, @dateActive, GETDATE(), 
			GETDATE(), @updateStatus, USER)
	END

END

GO

