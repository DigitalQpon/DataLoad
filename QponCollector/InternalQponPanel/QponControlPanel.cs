using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Helpers;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;
using System.IO;



namespace InternalQponPanel
{
	public partial class QponControlPanel : Form
	{
		PMerchantChannel _mer = new PMerchantChannel();
		PBrandChannel _brand = new PBrandChannel();
		PProductChannel _product = new PProductChannel();

		public QponControlPanel()
		{
			InitializeComponent();
			//subscription = new AzureSubscription(subscriptionId, certThumbprint);
		}

		void StatusMessage(String pMessage)
		{
			messageLB.Text = pMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void processCmd_Click(object sender, EventArgs e)
		{
			PBaseChannel _channelUsed = null;

			if (mainPC.SelectedTab == prosperantMerchantTB)
			{
				fetchMerchants();
				_channelUsed = _mer;
			}
			if (mainPC.SelectedTab == prosBrandTB)
			{
				fetchBrands();
				_channelUsed = _brand;
			}
			if (mainPC.SelectedTab == pProductsTB)
			{
				if (prospMerchantList.GetItemCount() == 0 && prosBrandList.GetItemCount() == 0)
				{
					MessageBox.Show("Need to have a merchant or brand selection active");
					return;
				}
				fetchProducts();
				_channelUsed = _product;
			}
			if (_channelUsed != null)
			{
				if (_channelUsed.ErrorInFetch)
					StatusMessage(_channelUsed.FetchErrorMessage);
				else
					StatusMessage(String.Format("Post fetch stats: MorePages? {4}; Page {0}, totalRecords {1}, totalRecordsAvailable {2}, totalRecordsFound {3}", 
						_channelUsed.jData.page, _channelUsed.jData.totalRecords, _channelUsed.jData.totalRecordsAvailable, 
						_channelUsed.jData.totalRecordsFound, _channelUsed.jData.MorePages));
			}
		}

		private void fetchProducts()
		{
			String _activeBrand = "";
			String _activeMerchant = "";
			if (prospMerchantList.GetItemCount() > 0)
			{
				_activeMerchant = (prospMerchantList.SelectedObject as PMerchantRecord).Merchant;
			}
			if (prosBrandList.GetItemCount() > 0)
			{
				_activeBrand = (prosBrandList.SelectedObject as PBrandRecord).Brand;
			}
			_product.doFetchData(new Dictionary<string, string>{
					{PProductChannel.climit, mFetchLimitED.Text},
					{PProductChannel.cfilterCategory, mCategoryED.Text}, 
					{PProductChannel.cfilterMerchant, _activeMerchant/*mMerchantED.Text*/},
					{PProductChannel.cfilterBrand, _activeBrand/*brandED.Text*/},
					{PProductChannel.cfilterKeyword, keywordsED.Text}
				}, fetchNextOP.Checked);
			pProductList.SetObjects(_product.ListOfProducts);
			fetchNextOP.Checked = true;
		}

		private void fetchBrands()
		{
			_brand.doFetchData(new Dictionary<string, string>{
					{PBrandChannel.climit, mFetchLimitED.Text},
					{PBrandChannel.cfilterBrand, brandED.Text}
				}, fetchNextOP.Checked);
			prosBrandList.SetObjects(_brand.ListOfBrands);
			fetchNextOP.Checked = true;
		}

		private void fetchMerchants()
		{
			_mer.doFetchData(new Dictionary<string, string>{
					{PMerchantChannel.climit, mFetchLimitED.Text},
					{PMerchantChannel.cfilterCategory, mCategoryED.Text}, 
					{PMerchantChannel.cfilterMerchant, mMerchantED.Text},
					{PMerchantChannel.cfilterNumCouponsUS, minCouponED.Value>0? minCouponED.Value.ToString():""},
					{PMerchantChannel.cfilterNumLocalDealsUS, minDealsED.Value>0?minDealsED.Value.ToString():""}
				}, fetchNextOP.Checked);
			prospMerchantList.SetObjects(_mer.ListOfMerchants);
			fetchNextOP.Checked = true;
		}

		private void pProductList_SelectedIndexChanged(object sender, EventArgs e)
		{
			//refresh coupons
			if (pProductList.SelectedObject != null)
			{
				PProductRecord _product = (pProductList.SelectedObject as PProductRecord);
				prodCouponWB.Url = new Uri(_product.image_url);
				//couponList.SetObjects(_product.coupons);
			}
		}

		private void mainPC_SelectedIndexChanged(object sender, EventArgs e)
		{
			fetchNextOP.Checked = (mainPC.SelectedTab.Tag == null || mainPC.SelectedTab.Tag.ToString() == "0") ? false : true;
		}

		private void prospMerchantList_SelectedIndexChanged(object sender, EventArgs e)
		{
			//dont fetch next for products...
			pProductsTB.Tag = 0;
		}

		private void couponList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (couponList.SelectedObject != null)
			{
				PProductRecord.PProductCoupons _coupon = (couponList.SelectedObject as PProductRecord.PProductCoupons);
				couponWB.Url = new Uri(_coupon.image_url);
			}
		}

		private void fetchProductsCM_Click(object sender, EventArgs e)
		{
			mainPC.SelectedTab = pProductsTB;
			processCmd_Click(sender, e);
		}

		private void filterTextChanged(object sender, EventArgs e)
		{
			fetchNextOP.Checked = false;
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{

		}

		private void fetchNextOP_CheckedChanged(object sender, EventArgs e)
		{
			mainPC.SelectedTab.Tag = fetchNextOP.Checked ? -1 : 0;
		}

        public object Deserialize(string jsonText, Type valueType)
        {
            Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();

            json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
            json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            StringReader sr = new StringReader(jsonText);
            Newtonsoft.Json.JsonTextReader reader = new JsonTextReader(sr);
            object result = json.Deserialize(reader, valueType);
            reader.Close();

            return result;
        }

        //QponCrush15
		private void azureTestCM_Click(object sender, EventArgs e)
		{
            if (pProductList.SelectedObject != null)
            {
                PProductRecord _product = (pProductList.SelectedObject as PProductRecord);
                string _json = JsonConvert.SerializeObject(_product);
                DataSet _ds = JsonConvert.DeserializeObject<DataSet>(_json);
                Console.WriteLine(_ds.Tables[0]);
            }
        }

	}
}
