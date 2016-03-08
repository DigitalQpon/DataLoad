using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InternalQponPanel
{
	public class AzureSubscription
	{
		private string subscriptionId = null;
		private X509Certificate2 certificate = null;

		public AzureSubscription(string SubscriptionId, string CertificateThumbprint)
		{
			if (string.IsNullOrEmpty(SubscriptionId))
			{
				throw new ArgumentNullException("SubscriptionId is null or empty.");
			}

			this.subscriptionId = SubscriptionId;
			this.certificate = GetCertificate(CertificateThumbprint);
		}

		public string SubscriptionId { get { return this.subscriptionId; } }

		public X509Certificate2 ManagementCertificate { get { return certificate; } }

		// Looks for the certificate by thumbprint in the "My" certificate store.
		private X509Certificate2 GetCertificate(string thumbprint)
		{
			List<StoreLocation> locations = new List<StoreLocation> { 
            StoreLocation.CurrentUser, 
            StoreLocation.LocalMachine };

			foreach (var location in locations)
			{
				X509Store store = new X509Store("My", location);
				try
				{
					store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
					X509Certificate2Collection certificates = store.Certificates.Find(
					  X509FindType.FindByThumbprint, thumbprint, false);
					if (certificates.Count == 1)
					{
						return certificates[0];
					}
				}
				finally
				{
					store.Close();
				}
			}

			throw new ArgumentException(string.Format(
			  "A certificate with thumbprint '{0}' could not be located.",
			  thumbprint));
		}
	}
}
