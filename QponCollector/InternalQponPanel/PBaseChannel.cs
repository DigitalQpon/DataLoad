using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace InternalQponPanel
{
	/// <summary>
	/// A channel for each type of request - Merchants, Brands, Categories, and Products
	/// </summary>
	public class PBaseChannel
	{

		#region the channels communication status "BaseJData"
		/// <summary>
		/// The "BaseJData" class is used to envelope all returned Json objects
		/// the object helps keep track of request submissions, etc.
		/// Currently these are constructed around the "prosperent" api
		/// I have resorted to 2 property naming formats Capitalized and lowercase.
		/// </summary>
		public class BaseJData : JRecord
		{
			public BaseJData()
			{
				errors = new List<Dictionary<string, string>>();
				warnings = new List<Dictionary<string, string>>();
				totalRecords = 0;
				page = -1;
			}

			public IList<Dictionary<string, string>> errors { get; private set; }
			public IList<Dictionary<string, string>> warnings { get; private set; }
			public int page { get; set; }
			public int limit;
			public int totalRecords;
			public int totalRecordsAvailable;
			public int totalRecordsFound;
			//
			public bool MorePages;
		}

		#endregion

		public PBaseChannel()
		{
		}

		public BaseJData jData { get; set; }
		public bool ErrorInFetch
		{
			get
			{
				return jData.errors.Count > 0;
			}
		}


		public String FetchErrorMessage
		{
			get
			{
				StringBuilder _msg = new StringBuilder();
				foreach (var _lItem in jData.errors)
				{
					foreach (KeyValuePair<string, string> dItem in _lItem)
					{
						_msg.Append(dItem.Key).Append("-").Append(dItem.Value).Append(";");
					}
				}
				return _msg.ToString();
			}
		}

		public string filterfromDictionary(Dictionary<string, string> pFilter)
		{
			string _addOn = "";
			foreach (KeyValuePair<string, string> _item in pFilter)
			{
				if (_item.Value.Trim() != "")
					_addOn = string.Format("{0}{1}={2}&", _addOn, _item.Key, WebUtility.HtmlEncode(_item.Value));
			}
			return _addOn;
		}

		public virtual String GetBaseUrl() 
		{
			return "";
		}

		/// <summary>
		/// saving the last used url for pagination
		/// </summary>
		protected String LastFetchUrl;

		/// <summary>
		/// converted the filter parameter into a valid url segment and then makes teh call to fetch data
		/// has been virtualized in case the new class needs to override behaviour
		/// </summary>
		/// <param name="pFilterList">the query filter is based on this list for first time calls</param>
		/// <param name="pFetchNextPage">If set then the previous filter is applied with the page set to the last page + 1 
		/// (no reversal for now but can be changed easily)</param>
		/// <returns></returns>
		public virtual bool doFetchData(Dictionary<string, string> pFilterList, bool pFetchNextPage)
		{
			String _fetchUrl = GetBaseUrl();
			// todo: control timeout information, etc.
			using (WebClient _web = new WebClient())
			{
				//perhaps we should compare the 
				pFetchNextPage = pFetchNextPage ? jData.page > 0 : pFetchNextPage;
				if (!pFetchNextPage)
				{
					_fetchUrl = string.Format("{0}{1}", _fetchUrl, filterfromDictionary(pFilterList));
				}
				else
				{
					_fetchUrl = String.Format("{0}&page={1}", LastFetchUrl, jData.page + 1);
				}
				Console.WriteLine(_fetchUrl);
				doparseJson(_web.DownloadString(_fetchUrl));
				Console.WriteLine(String.Format("Post fetch stats: Page {0}, totalRecords {1}, totalRecordsAvailable {2}, totalRecordsFound {3}", 
					jData.page, jData.totalRecords, jData.totalRecordsAvailable, jData.totalRecordsFound));
				if (!pFetchNextPage)
					LastFetchUrl = _fetchUrl;
				jData.MorePages = jData.totalRecords >= jData.limit;
			}
			return true;
		}

		public virtual bool doparseJson(String pJson)
		{
			return false;
		}

		public virtual bool SaveInformationToRepository(Object pRecordParam) 
		{
			// save information based on Record Parameter
			
			return true;
		}
	}
}
