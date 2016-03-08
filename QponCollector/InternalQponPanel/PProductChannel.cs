using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace InternalQponPanel
{
	public class PProductChannel: PBaseChannel
	{
		public class ProductJData : PBaseChannel.BaseJData
		{
			public ProductJData()
			{
				data = new List<PProductRecord>();
			}
			public IList<PProductRecord> data { get; private set; }
		}

		public PProductChannel()
		{
			jData = new ProductJData(); // empty channel...
		}

		public ProductJData JData
		{
			get
			{
				return (jData as ProductJData);
			}
		}

		public IList<PProductRecord> ListOfProducts
		{
			get
			{
				return JData.data;
			}
		}

		public override bool doparseJson(String pJson) {
			// perform validations to ensure it is actually a Merchant Json string?
			jData = JsonConvert.DeserializeObject<ProductJData>(pJson);
			return true;
		}

		public static string cfilterMerchant = "filterMerchant";
		public static string cfilterCategory = "filterCategory";
		public static string climit = "limit";
		public static string cfilterBrand = "filterBrand";
		public static string cfilterKeyword = "filterKeyword";

		public override string GetBaseUrl()
		{
			return @"http://api.prosperent.com/api/search?api_key=633bab575e0bf71d77701e75de8d6712&";
		}

	}
}

/*
 * ADO.NET:
 * Server=tcp:oud11hbwrc.database.windows.net,1433;Database=intermediate-qponcrush_db;User ID=saviobernard@oud11hbwrc;Password={your_password_here};Trusted_Connection=False;Encrypt=True;Connection Timeout=30;
 * 
 * ODBC:
 * Driver={SQL Server Native Client 10.0};Server=tcp:oud11hbwrc.database.windows.net,1433;Database=intermediate-qponcrush_db;Uid=saviobernard@oud11hbwrc;Pwd=QponCrush15;Encrypt=yes;Connection Timeout=30;
 */