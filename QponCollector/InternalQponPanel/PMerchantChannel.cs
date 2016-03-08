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
	public class PMerchantChannel : PBaseChannel
	{
		public static String SampleString = "{\"errors\":[],\"warnings\":[],\"data\":[{\"merchant\":\"Books-A-Million\",\"merchantId\":\"123456\",\"logoUrl\":\"http://images.prosperentcdn.com/images/logo/merchant/original/120x60/123456.jpg?prosp=&m=Books-A-Million\",\"color1\":\"#4DB5E7\",\"color2\":\"#B3B6B7\",\"domain\":\"booksamillion.com\",\"category\":\"Media\",\"description\":null,\"productDatafeed\":\"0\",\"deepLinking\":\"1\",\"numProducts\":0,\"numProductsCA\":0,\"numProductsUK\":0,\"numCouponsUS\":0,\"numTravelOffersUS\":0,\"numLocalDealsUS\":0,\"minPaymentPercentage\":\"4.98\",\"maxPaymentPercentage\":\"5.03\",\"averagePaymentPercentage\":\"5.00\",\"conversionRate\":0.11999999731779,\"epc\":0,\"merchantWeight\":1.0005601644516,\"averageCommission\":0,\"dateActive\":\"2013-11-19\"}],\"page\":1,\"limit\":1,\"totalRecords\":1,\"totalRecordsAvailable\":49351,\"totalRecordsFound\":49351}";

		public class MerchantJData : PBaseChannel.BaseJData
		{
			public MerchantJData()
			{
				data = new List<PMerchantRecord>();
			}
			public IList<PMerchantRecord> data { get; private set; }
		}

		public PMerchantChannel()
		{
			jData = new MerchantJData(); // empty channel...
		}

		public MerchantJData JData
		{
			get
			{
				return (jData as MerchantJData);
			}
		}

		public IList<PMerchantRecord> ListOfMerchants
		{
			get
			{
				return JData.data;
			}
		}

		public override bool doparseJson(String pJson) {
			// perform validations to ensure it is actually a Merchant Json string?
			jData = JsonConvert.DeserializeObject<MerchantJData>(pJson);
			return true;
		}

		public static string cfilterMerchant = "filterMerchant";
		public static string cfilterCategory = "filterCategory";
		public static string cfilterNumCouponsUS = "filterNumCouponsUS";
		public static string cfilterNumLocalDealsUS = "filterNumLocalDealsUS";
		public static string climit = "limit";

		public override string GetBaseUrl()
		{
			return @"http://api.prosperent.com/api/merchant?";
		}

	}
}
