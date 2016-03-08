using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace InternalQponPanel
{
	public class LocationsApi : AzureBase
	{
		public LocationsApi(AzureSubscription Subscription)
			: base(Subscription)
		{ }

		public override Uri ServiceManagementUri
		{
			get
			{
				return new Uri(string.Format("{0}/locations",
					base.ServiceManagementUri));
			}
		}

		public Locations List()
		{
			// Invoke REST API
			HttpResponseMessage response = this.HttpClientInstance.GetAsync("").Result;
			response.EnsureSuccessStatusCode();
			List<MediaTypeFormatter> formatters = new List<MediaTypeFormatter>(){
                new XmlMediaTypeFormatter()
            };

			return response.Content.ReadAsAsync<Locations>(formatters).Result;
		}
	}
}
