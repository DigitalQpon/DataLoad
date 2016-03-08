using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InternalQponPanel
{
	[DataContract(
		Namespace = "http://schemas.microsoft.com/windowsazure",
		Name = "Location")]
	public class DataCenterLocation
	{
		[DataMember(Order = 1)]
		public string Name { get; set; }

		[DataMember(Order = 2)]
		public string DisplayName { get; set; }

		[DataMember(Order = 3)]
		public Services AvailableServices { get; set; }

        [DataMember(Order = 4)]
        public Services Models { get; set; }
    }

	[CollectionDataContract(
		Namespace = "http://schemas.microsoft.com/windowsazure")]
	public class Locations : Collection<DataCenterLocation>
	{
	}

	[CollectionDataContract(
		Namespace = "http://schemas.microsoft.com/windowsazure",
		ItemName = "AvailableService")]
	public class Services : Collection<string>
	{
	}
}


/*
 * HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);j
 */