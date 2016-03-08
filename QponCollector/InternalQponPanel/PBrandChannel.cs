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
	public class PBrandChannel : PBaseChannel
	{
		public class BrandJData : PBaseChannel.BaseJData
		{
			public BrandJData()
			{
				data = new List<PBrandRecord>();
			}
			public IList<PBrandRecord> data { get; private set; }
		}
		public PBrandChannel()
		{
			jData = new BrandJData(); // empty channel...
		}

		public BrandJData JData
		{
			get
			{
				return (jData as BrandJData);
			}
		}

		public IList<PBrandRecord> ListOfBrands
		{
			get
			{
				return JData.data;
			}
		}

		public override bool doparseJson(String pJson) {
			// perform validations to ensure it is actually a Merchant Json string?
			jData = JsonConvert.DeserializeObject<BrandJData>(pJson);
			return true;
		}

		public static string cfilterBrand = "filterBrand";
		public static string climit = "limit";

		public override string GetBaseUrl()
		{
			return @"http://api.prosperent.com/api/brand?";
		}
	}
}
