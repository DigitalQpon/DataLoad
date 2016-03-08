using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalQponPanel
{

	public class PMerchantRecord : JRecord
	{
		public String Merchant;
		public String MerchantId;
		public String LogoUrl;
		public String Color1;
		public String Color2;
		public String Domain;
		public String Category;
		public String Description;
		public String ProductDataFeed;
		public String DeepLinking;
		public int NumProducts;
		public int NumProductsCA;
		public int NumProductsUK;
		public int NumCouponsUS;
		public int NumTravelOffersUS;
		public int NumLocalDealsUS;
		public string MinPaymentPercentage;
		public string MaxPaymentPercentage;
		public string AveragePaymentPercentage;
		public double ConversionRate;
		public double EPC;
		public double MerchantWeight;
		public double AverageCommission;
		public DateTime DateActive;
	}

}
