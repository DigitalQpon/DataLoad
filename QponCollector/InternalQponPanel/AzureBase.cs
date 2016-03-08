using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace InternalQponPanel
{
	public class AzureBase
	{
		private AzureSubscription azureSubscription = null;

		public AzureBase(AzureSubscription Subscription)
		{
			if (Subscription == null)
			{
				throw new ArgumentNullException(
					"Subscription", "Subscription parameter cannot be null.");
			}

			this.azureSubscription = Subscription;
		}

		// Base Uri for all Service Management APIs
		public virtual Uri ServiceManagementUri
		{
			get
			{
				return new Uri(
					string.Format("https://management.core.windows.net/{0}",
					azureSubscription.SubscriptionId));
			}
		}

		// Returns a WebRequestHandler with the AzureSubscriptions
		// management certificate already in the client certificates collection.
		public WebRequestHandler RequestHandler
		{
			get
			{
				var handler = new WebRequestHandler();
				handler.ClientCertificates.Add(
					this.azureSubscription.ManagementCertificate);
				return handler;
			}
		}

		// Returns an HttpClient instance using the RequestHandler for this instance.
		// Sets the base address and common headers for all services APIs.
		// Sets up the Accept header for xml format.
		public HttpClient HttpClientInstance
		{
			get
			{
				var httpClient = new HttpClient(this.RequestHandler);
				httpClient.BaseAddress = ServiceManagementUri;
				httpClient.DefaultRequestHeaders.Add("x-ms-version", "2013-03-01");
				httpClient.DefaultRequestHeaders.Accept.Add(
					new MediaTypeWithQualityHeaderValue("application/xml"));

				return httpClient;
			}
		}
	}
}
