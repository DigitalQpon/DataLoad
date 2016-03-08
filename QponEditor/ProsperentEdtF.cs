using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

namespace QponEditor
{
    public partial class ProsperentEdtF : Form
    {
        // retaining the string for refrence and for ease of coding
        static String _connStr = @"Server=tcp:oud11hbwrc.database.windows.net,1433;Database=intermediate-qponcrush_db;User ID=saviobernard@oud11hbwrc;Password=QponCrush15;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
        SqlConnection _AzureConn = new SqlConnection(_connStr);
        QponFeed _Feed = new QponFeed();
        QponRegisterAdmin _DefaultAccessToken = null;
        String _productFilterString = "";
        bool _loginCompleted = false;
        int _lastProductRow = -1;
        int _lastMerchantRow = -1;
        int _lastFmMerchantRow = -1;
        const String _cProspPasswordSuff = ""; //".Prosp"; // todo:add this when the prosperent list is re-established (cleared from the App database)
        const String _cFmtcPasswordSuff = ".Fmtc";
        const String _cCategoryList = "CategoryList";

        public List<DataRow> ARows = new List<DataRow>(); 



        public void AddNewCategories(DataTable _catTbl)
        {
            DataRow _catRow;

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 0;
            _catRow["stringKey"] = "";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 2;
            _catRow["stringKey"] = "Women's Apparel";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 3;
            _catRow["stringKey"] = "Men's Apparel";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 4;
            _catRow["stringKey"] = "Baby & Kid’s Apparel";


            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 5;
            _catRow["stringKey"] = "Events & Offers";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 6;
            _catRow["stringKey"] = "Home & Garden";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 7;
            _catRow["stringKey"] = "Electronics";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 8;
            _catRow["stringKey"] = "Health & Beauty";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 10;
            _catRow["stringKey"] = "Bed & Bath";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 11;
            _catRow["stringKey"] = "Travel";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 13;
            _catRow["stringKey"] = "Automotive";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 14;
            _catRow["stringKey"] = "Pets";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 15;
            _catRow["stringKey"] = "Toys";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 16;
            _catRow["stringKey"] = "Women's Shoes";  

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 17;
            _catRow["stringKey"] = "Sports & Recreation";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 18;
            _catRow["stringKey"] = "Jewelry";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 19;
            _catRow["stringKey"] = "Fragrance";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 20;
            _catRow["stringKey"] = "Restaurants";

            _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 21;
            _catRow["stringKey"] = "Accesories";
   
            
        }

        public ProsperentEdtF()
        {
            InitializeComponent();
            _DefaultAccessToken = _Feed.DefaultAccessToken();
            Console.WriteLine(_DefaultAccessToken.admin.AccessToken);
            // 
            SqlCommand _cmd = null;
            SqlDataAdapter _sqlDa = null;
            // category list from Swagger API will be stored in a a generic table for <int, string> keyPair values
            _cmd = new SqlCommand("SELECT intValue, stringKey FROM [dbo].[ListInt] WHERE [listType] = 1", _AzureConn);
            _sqlDa = new SqlDataAdapter(_cmd);
            _sqlDa.Fill(this.tempDS, _cCategoryList); // create a temporary table to hold values from the swagger repository
            JArray _lCats = _Feed.getCategoryList();
            // first add empty item to allow for new records in tables
            DataTable _catTbl = this.tempDS.Tables[_cCategoryList];
            DataRow _catRow = _catTbl.Rows.Add();
            _catRow["intValue"] = 0;
            _catRow["stringKey"] = "";
            foreach (JObject _row in _lCats)
            {
                _catRow = _catTbl.Rows.Add();
                _catRow["intValue"] = _row.Value<int>("CouponCategoryId");
                _catRow["stringKey"] = _row.Value<String>("CategoryName");
            } 

            // Add New

          //  AddNewCategories(_catTbl);


            // set the category combo boxes all over
            // prosperent grid
            qponCategoryProsp.DataSource = this.tempDS.Tables[_cCategoryList];
            qponCategoryProsp.DisplayMember = "stringKey";
            qponCategoryProsp.ValueMember = "stringKey";
            // prosperent default view
            pCategoryEdt.DataSource = this.tempDS.Tables[_cCategoryList];
            pCategoryEdt.DisplayMember = "stringKey";
            pCategoryEdt.ValueMember = "stringKey";
            // fmtc grid
            qponCategoryfmDeals.DataSource = this.tempDS.Tables[_cCategoryList];
            qponCategoryfmDeals.DisplayMember = "stringKey";
            qponCategoryfmDeals.ValueMember = "stringKey";
            // fmtc default view
            fCategoryEdt.DataSource = this.tempDS.Tables[_cCategoryList];
            fCategoryEdt.DisplayMember = "stringKey";
            fCategoryEdt.ValueMember = "stringKey";
            // get user list to be used in the login and other sections of the forms
            _cmd = new SqlCommand("SELECT * FROM [dbo].[UserList] WHERE [isActive] = 1", _AzureConn);
            _sqlDa = new SqlDataAdapter(_cmd);
            _sqlDa.Fill(this.tempDS.UserList);
        }

        private void ProsperentEdtF_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qponQrushb22DataSet.BaseCoupon' table. You can move, or remove it, as needed.
            this.baseCouponTableAdapter1.Fill(this.qponQrushb22DataSet.BaseCoupon);
            // TODO: This line of code loads data into the 'qponCrushDataSet1.BaseCoupon' table. You can move, or remove it, as needed.
            this.baseCouponTableAdapter.Fill(this.qponCrushDataSet1.BaseCoupon);
            // TODO: This line of code loads data into the 'tempDS.CategoryMap' table. You can move, or remove it, as needed.
        }

        private void openAzureConn()
        {
            if (_AzureConn.State != ConnectionState.Open)
                _AzureConn.Open();
        }

        void StatusMessage(String pMsg)
        {
            Console.WriteLine(pMsg);
        }

        #region Calling the Azure API as defined in the swagger site to link elements in the tables to the swagger objects

        /*
         * ProsperentMerchant => Vendor
         * Does the actual linking of the row in the ProsperentMerchant table to the vendorId as used by the App API
         * There does not seem to be a way of removing/updating the swagger record via the API calls?
         */
        void linkMerchantToVendor(DataRow pMerchantRow)
        {
            RegisterVendorJS _vData = new RegisterVendorJS();
            // link only if there is an established vendor id returned from the APP calls
            if (pMerchantRow["vendorId"].ToString() == "" /* _row.Field<string>("vendorId").CompareTo("") == 0*/ )
            {
                _vData.Name = pMerchantRow.Field<string>("merchant");
                _vData.Password = pMerchantRow.Field<string>("merchantId") + _cProspPasswordSuff;
                _vData.WebsiteUrl = pMerchantRow.Field<string>("domain");
                _vData.Email = string.Format("{0}@{0}.com", pMerchantRow.Field<string>("merchantId"));
                _vData.LogoImageURL = pMerchantRow.Field<string>("logoUrl"); 
                StatusMessage(String.Format("Registering vendor {0}", _vData.Name));
                QponResponse _resp = _Feed.doRegisterVendor(_vData);
                if (_resp.success)
                {
                    JToken _vendor = _resp.data.Value<JToken>("vendor");
                    if (_vendor != null)
                    {
                        String _vendorId = _vendor.Value<string>("VendorId");
                        Console.WriteLine("_vendor.VendorId = " + _vendorId);
                        SqlCommand _cmdVA = new SqlCommand("UPDATE [dbo].[ProsperentMerchant] SET VendorId = @vendorId WHERE merchantId = @merchantId");
                        _cmdVA.Connection = _AzureConn;
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@vendorId", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@merchantId", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters["@merchantId"].Value = pMerchantRow.Field<string>("merchantId");
                        _cmdVA.Parameters["@vendorId"].Value = _vendorId;
                        _cmdVA.ExecuteNonQuery();
                    }
                }
            }
        }

        /*
         * ProsperentProduct => coupon
         * Does the actual linking of the row in the ProsperentProduct table to the Coupon as used by the App API
         * TODO: Expiry dates not always available...so how do we eliminate older records? Using period based algo?
         */

        private bool linkBaseCouponIdToProduct(DataRow pProductRow)
        {
            if (pProductRow.Field<string>("baseCouponId") != null)
            {
                /*
                 * todo: dont save / update coupon already in system as there is no mechanism to update and sending the information will create a new coupon
                 * todo: do we want to allow that?
                 * */
                String _msg = String.Format("Coupon already created for for {0} {1} {2}", pProductRow.Field<string>("merchant"),
                    pProductRow.Field<string>("productId"), pProductRow.Field<string>("baseCouponId"));
                MessageBox.Show(_msg);
                return false;
            }
            if (pProductRow.Field<string>("qponCategory") == null)
            {
                String _msg = String.Format("No Qpon category supplied for {0} {1} {2}", pProductRow.Field<string>("merchant"),
                    pProductRow.Field<string>("productId"), pProductRow["baseCouponId"].ToString());
                MessageBox.Show(_msg);
                return false;
            }
            BaseCouponJD _vData = new BaseCouponJD();
            SqlCommand _cmd = new SqlCommand("SELECT * FROM [dbo].[ProsperentMerchant] WHERE [merchantId] = @merchantid", _AzureConn);
            _cmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@merchantid", System.Data.SqlDbType.NVarChar, 100)));
            _cmd.Parameters["@merchantid"].Value = pProductRow.Field<string>("merchantid");
            DataTable _merchantTbl = new DataTable();
            SqlDataAdapter _sqlDa = new SqlDataAdapter(_cmd);
            _sqlDa.Fill(_merchantTbl);
            if (_merchantTbl.Rows.Count == 0) // add merchat information
            {
                String _msg = String.Format("Merchant {0} is not an 'accepted' vendor", pProductRow.Field<string>("merchant"));
                MessageBox.Show(_msg);
                return false;
            }
            // only prooceed for merchants with an established vendor id
            if (_merchantTbl.Rows[0]["vendorId"].ToString() != "" /* _row.Field<string>("vendorId").CompareTo("") == 0*/ )
            {
                _vData.VendorId = _merchantTbl.Rows[0].Field<string>("VendorId");
                _vData.BarCode = pProductRow.Field<string>("upc");
                _vData.Title = pProductRow.Field<string>("keyword");
                _vData.PurchaseUrl = pProductRow.Field<string>("affiliate_url");
                _vData.Description = pProductRow.Field<string>("description");
                _vData.Terms = "none";
                _vData.CouponType = "Fixed Value";
                StatusMessage(String.Format("Adding Product {0}", pProductRow.Field<string>("description")));
                _vData.CategoryType = pProductRow.Field<string>("qponCategory");
                if (!pProductRow.IsNull("price_sale"))
                {
                    _vData.DiscountType = "Dollar";
                    _vData.BaseValue = Convert.ToInt32(Double.Parse(pProductRow["price_sale"].ToString(), System.Globalization.NumberStyles.Currency));
                    _vData.MaxValue = Convert.ToInt32(Double.Parse(pProductRow["price_sale"].ToString(), System.Globalization.NumberStyles.Currency));
                }
                // percent takes precedence over value
                if (!pProductRow.IsNull("percentOff"))
                {
                    _vData.DiscountType = "Percent";
                    Console.WriteLine(pProductRow["percentOff"]);
                    _vData.BaseValue = Convert.ToInt32(Double.Parse(pProductRow["percentOff"].ToString(), System.Globalization.NumberStyles.Currency));
                    _vData.MaxValue = Convert.ToInt32(Double.Parse(pProductRow["percentOff"].ToString(), System.Globalization.NumberStyles.Currency));
                }
                _vData.ShareType = "Group";
                _vData.ExpiryType = "Date";
                _vData.MaximumIssue = 0;
                DateTime _expiryDate = DateTime.Today.AddDays(30); // Prosperent items will expire 30 days from start
                _vData.expiryDate = String.Format("{0:yyyy-MM-dd}", _expiryDate);
                _vData.ProductImageUrl = pProductRow.Field<string>("image_url");
                _vData.AccessToken = _DefaultAccessToken.admin.AccessToken;
                _vData.VideoUrl = "";
                _vData.logoImageUrl = _merchantTbl.Rows[0].Field<string>("logoUrl");
                QponResponse _resp = _Feed.doAddbaseCouponAdmin(_vData);
                if (_resp.success)
                {
                    JToken _coupon = _resp.data.Value<JToken>("coupon");
                    if (_coupon != null)
                    {
                        String _couponId = _coupon.Value<string>("BaseCouponId");
                        Console.WriteLine("BaseCouponId = " + _couponId);
                        SqlCommand _cmdVA = new SqlCommand(@"UPDATE [dbo].[ProsperentProduct] SET BaseCouponId = @BaseCouponId, 
                                qponCategory = @qponCategory, expiryDate = @expiryDate, updateStatus = @updateStatus
                                WHERE catalogId = @catalogId");
                        _cmdVA.Connection = _AzureConn;
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@BaseCouponId", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@catalogId", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@qponCategory", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@expiryDate", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));
                        _cmdVA.Parameters["@catalogId"].Value = pProductRow.Field<string>("catalogId");
                        _cmdVA.Parameters["@updateStatus"].Value = pProductRow.Field<int>("updateStatus");
                        _cmdVA.Parameters["@qponCategory"].Value = pProductRow.Field<string>("qponCategory");
                        _cmdVA.Parameters["@BaseCouponId"].Value = _couponId;
                        _cmdVA.Parameters["@expiryDate"].Value = _expiryDate;
                        _cmdVA.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        /*
         * FmtcMerchant => Vendor (Azure)
         * */
        void linkFmMerchantToVendor(DataRow pMerchantRow)
        {
            RegisterVendorJS _vData = new RegisterVendorJS();
            // the usual - only is vendor id was recoered from APP
            if (pMerchantRow["vendorId"].ToString() == "" /* _row.Field<string>("vendorId").CompareTo("") == 0*/ )
            {
                _vData.Name = pMerchantRow.Field<string>("cName");
                _vData.Password = pMerchantRow.Field<int>("nMerchantID").ToString() + _cFmtcPasswordSuff;
                _vData.WebsiteUrl = pMerchantRow.Field<string>("cHomepageURL");
                _vData.Email = string.Format("{0}@{0}.fmtc", pMerchantRow.Field<int>("nMerchantID").ToString());
                _vData.LogoImageURL = pMerchantRow.Field<string>("logoUrl");
                StatusMessage(String.Format("Registering vendor {0} {1}", _vData.Name, _vData.Email));
                QponResponse _resp = _Feed.doRegisterVendor(_vData);
                if (_resp.success)
                {
                    JToken _vendor = _resp.data.Value<JToken>("vendor");
                    if (_vendor != null)
                    {
                        String _vendorId = _vendor.Value<string>("VendorId");
                        Console.WriteLine("_vendor.VendorId = " + _vendorId);
                        SqlCommand _cmdVA = new SqlCommand("UPDATE [dbo].[FmtcMerchants] SET VendorId = @vendorId WHERE nMerchantID = @nMerchantID");
                        _cmdVA.Connection = _AzureConn;
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@vendorId", System.Data.SqlDbType.NVarChar, 100)));//NVARCHAR (100) NULL,
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@nMerchantID", System.Data.SqlDbType.NVarChar, 100)));//NVARCHAR (100) NULL,
                        _cmdVA.Parameters["@nMerchantID"].Value = pMerchantRow.Field<int>("nMerchantID");
                        _cmdVA.Parameters["@vendorId"].Value = _vendorId;
                        _cmdVA.ExecuteNonQuery();
                    }
                }
            }
        }

        /*
         * fmtcDeals => coupon
         * TODO:the deals sometimes lack the identifying logo/image??
         * TODO: Azure API calls dont seem to support the time element
         * */
        private bool linkBaseCouponIdToDeals(DataRow pDealsRow)
        {
            if (pDealsRow.Field<string>("baseCouponId") != null)
            {
                /*
                 * todo: dont save / update coupon already in system as there is no mechanism to update and sending the information will create a new coupon
                 * todo: do we want to allow that?
                 * */
                //modified:Nov/2015 from cLabel to cMerchant
                String _msg = String.Format("Coupon already created for for {0} {1} {2}", pDealsRow.Field<string>("cName"),
                    pDealsRow.Field<string>("cMerchant"), pDealsRow.Field<string>("baseCouponId"));
                MessageBox.Show(_msg);
                return false;
            }
            if (pDealsRow.Field<string>("qponCategory") == null)
            {
                String _msg = String.Format("No Qpon category supplied for {0} {1} {2}", pDealsRow.Field<string>("cName"),
                    pDealsRow.Field<string>("cLabel"), pDealsRow["fSalePrice"].ToString());
                MessageBox.Show(_msg);
                return false;
            }
            BaseCouponJD _vData = new BaseCouponJD();
            SqlCommand _cmd = new SqlCommand("SELECT * FROM [dbo].[FmtcMerchants] WHERE [nMerchantID] = @nMerchantID", _AzureConn);
            _cmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@nMerchantID", System.Data.SqlDbType.NVarChar, 100)));
            _cmd.Parameters["@nMerchantID"].Value = pDealsRow.Field<int>("nMerchantID");
            DataTable _merchantTbl = new DataTable();
            SqlDataAdapter _sqlDa = new SqlDataAdapter(_cmd);
            _sqlDa.Fill(_merchantTbl);
            if (_merchantTbl.Rows.Count == 0) // add merchat information
            {
                String _msg = String.Format("Merchant {0} is not an 'acceptedOpt' vendor", pDealsRow.Field<string>("cName"));
                MessageBox.Show(_msg);
                return false;
            }
            //todo:remove this after the databae issue is resolved
            //String _bypass = "edddee26-6ee4-4d1a-96c5-ec5723cfdab1";
            String _bypass = "";
            String _vendorId = _bypass != "" ? _bypass : _merchantTbl.Rows[0]["vendorId"].ToString();
            if (_vendorId != ""
                /* _row.Field<string>("vendorId").CompareTo("") == 0*/ )
            {

                _vData.VendorId = _vendorId;
                _vData.BarCode = pDealsRow.Field<string>("cCode");
                _vData.Title = pDealsRow.Field<string>("clabel");
                _vData.PurchaseUrl = pDealsRow.Field<string>("cAffiliateURL");
                _vData.Description = pDealsRow.Field<string>("cLabel");
                _vData.Terms = "none";
                _vData.CouponType = "Fixed Value";
                StatusMessage(String.Format("Adding Product {0}", pDealsRow.Field<string>("cLabel")));
                _vData.CategoryType = pDealsRow.Field<string>("qponCategory");
                if (!pDealsRow.IsNull("nPercent"))
                {
                    _vData.DiscountType = "Percent";
                    _vData.BaseValue = Convert.ToInt32(Double.Parse(pDealsRow["nPercent"].ToString(), System.Globalization.NumberStyles.Currency));
                    _vData.MaxValue = Convert.ToInt32(Double.Parse(pDealsRow["nPercent"].ToString(), System.Globalization.NumberStyles.Currency));
                }
                if (!pDealsRow.IsNull("fSalePrice"))
                {
                    _vData.DiscountType = "Dollar";
                    _vData.BaseValue = Convert.ToInt32(Double.Parse(pDealsRow["fSalePrice"].ToString(), System.Globalization.NumberStyles.Currency));
                    _vData.MaxValue = Convert.ToInt32(Double.Parse(pDealsRow["fSalePrice"].ToString(), System.Globalization.NumberStyles.Currency));
                }
                _vData.ShareType = "Group";
                _vData.ExpiryType = "Date";
                _vData.MaximumIssue = 0;
                DateTime _expiryDate = DateTime.Today.AddDays(30);
                _vData.expiryDate = String.Format("{0:yyyy-MM-dd}", _expiryDate);
                if (!pDealsRow.IsNull("dtEndDate"))
                {
                    _expiryDate = pDealsRow.Field<DateTime>("dtEndDate");
                    _vData.expiryDate = String.Format("{0:yyyy-MM-dd}", _expiryDate);
                }
                _vData.ProductImageUrl = pDealsRow.Field<string>("cImage");
                _vData.AccessToken = _DefaultAccessToken.admin.AccessToken;
                _vData.VideoUrl = "";
                _vData.logoImageUrl = pDealsRow.Field<string>("cImage");
                QponResponse _resp = _Feed.doAddbaseCouponAdmin(_vData);
                if (_resp.success)
                {
                    JToken _coupon = _resp.data.Value<JToken>("coupon");
                    if (_coupon != null)
                    {
                        String _couponId = _coupon.Value<string>("BaseCouponId");
                        Console.WriteLine("BaseCouponId = " + _couponId);
                        SqlCommand _cmdVA = new SqlCommand(@"UPDATE [dbo].[FmtcDeals] SET BaseCouponId = @BaseCouponId, 
                                qponCategory = @qponCategory, /*expiryDate = expiryDate, */ updateStatus = @updateStatus
                                WHERE nCouponId = @nCouponId");
                        _cmdVA.Connection = _AzureConn;
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@BaseCouponId", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@nCouponId", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@qponCategory", System.Data.SqlDbType.NVarChar, 100)));
                        // todo: expiry dates are available in the fmtc database so no need to calculate...
                        //_cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@expiryDate", System.Data.SqlDbType.NVarChar, 100)));
                        _cmdVA.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));
                        _cmdVA.Parameters["@nCouponId"].Value = pDealsRow.Field<int>("nCouponId");
                        _cmdVA.Parameters["@updateStatus"].Value = pDealsRow.Field<int>("updateStatus");
                        _cmdVA.Parameters["@qponCategory"].Value = pDealsRow.Field<string>("qponCategory");
                        _cmdVA.Parameters["@BaseCouponId"].Value = _couponId;
                        //_cmdVA.Parameters["@expiryDate"].Value = _expiryDate;
                        _cmdVA.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        #endregion Calling the Azure API...

        #region Prosperent Merchant manipulation
        /*
         * 
         */
        void updateMerchantStatusTo(DataRow pMerchantRow, int pNewStatus)
        {
            StatusMessage(String.Format("Updating Merchant Status {0}", pMerchantRow.Field<string>("merchant")));
            openAzureConn();
            SqlCommand _cmdSU = new SqlCommand("UPDATE [dbo].[ProsperentMerchant] SET updateStatus = @updateStatus, " + 
                "updateUser = @updateUser WHERE merchantId = @merchantId");
            _cmdSU.Connection = _AzureConn;
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@merchantId", System.Data.SqlDbType.NVarChar, 100)));
            _cmdSU.Parameters["@merchantId"].Value = pMerchantRow.Field<string>("merchantId");
            _cmdSU.Parameters["@updateStatus"].Value = pMerchantRow.Field<int>("updateStatus");
            _cmdSU.Parameters["@updateUser"].Value = pMerchantRow.Field<int>("updateUser");
            _cmdSU.ExecuteNonQuery();
        }

        private void saveMerchantCmd_Click(object sender, EventArgs e)
        {
            // save information to Azure
            openAzureConn();
            DataTable _changes = this.tempDS.ProsperentMerchant.GetChanges();
            if (_changes == null)
                return;
            foreach (DataRow _row in _changes.Rows)
            {
                //todo: once vendors are in the system then there is no way to remove them...which may be ok
                if (_row.Field<int>("updateStatus") == AppUtility.cAcceptedItems) 
                    linkMerchantToVendor(_row);
                // update the vendor with the acceptance/rejection information
                updateMerchantStatusTo(_row, _row.Field<int>("updateStatus"));
            }
            _AzureConn.Close();
            reloadMerchantCmd_Click(sender, e);
        }

        private void reloadMerchantCmd_Click(object sender, EventArgs e)
        {
            SqlCommand _cmd = new SqlCommand();
            // add filters...
            StatusMessage("Loading Merchant data");
            //todo: merchants are loaded only for the current user or those that have no user attached unless the user is an adminstrator?
            _cmd.CommandText = "SELECT * FROM [dbo].[ProsperentMerchant] WHERE " + 
                "(@allUser = 1 OR updateUser = @updateUser) " + 
                "AND [updateStatus] in (@newItems, @accepted, @rejected, @later) ORDER BY [merchant]";
            _cmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@allUser", System.Data.SqlDbType.Int)));
            _cmd.Parameters["@allUser"].Value = pmLimitMyMerOpt.Checked ? 0 : 1;
            _cmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));
            _cmd.Parameters["@updateUser"].Value = Convert.ToInt16(pUserAssignEdt.SelectedValue);
            //
            _cmd.Connection = _AzureConn;
            setStatusCheckFilter(_cmd, newOpt.Checked, acceptedOpt.Checked, rejectedOpt.Checked, laterOpt.Checked);
            SqlDataAdapter _sqlDa = new SqlDataAdapter(_cmd);
            openAzureConn();
            this.tempDS.ProsperentMerchant.Clear();
            _lastMerchantRow = -2;
            _sqlDa.Fill(this.tempDS.ProsperentMerchant);
            _lastMerchantRow = -1;



            StatusMessage("Merchant Data Loaded");
        }

        private void prosperentMerchantGrd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((prosperentMerchantBS.Current as DataRowView) == null || (prosperentMerchantBS.Current as DataRowView).Row == null)
                return;
            DataRow _row = (prosperentMerchantBS.Current as DataRowView).Row;
            _row["updateUser"] = Convert.ToInt16(pUserAssignEdt.SelectedValue);
            if (e.ColumnIndex == AcceptCmd.Index)
                toggleAcceptStatus((sender as DataGridView).Columns[e.ColumnIndex], _row);
        }

        private void prosperentMerchantGrd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        #endregion Prosperent Merchant manipulation

        #region Prosperent Product manipulation
        void updateProductStatusTo(DataRow pProductRow, int pNewStatus)
        {
            StatusMessage(String.Format("Updating Product Status {0}", pProductRow.Field<string>("description")));
            openAzureConn();
            SqlCommand _cmdSU = new SqlCommand("UPDATE [dbo].[ProsperentProduct] SET updateStatus = @updateStatus, updateUser = @updateUser WHERE catalogId = @catalogId");
            _cmdSU.Connection = _AzureConn;
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@catalogId", System.Data.SqlDbType.NVarChar, 100)));
            _cmdSU.Parameters["@catalogId"].Value = pProductRow.Field<string>("catalogId");
            _cmdSU.Parameters["@updateStatus"].Value = pProductRow.Field<int>("updateStatus");
            _cmdSU.Parameters["@updateUser"].Value = pProductRow.Field<int>("updateUser");
            _cmdSU.ExecuteNonQuery();
        }

        private void saveProductCmd_Click(object sender, EventArgs e)
        {
            openAzureConn();
            DataTable _changes = this.tempDS.ProsperentProduct.GetChanges();
            if (_changes == null)
                return;
            int _productCount = 0;
            foreach (DataRow _row in _changes.Rows)
            {
                if (_row.Field<int>("updateStatus") == AppUtility.cAcceptedItems)
                {
                    if (linkBaseCouponIdToProduct(_row))
                        _productCount++;
                    else
                        return;
                }
                updateProductStatusTo(_row, _row.Field<int>("updateStatus"));
            }
            _AzureConn.Close();
            reloadProductCmd_Click(sender, e);
            StatusMessage(String.Format("Added {0} Products", _productCount));
        }

        private void reloadProductCmd_Click(object sender, EventArgs e)
        {
            SqlCommand _cmd = new SqlCommand();
            // add filters...
            /*
                    /*AND
                    (@allMerchants = 1 OR (merchantId in 
                    (SELECT MerchantId from [dbo].[ProsperentMerchant] WHERE updateStatus = 2 AND vendorId IS NOT NULL)))
             * */
            _cmd.CommandText = String.Format("SELECT * FROM [dbo].[prosperentProduct] P WHERE " +
                (accProductsOpt.Checked ? " (baseCouponId is NULL) AND " : "") +
                    " [updateStatus] in (@newItems, @accepted, @rejected, @later) {0}", getProductFilterString());
            //_cmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@allMerchants", 1 /* (validMerchantOpt.Checked ? 0 : 1)*/)));
            setStatusCheckFilter(_cmd, newProductsOpt.Checked, accProductsOpt.Checked, rejProductsOpt.Checked, laterProductsopt.Checked);
            _cmd.Connection = _AzureConn;
            SqlDataAdapter _sqlDa = new SqlDataAdapter(_cmd);
            openAzureConn();
            this.tempDS.ProsperentProduct.Clear();
            _lastProductRow = -2;
            _sqlDa.Fill(this.tempDS.ProsperentProduct);
            _AzureConn.Close();
            _lastProductRow = -1;
        }

        private string getProductFilterString()
        {
            StringBuilder _filter = new StringBuilder(" "); // cater to "and" in query
            if (filterDataEdt.Text.Trim() != "")
                _filter.Append(String.Format("AND (merchant = \'{0}\')", filterDataEdt.Text.Trim()));
            if (discountedOpt.Checked)
                _filter.Append(String.Format("{0} (ISNULL(price_sale, 0) <> 0 OR ISNULL(percentOff, 0) <> 0)", _filter.Length > 0 ? "AND " : ""));
            if (pLimitMyMerOpt.Checked)
                _filter.Append(String.Format("{0} (EXISTS (SELECT 1 FROM [dbo].ProsperentMerchant M WHERE M.merchantId = P.merchantId AND ISNULL(updateUser, 0) = {1}))",
                    _filter.Length > 0 ? "AND " : "", pUserAssignEdt.SelectedValue.ToString()));
            return _filter.ToString();
        }

        private void updateProductInfo(DataRow pProductRow)
        {
            toggleAcceptStatus(updateStatusText, pProductRow);
            //productGrd.Columns[qponCategoryProsp.Index].Selected = true;
            if (pCategoryEdt.SelectedValue.ToString() != "")
                pProductRow["qponCategory"] = pCategoryEdt.SelectedValue;
        }

        private void productGrd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            productGrd.Rows[e.RowIndex].Selected = true;
            if ((prosperentProductBS.Current as DataRowView) == null || (prosperentProductBS.Current as DataRowView).Row == null)
                return;
            DataRow _currentRow = (prosperentProductBS.Current as DataRowView).Row;
            _currentRow["updateUser"] = Convert.ToInt16(pUserAssignEdt.SelectedValue);
            if (e.ColumnIndex == updateStatusText.Index)
                updateProductInfo(_currentRow);
        }

        private void productGrd_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void filterDataEdt_DoubleClick(object sender, EventArgs e)
        {
        }

        private void productGrd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // let grid do sort...
            /*
            if (e.ColumnIndex != updateStatus.Index && e.ColumnIndex != qponCategory.Index)
            {
                _productFilterString = productGrd.Columns[e.ColumnIndex].DataPropertyName;
                reloadProductCmd_Click(reloadProductCmd, null);
            }
             * */
        }

        private void productGrd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }
        #endregion Prosperent Product manipulation

        #region Fmtc Merchant manipulation
        private void loadFmtcMerch_Click(object sender, EventArgs e)
        {
            SqlCommand _cmd = new SqlCommand();
            // add filters...
            StatusMessage("Loading Fmtc Merchant data");
            _cmd.CommandText = "SELECT * FROM [dbo].[FmtcMerchants] WHERE " +
                "(@allUser = 1 OR updateUser = @updateUser) " +
                " AND [updateStatus] in (@newItems, @accepted, @rejected, @later) ORDER BY [cName]";
            _cmd.Connection = _AzureConn;
            _cmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@allUser", System.Data.SqlDbType.Int)));
            _cmd.Parameters["@allUser"].Value = fmLimitMyMerOpt.Checked ? 0 : 1;
            _cmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));
            _cmd.Parameters["@updateUser"].Value = Convert.ToInt16(fUserAssignEdt.SelectedValue);

            setStatusCheckFilter(_cmd, fmNewOpt.Checked, fmAccOpt.Checked, fmRejOpt.Checked, fmLaterOpt.Checked);
            SqlDataAdapter _sqlDa = new SqlDataAdapter(_cmd);
            openAzureConn();
            this.tempDS.FmtcMerchants.Clear();
            _lastFmMerchantRow = -2;
            _sqlDa.Fill(this.tempDS.FmtcMerchants);
            _lastFmMerchantRow = -1;
            StatusMessage("Merchant Data Loaded");
        }

        private void fmMercGrd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((fmtcMerchantsBS.Current as DataRowView) == null || (fmtcMerchantsBS.Current as DataRowView).Row == null)
                return;
            DataRow _row = (fmtcMerchantsBS.Current as DataRowView).Row;
            _row["updateUser"] = Convert.ToInt16(pUserAssignEdt.SelectedValue);
            if (e.ColumnIndex == AcceptCmd.Index)
                toggleAcceptStatus((sender as DataGridView).Columns[e.ColumnIndex], _row);
        }

        private void saveFmMerchCmd_Click(object sender, EventArgs e)
        {
            // save information to Azure
            openAzureConn();
            DataTable _changes = this.tempDS.FmtcMerchants.GetChanges();
            if (_changes == null)
                return;
            foreach (DataRow _row in _changes.Rows)
            {
                if (_row.Field<int>("updateStatus") == AppUtility.cAcceptedItems)
                {
                    linkFmMerchantToVendor(_row);
                }
                // update the vendor with the acceptance/rejection information
                updateFmMerchantStatusTo(_row, _row.Field<int>("updateStatus"));
            }
            _AzureConn.Close();
            reloadMerchantCmd_Click(sender, e);
        }

        void updateFmMerchantStatusTo(DataRow pMerchantRow, int pNewStatus)
        {
            StatusMessage(String.Format("Updating Merchant Status {0}", pMerchantRow.Field<string>("cName")));
            openAzureConn();
            SqlCommand _cmdSU = new SqlCommand("UPDATE [dbo].[FmtcMerchants] SET updateStatus = @updateStatus, updateUser = @updateUser WHERE nMerchantID = @nMerchantID");
            _cmdSU.Connection = _AzureConn;
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@nMerchantID", System.Data.SqlDbType.NVarChar, 100)));
            _cmdSU.Parameters["@nMerchantID"].Value = pMerchantRow.Field<int>("nMerchantID");
            _cmdSU.Parameters["@updateStatus"].Value = pMerchantRow.Field<int>("updateStatus");
            _cmdSU.Parameters["@updateUser"].Value = pMerchantRow.Field<int>("updateUser");
            _cmdSU.ExecuteNonQuery();
        }

        #endregion Fmtc Merchant manipulation

        #region Fmtc Deals manipulation
        private void updateDealsInfo(DataRow pProductRow)
        {
            toggleAcceptStatus(updateStatusTextFmDeals, pProductRow);
            fmtcDealsGrd.Columns[qponCategoryfmDeals.Index].Selected = true;
            String _cat = stripFmtcCatString(pProductRow.Field<String>("fmtcCategory"));
            if (_cat != "")
            {
                String _url = "";
                int _overwrite = 0;
                String _qponCategory = getQponCategoryfor(_cat, out _overwrite, out _url);
                if (_qponCategory != "")
                {
                    pProductRow.SetField<int>("udfCategoryLuStat", 1);
                    pProductRow.SetField<String>("qponCategory", _qponCategory);
                    if (_url != "")
                        if (_overwrite != 0 || pProductRow.IsNull("cImage") || pProductRow.Field<String>("cImage") == "")
                            pProductRow.SetField<String>("cImage", _url);
                }
            }
            /*
            if (fCategoryEdt.SelectedValue.ToString() != "")
                pProductRow["qponCategory"] = fCategoryEdt.SelectedValue;
             * */
        }

        private void fmtcDealsGrd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                fmtcDealsGrd.Rows[e.RowIndex].Selected = true;
                if ((fmtcDealsBS.Current as DataRowView) == null || (fmtcDealsBS.Current as DataRowView).Row == null)
                    return;
                DataRow _currentRow = (fmtcDealsBS.Current as DataRowView).Row;
                _currentRow["updateUser"] = Convert.ToInt16(pUserAssignEdt.SelectedValue);
                if (e.ColumnIndex == updateStatusText.Index)
                    updateDealsInfo(_currentRow);
            }
            catch (Exception)
            {
                
                
            }
         
        }

        private void saveFmtcDealsCmd_Click(object sender, EventArgs e)
        {
            openAzureConn();
            DataTable _changes = this.tempDS.FmtcDeals.GetChanges();

           

            if (_changes == null)
                return;
            int _productCount = 0;
            foreach (DataRow _row in _changes.Rows)
            {
                if (_row.Field<int>("updateStatus") == 2)
                {
                    if (linkBaseCouponIdToDeals(_row))
                        _productCount++;
                    else
                        return;
                }
                updateDealStatusTo(_row, _row.Field<int>("updateStatus"));
            }
            _AzureConn.Close();
            loadFmtcDealsCmd_Click(sender, e);
            StatusMessage(String.Format("Added {0} Products", _productCount));
        }

        void updateDealStatusTo(DataRow pDealRow, int pNewStatus)
        {
            StatusMessage(String.Format("Updating Product Status {0}", pDealRow.Field<string>("cLabel")));
            openAzureConn();
            SqlCommand _cmdSU = new SqlCommand("UPDATE [dbo].[FmtcDeals] SET updateStatus = @updateStatus, updateUser = @updateUser WHERE nCouponID = @nCouponID");
            _cmdSU.Connection = _AzureConn;
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));
            _cmdSU.Parameters.Add(AppUtility.setNullable(new SqlParameter("@nCouponID", System.Data.SqlDbType.NVarChar, 100)));
            _cmdSU.Parameters["@nCouponID"].Value = pDealRow.Field<int>("nCouponID");
            _cmdSU.Parameters["@updateStatus"].Value = pDealRow.Field<int>("updateStatus");
            _cmdSU.Parameters["@updateUser"].Value = pDealRow.Field<int>("updateUser");
            _cmdSU.ExecuteNonQuery();
        }

        private string getDealsFilterString()
        {
            StringBuilder _filter = new StringBuilder(" "); // cater to "and" in query
            if (filterDealsEdt.Text.Trim() != "")
                _filter.Append(String.Format("AND (fmtcCategory = \'{0}\')", filterDealsEdt.Text.Trim()));
            if (fdDiscOnlyOpt.Checked)
                _filter.Append(String.Format("{0} (ISNULL(fDiscount, 0) <> 0 OR ISNULL(fsalePrice, 0) <> 0)", _filter.Length > 0 ? "AND " : ""));
            if (fLimitMyMerOpt.Checked)
            {
                _filter.Append(String.Format("{0} (EXISTS (SELECT 1 FROM [dbo].FmtcMerchants M WHERE M.nMerchantID = D.nMerchantID AND ISNULL(updateUser, 0) = {1}))", 
                    _filter.Length > 0 ? "AND " : "", fUserAssignEdt.SelectedValue.ToString()));
            }
            //
            return _filter.ToString();
        }

        private void loadFmtcDealsCmd_Click(object sender, EventArgs e)
        {
            SqlCommand _cmd = new SqlCommand();
            // add filters...
            StatusMessage("Loading Fmtc Deals data");
            _cmd.CommandText = String.Format("SELECT * FROM [dbo].[FmtcDeals] D WHERE " +
                (fdAccOpt.Checked ? " (baseCouponId is NULL) AND " : "") +
                    " [updateStatus] in (@newItems, @accepted, @rejected, @later) {0}", getDealsFilterString());
            _cmd.Connection = _AzureConn;
            setStatusCheckFilter(_cmd, fdNewOpt.Checked, fdAccOpt.Checked, fdRejOpt.Checked, fdLaterOpt.Checked);
            SqlDataAdapter _sqlDa = new SqlDataAdapter(_cmd);
            openAzureConn();
            this.tempDS.FmtcDeals.Clear();
            _sqlDa.Fill(this.tempDS.FmtcDeals);

          
            StatusMessage("Fmtc Deals Data Loaded");
        }

        #endregion Fmtc Deals manipulation


        #region Utility methods

        private void setStatusCheckFilter(SqlCommand pCmd, bool pNewOpt, bool pAcceptedOpt, bool pRejectedOpt, bool pLaterOpt)
        {
            pCmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@newItems", 0)));
            pCmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@accepted", 0)));
            pCmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@rejected", 0)));
            pCmd.Parameters.Add(AppUtility.setNullable(new SqlParameter("@later", 0)));
            if (pNewOpt)
                pCmd.Parameters["@newItems"].Value = AppUtility.cNewItems;
            if (pAcceptedOpt)
                pCmd.Parameters["@accepted"].Value = AppUtility.cAcceptedItems;
            if (pRejectedOpt)
                pCmd.Parameters["@rejected"].Value = AppUtility.cRejectedItems;
            if (pLaterOpt)
                pCmd.Parameters["@later"].Value = AppUtility.cLaterItems;
        }

        private void toggleAcceptStatus(DataGridViewColumn pCol, DataRow pRow)
        {

            if (pCol is DataGridViewButtonColumn)
            {
                int _updateStatus = Convert.ToInt32(pRow["UpdateStatus"]);
                _updateStatus++;
                if (_updateStatus > 4)
                    _updateStatus = 2;
                pRow["UpdateStatus"] = _updateStatus;
            }
        }
        #endregion Utility methods

        private void ProsperentEdtF_Activated(object sender, EventArgs e)
        {
            if (!_loginCompleted)
            {
                LoginF _loginf = new LoginF();
                _loginf.UserList = tempDS.UserList;
                if (_loginf.ShowDialog() == DialogResult.OK)
                {
                    pUserAssignEdt.SelectedValue = _loginf.loginIdEdt.SelectedValue;
                    fUserAssignEdt.SelectedValue = _loginf.loginIdEdt.SelectedValue;
                    if (Convert.ToInt32(_loginf.loginIdEdt.SelectedValue) != 0)
                    {
                        pUserAssignEdt.Enabled = false;
                        fUserAssignEdt.Enabled = false;
                    }
                    _loginCompleted = true;
                }
            }
        }

        private void reloadCategoryCmd_Click(object sender, EventArgs e)
        {
            // 
            SqlCommand _cmd = null;
            SqlDataAdapter _sqlDa = null;
            // category list from Swagger API will be stored in a a generic table for <int, string> keyPair values
            _cmd = new SqlCommand("SELECT * FROM [dbo].[Categorymap]", _AzureConn);
            _sqlDa = new SqlDataAdapter(_cmd);
            _sqlDa.Fill(this.tempDS.CategoryMap); // create a temporary table to hold values from the swagger repository
        }

        private String stripProspCatString(String pCat)
        {
            string[] _cats = pCat.Split('>');
            StringBuilder _result = new StringBuilder();
            if (_cats.Length == 0)
                return "";
            _result.Append(_cats[0].Trim());
            if (_cats.Length > 1)
                _result.Append(";").Append(_cats[1].Trim());
            if (_cats.Length > 2)
                _result.Append(";").Append(_cats[2].Trim());
            return _result.ToString();
        }

        private String stripFmtcCatString(String pCat)
        {
            string[] _cats = pCat.Split(';');
            StringBuilder _result = new StringBuilder();
            if (_cats.Length == 0)
                return "";
            _result.Append(_cats[0]);
            if (_cats.Length > 1)
                _result.Append(";").Append(_cats[1]);
            if (_cats.Length > 2)
                _result.Append(";").Append(_cats[2]);
            return _result.ToString();
        }

        private String getQponCategoryfor(String pKeyWord, out int pOverwriteImage, out String pImageUrl)
        {
            SqlCommand _cmd = new SqlCommand("SELECT qponCategory, imageUrl, overwriteImageUrl FROM dbo.CategoryMap WHERE UPPER(keyWord) LIKE '%' + UPPER(@pKeyWord) + '%'");
            _cmd.Parameters.Add("@pKeyWord", SqlDbType.NVarChar, 60);
            _cmd.Parameters["@pKeyWord"].Value = pKeyWord;
            _cmd.Connection = _AzureConn;
            pImageUrl = "";
            pOverwriteImage = 0;
            using (SqlDataReader _reader = _cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (!_reader.Read())
                    return "";
                pImageUrl = _reader.IsDBNull(1) ? "" : _reader.GetFieldValue<String>(1);
                pOverwriteImage = _reader.IsDBNull(2) ? 0 : _reader.GetFieldValue<int>(2);
                return _reader.IsDBNull(0) ? "" : _reader.GetFieldValue<String>(0);
            }
        }

        private void SaveQponCategory(String pKeyWord, String pCategory, bool pUpdate)
        {
            SqlCommand _cmd = null;
            if (!pUpdate)
            {
                _cmd = new SqlCommand("INSERT INTO dbo.CategoryMap(qponCategory, keyword) VALUES (@qponCategory, @KeyWord)");
            }
            else
            {
                _cmd = new SqlCommand("UPDATE dbo.CategoryMap SET qponCategory = @qponCategory WHERE , keyword = @KeyWord");
            }
            _cmd.Parameters.Add("@KeyWord", SqlDbType.NVarChar, 60);
            _cmd.Parameters.Add("@qponcategory", SqlDbType.NVarChar, 60);
            _cmd.Parameters["@KeyWord"].Value = pKeyWord;
            _cmd.Parameters["@qponcategory"].Value = pCategory;
            _cmd.Connection = _AzureConn;
            try
            {
                _cmd.ExecuteNonQuery();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message); // ignore error for duplicates...
            }
        }

        private void saveCategoryCmd_Click(object sender, EventArgs e)
        {
            openAzureConn();
            DataTable _changes = this.tempDS.CategoryMap.GetChanges();
            if (_changes == null)
                return;
            int _count = 0;
            foreach (DataRow _row in _changes.Rows)
            {
                SqlCommand _cmd = null;
                if (_row.RowState == DataRowState.Added)
                {
                    _cmd = new SqlCommand("INSERT INTO dbo.CategoryMap(qponCategory, keyword, imageUrl, overwriteImageUrl) VALUES (@qponCategory, @keyword, @imageUrl, @overwriteImageUrl)");
                    _cmd.Parameters.Add("@keyword", SqlDbType.NVarChar, 250);
                    _cmd.Parameters.Add("@qponCategory", SqlDbType.NVarChar, 60);
                    _cmd.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 250);
                    _cmd.Parameters.Add("@overwriteImageUrl", SqlDbType.Int);
                    _cmd.Parameters["@keyword"].Value = _row.Field<String>("keyword");
                    _cmd.Parameters["@qponCategory"].Value = _row.Field<String>("qponCategory");
                    _cmd.Parameters["@imageUrl"].Value = _row.Field<String>("imageUrl");
                    _cmd.Parameters["@overwriteImageUrl"].Value = _row.Field<int>("overwriteImageUrl");
                }
                if (_row.RowState == DataRowState.Modified)
                {
                    if (_row.HasVersion(DataRowVersion.Original))
                    {
                        _cmd = new SqlCommand("UPDATE dbo.CategoryMap SET qponCategory = @qponCategory, " +
                            "imageUrl = @imageUrl, overwriteImageUrl = @overwriteImageUrl, keyword = @keyword WHERE qponCategory = @qponCategoryOrg AND keyword = @keywordOrg");
                        //_cmd = new SqlCommand("DELETE FROM dbo.CategoryMap WHERE qponCategory = @qponCategory AND keyword = @keyword");
                        _cmd.Parameters.Add("@keyword", SqlDbType.NVarChar, 60);
                        _cmd.Parameters.Add("@qponCategory", SqlDbType.NVarChar, 60);
                        _cmd.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 250);
                        _cmd.Parameters.Add("@overwriteImageUrl", SqlDbType.Int);
                        _cmd.Parameters.Add("@keywordOrg", SqlDbType.NVarChar, 60);
                        _cmd.Parameters.Add("@qponCategoryOrg", SqlDbType.NVarChar, 60);
                        _cmd.Parameters["@keyword"].Value = _row.Field<String>("keyword");
                        _cmd.Parameters["@qponCategory"].Value = _row.Field<String>("qponCategory");
                        _cmd.Parameters["@imageUrl"].Value = _row.Field<String>("imageUrl");
                        _cmd.Parameters["@overwriteImageUrl"].Value = _row.Field<int>("overwriteImageUrl");
                        _cmd.Parameters["@keywordOrg"].Value = _row.Field<String>("keyword", DataRowVersion.Original);
                        _cmd.Parameters["@qponCategoryOrg"].Value = _row.Field<String>("qponCategory", DataRowVersion.Original);
                    }
                }
                if (_row.RowState == DataRowState.Deleted)
                {
                    if (_row.HasVersion(DataRowVersion.Original))
                    {
                        _cmd = new SqlCommand("DELETE FROM dbo.CategoryMap WHERE qponCategory = @qponCategoryOrg AND keyword = @keywordOrg");
                        //_cmd = new SqlCommand("DELETE FROM dbo.CategoryMap WHERE qponCategory = @qponCategory AND keyword = @keyword");
                        _cmd.Parameters.Add("@keywordOrg", SqlDbType.NVarChar, 60);
                        _cmd.Parameters.Add("@qponCategoryOrg", SqlDbType.NVarChar, 60);
                        _cmd.Parameters["@keywordOrg"].Value = _row.Field<String>("keyword", DataRowVersion.Original);
                        _cmd.Parameters["@qponCategoryOrg"].Value = _row.Field<String>("qponCategory", DataRowVersion.Original);
                    }
                }
                if (_cmd != null)
                {
                    _cmd.Connection = _AzureConn;
                    _cmd.ExecuteNonQuery();
                }
            }
            _AzureConn.Close();
            reloadCategoryCmd_Click(sender, e);
            StatusMessage(String.Format("Added/modified {0} Categories", _count));
        }

        private void fmtcDealsBS_CurrentItemChanged(object sender, EventArgs e)
        {
        }

        private void fmtcDealsGrd_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {

        }

        private void fmtcDealsGrd_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == qponCategoryfmDeals.Index)
            {
                if ((fmtcDealsBS.Current as DataRowView) == null || (fmtcDealsBS.Current as DataRowView).Row == null)
                    return;
                DataRow _row = (fmtcDealsBS.Current as DataRowView).Row;
                if (_row.Field<int>("udfCategoryLuStat") == 0 || _row.Field<int>("udfCategoryLuStat") == 1)
                {
                    String _cat = stripFmtcCatString(_row.Field<String>("fmtcCategory"));
                    String _qponCategory = _row.Field<String>("qponcategory");
                    bool _doUpdate = _row.Field<int>("udfCategoryLuStat") == 1;
                    if (_cat != "" && _qponCategory != "")
                    {
                     /*   if (MessageBox.Show(String.Format("Associate \"{0}\" with category {1}", _cat, _qponCategory), "New category", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveQponCategory(_cat, _qponCategory, _doUpdate);
                            _row.SetField<int>("udfCategoryLuStat", 2);
                        } */
                    }
                }
            }
            if (e.ColumnIndex == updateStatusTextFmDeals.Index)
            {
                if ((fmtcDealsBS.Current as DataRowView) == null || (fmtcDealsBS.Current as DataRowView).Row == null)
                    return;
                DataRow _row = (fmtcDealsBS.Current as DataRowView).Row;
                     String _cat = stripFmtcCatString(_row.Field<String>("cCategory"));
                     if (_cat != "")
                     {
                         String _url = "";
                         int _overwrite = 0;
                         String _qponCategory = getQponCategoryfor(_cat, out _overwrite, out _url);
                         if (_qponCategory != "")
                         {
                             _row.SetField<int>("udfCategoryLuStat", 1);
                             _row.SetField<String>("qponCategory", _qponCategory);
                             if (_url != "")
                                 if (_overwrite != 0 || _row.IsNull("cImage") || _row.Field<String>("cImage") == "")
                                     _row.SetField<String>("cImage", _url);
                         }
                     } 
            } 
        }

        private void fmtcDealsGrd_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
        }

        private void productGrd_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == qponCategoryProsp.Index)
            {
                if ((prosperentProductBS.Current as DataRowView) == null || (prosperentProductBS.Current as DataRowView).Row == null)
                    return;
                DataRow _row = (prosperentProductBS.Current as DataRowView).Row;
                if (_row.Field<int>("udfCategoryLuStat") == 0 || _row.Field<int>("udfCategoryLuStat") == 1)
                {
                    String _cat = stripProspCatString(_row.Field<String>("category"));
                    String _qponCategory = _row.Field<String>("qponcategory");
                    bool _doUpdate = _row.Field<int>("udfCategoryLuStat") == 1;
                    if (_cat != "" && _qponCategory != "")
                    {
                    /*    if (MessageBox.Show(String.Format("Associate \"{0}\" with category {1}", _cat, _qponCategory), "New category", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveQponCategory(_cat, _qponCategory, _doUpdate);
                            _row.SetField<int>("udfCategoryLuStat", 2);
                        } */
                    }
                }
            }
            if (e.ColumnIndex == updateStatusText.Index)
            {
                if ((prosperentProductBS.Current as DataRowView) == null || (prosperentProductBS.Current as DataRowView).Row == null)
                    return;
                DataRow _row = (prosperentProductBS.Current as DataRowView).Row;
                String _cat = stripProspCatString(_row.Field<String>("category"));
                if (_cat != "")
                {
                    String _url = "";
                    int _overwrite = 0;
                    String _qponCategory = getQponCategoryfor(_cat, out _overwrite, out _url);
                    if (_qponCategory != "")
                    {
                        _row.SetField<int>("udfCategoryLuStat", 1);
                        _row.SetField<String>("qponCategory", _qponCategory);
                        if (_url != "")
                            if (_overwrite != 0 || ( _row.IsNull("image_url") || _row.Field<String>("image_url") == ""))
                                _row.SetField<String>("image_url", _url);
                    }
                }
            }
        }

        private void fmtcDealsGrd_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void filterDealsEdt_DoubleClick(object sender, EventArgs e)
        {
        }

        private void filterDealsCmd_Click(object sender, EventArgs e)
        {
            if (filterDealsEdt.Text.Trim() != "") {
                filterDealsEdt.Text = "";
                loadFmtcDealsCmd_Click(loadFmtcDealsCmd, null);
            }
            else {
                if ((fmtcDealsBS.Current as DataRowView) == null || (fmtcDealsBS.Current as DataRowView).Row == null)
                    return;
                DataRow _currentRow = (fmtcDealsBS.Current as DataRowView).Row;
                filterDealsEdt.Text = _currentRow.Field<String>("fmtcCategory");
                loadFmtcDealsCmd_Click(loadFmtcDealsCmd, null);
            }
        }

        private void filterProsCmd_Click(object sender, EventArgs e)
        {
            if (filterDataEdt.Text.Trim() != "")
            {
                filterDataEdt.Text = "";
                reloadProductCmd_Click(reloadProductCmd, null);
            }
            else
            {
                if ((prosperentProductBS.Current as DataRowView) == null || (prosperentProductBS.Current as DataRowView).Row == null)
                    return;
                DataRow _currentRow = (prosperentProductBS.Current as DataRowView).Row;
                filterDataEdt.Text = _currentRow.Field<String>("merchant");
                reloadProductCmd_Click(reloadProductCmd, null);
            }
        }


        public void ExpireBaseQpon(string QponID)
        {

            String _qponConnStr = @"Server=tcp:njk8bedrfs.database.windows.net,1433;Database=QponQrushb22;User ID=qpon@njk8bedrfs;Password=crushin01$;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";


            SqlConnection _QponConn = new SqlConnection(_qponConnStr);

            System.Data.DataTable storage = new System.Data.DataTable();

            SqlCommand _cmd = new SqlCommand();

          
            _cmd.CommandText =
                "UPDATE BaseCoupon SET ExpiryDate = '1950-01-01' WHERE BaseCouponID = '" + QponID+"'";
            
            _cmd.Connection = _QponConn;

            _QponConn.Open();

            _cmd.ExecuteNonQuery();

            _QponConn.Close();

            

        }

        public void SetRetailSale(string QponID, string retail, string sale)
        {

        //    String _qponConnStr = @"Server=tcp:njk8bedrfs.database.windows.net,1433;Database=QponQrushb22;User ID=qpon@njk8bedrfs;Password=crushin01$;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

            String _qponConnStr =
                @"Server=tcp:njk8bedrfs.database.windows.net,1433;Database=QponCrush;User ID=qpon@njk8bedrfs;Password=crushin01$;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

            SqlConnection _QponConn = new SqlConnection(_qponConnStr);

            System.Data.DataTable storage = new System.Data.DataTable();

            SqlCommand _cmd = new SqlCommand();


            if (sale != "" && (retail != ""))
            {
                _cmd.CommandText =
                    "UPDATE BaseCoupon SET salePrice =" + sale + ", retailPrice =" + retail +
                    " WHERE BaseCouponID = '" + QponID + "'";

                _cmd.Connection = _QponConn;

                _QponConn.Open();

                _cmd.ExecuteNonQuery();

                _QponConn.Close();
            }



        }


        private void button1_Click(object sender, EventArgs e)
        {

            ExpireBaseQpon(couponView[0, couponView.CurrentCell.RowIndex].Value.ToString());
            MessageBox.Show("Qpon "+couponView[0, couponView.CurrentCell.RowIndex].Value.ToString() +" Expired");


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            for (var c = 0; c < couponView.Rows.Count; c++)
            {
                
                CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[couponView.DataSource];
                currencyManager1.SuspendBinding();

                if (couponView[4, c].Value != null)
                {
                    string mySqlTime = couponView[4, c].Value.ToString();
                    DateTime qdate = DateTime.Parse(mySqlTime);


                    if (qdate < DateTime.Now)
                        couponView.Rows[c].Visible = false;
                    else
                    {
                        couponView.Rows[c].Visible = true;
                    }
                }
                currencyManager1.ResumeBinding();

            }
        }

        private void showAllBtn_Click(object sender, EventArgs e)
        {

            for (var c = 0; c < couponView.Rows.Count; c++)
            {
                couponView.Rows[c].Visible = true;
            }

        }

        private void RemoveDuplicates_Click(object sender, EventArgs e)
        {

            int ec = 0;
            // for (var c = 0; c < couponView.Rows.Count; c++)
            for (var c = 0; c < couponView.Rows.Count - 2; c++)
            {
                var description = couponView[3, c].Value.ToString();

                for (var c1 = c + 1; c1 < couponView.Rows.Count -1; c1++)
                {

                    if (description == couponView[3, c1].Value.ToString())
                    {
                        string mySqlTime = couponView[4, c1].Value.ToString();
                        DateTime qdate = DateTime.Parse(mySqlTime);

                        if (qdate > DateTime.Now)
                        {
                            ExpireBaseQpon(couponView[0, c1].Value.ToString());
                            ec++;
                            // MessageBox.Show(couponView[3, c1].Value.ToString() + " duplicate removed.");
                        }
                    }

                }

            }
            
            couponView.Refresh();
            MessageBox.Show("Done removing "+ec+" duplicates");
        }

        private void Hide_Click(object sender, EventArgs e)
        {

            fmtcDealsGrd.Visible = false;

            CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[fmtcDealsGrd.DataSource];
            currencyManager1.SuspendBinding();

            for (int c = 0; c < fmtcDealsGrd.Rows.Count - 1; c++)
            {


                if ("Amazon Local".Equals(fmtcDealsGrd[3, c].Value))
                    fmtcDealsGrd.Rows[c].Visible = false;

                if ("Amazon Local UK".Equals(fmtcDealsGrd[3, c].Value))
                    fmtcDealsGrd.Rows[c].Visible = false;

                if ("Groupon".Equals(fmtcDealsGrd[3, c].Value))
                    fmtcDealsGrd.Rows[c].Visible = false;
            }

            currencyManager1.ResumeBinding();

            fmtcDealsGrd.Visible = true;

        }

        private void HideGroupOnBtn_Click(object sender, EventArgs e)
        {

            fmtcDealsGrd.Visible = false;

            CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[fmtcDealsGrd.DataSource];
            currencyManager1.SuspendBinding();

            for (int c = 0; c < fmtcDealsGrd.Rows.Count - 1; c++)
            {

                String merchant = fmtcDealsGrd[3, c].Value.ToString();
 
                if (merchant.IndexOf("Groupon") > -1)
                    fmtcDealsGrd.Rows[c].Visible = false;
            }

            currencyManager1.ResumeBinding();

            fmtcDealsGrd.Visible = true;

        }

        private void ShowWithImage_Click(object sender, EventArgs e)
        {
            fmtcDealsGrd.Visible = false;

            CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[fmtcDealsGrd.DataSource];
            currencyManager1.SuspendBinding();

            for (int c = 0; c < fmtcDealsGrd.Rows.Count-1; c++)
            {

                try
                {
                    String image = fmtcDealsGrd[13, c].Value.ToString();

                    if (image == "")
                        fmtcDealsGrd.Rows[c].Visible = false;
                }
                catch (Exception)
                {
                    fmtcDealsGrd.Rows[c].Visible = false;   
 
                }
               
            }

            currencyManager1.ResumeBinding();

            fmtcDealsGrd.Visible = true;
        }

        private int ReturnColumn(string s)
        {
            int c = -1;

            for (c = 0; c < fmtcDealsGrd.Columns.Count; c++)
            {

                if (s == fmtcDealsGrd.Columns[c].HeaderText)
                    return c;
            }

            return c;
        }

        private string ConvertCategory(string merchant, string fmtc)
        {
            string org = fmtc;
            fmtc = fmtc.ToLower();


            if (fmtc.IndexOf("accesories") > -1)
                return "Accesories";

            if (fmtc.IndexOf("gift") > -1)
                return "Accesories";

            if (fmtc.IndexOf("shoe") > -1)
            {

                if (fmtc.IndexOf("women") > -1)
                    return "Women's Shoes";

                if (fmtc.IndexOf("men") > -1)
                    return "Men's Apparel";

                return "Women's Shoes";
            }



            if (fmtc.IndexOf("automotive") > -1)
                return "Automotive";

            if (fmtc.IndexOf("fragrance") > -1)
                return "Fragrance";

            if (fmtc.IndexOf("babies") > -1)
                return "Baby & Kid’s Apparel";

         

            if (fmtc.IndexOf("jewelry") > -1)
                return "Jewelry";

            if (fmtc.IndexOf("health") > -1)
                return "Health & Beauty";

            if (fmtc.IndexOf("beauty") > -1)
                return "Health & Beauty";

            if (fmtc.IndexOf("food") > -1)
                return "Restaurants";

            if (fmtc.IndexOf("book") > -1)
                return "Household";

            if (fmtc.IndexOf("kitchen") > -1)
                return "Household";

            if (fmtc.IndexOf("pets") > -1)
                return "Pets";

            if (fmtc.IndexOf("sports") > -1)
                return "Sports & Recreation";

            if (fmtc.IndexOf("sporting") > -1)
                return "Sports & Recreation";

            if (fmtc.IndexOf("outdoor") > -1)
                return "Sports & Recreation";

            if (fmtc.IndexOf("toy") > -1)
                return "Toys";

            if (fmtc.IndexOf("home") > -1)
                return "Household";

            


            if (fmtc.IndexOf("office") > -1)
                return "Household";

            if (fmtc.IndexOf("book") > -1)
                return "Household";


            if (fmtc.IndexOf("electronics") > -1)
                return "Electronics";

            if (fmtc.IndexOf("computer") > -1)
                return "Electronics";

            if (fmtc.IndexOf("camera") > -1)
                return "Electronics";

            if (fmtc.IndexOf("bath") > -1)
                return "Household";

          
            if (fmtc.IndexOf("luggage") > -1)
                return "Travel";

            if (fmtc.IndexOf("intimate") > -1)
                return "Women's Apparel";

            if (fmtc.IndexOf("wedding") > -1)
                return "Women's Apparel";

            if (fmtc.IndexOf("women") > -1)
                return "Women's Apparel";

            if (fmtc.IndexOf("entertainment") > -1)
                return "Travel";

            
            if (org.IndexOf("Men") > -1)
                return "Men's Apparel";

            if (fmtc.IndexOf("travel") > -1)
                return "Travel";

            if (fmtc.IndexOf("phone") > -1)
                return "Electronics";

            if (fmtc.IndexOf("baby") > -1)
                return "Baby & Kid’s Apparel";

            if (fmtc.IndexOf("apparel") > -1)
                return "Men's Apparel";

            if (fmtc.IndexOf("bed") > -1)
                return "Household";



            return null;

        }

        private string MerchantImage(string s, string id)
        {
            
            if (s.ToLower() == "auto parts warehouse")
                return "http://usb2web.com/autoparts.jpg";

            if (s.ToLower() == "best buy")
                return "http://usb2web.com/bestbuy.jpg";

            if (s.ToLower() == "the college shack")
                return "http://usb2web.com/collegeshack.jpg";

            if (s.ToLower() == "the walking company")
                return "http://usb2web.com/walkingcompany.jpg";

            if (s.ToLower() == "jollychic.com")
                return "http://usb2web.com/jollychic.jpg";

            if (s.ToLower() == "jewelbasket.com")
                return "http://usb2web.com/jewelbasket.jpg";

            if (s.ToLower() == "izidress.com")
                return "http://usb2web.com/Izidress.jpg";

            if (s.ToLower() == "izidress.com")
                return "http://usb2web.com/Izidress.jpg";

            if (s.ToLower() == "1-800-flowers")
                return "http://usb2web.com/1800flowers.jpg";

            if (s.ToLower() == "1-800-flowers.ca")
                return "http://usb2web.com/1800flowers.jpg";

            if (s.ToLower() == "1800anylens")
                return "http://usb2web.com/1800AnyLens.jpg";


            if (s.ToLower() == "ace hardware")
                return "http://usb2web.com/acehardware.jpg";

            
                return "http://usb2web.com/logos/"+id+".gif";

 
        }
        private void AutoFillBtn_Click(object sender, EventArgs e)
        {
         //   for (int c = 0; c < fmtcDealsGrd.Rows.Count - 1; c++)

            pBar.Maximum = fmtcDealsGrd.Rows.Count;

            pBar.Value = 0;

            //this.updateStatusText.ReadOnly = false;
            ARows.Clear();
            int col = ReturnColumn("qponCategory");

            if (col > -1)
            {

                fmtcDealsGrd.Columns[col].ReadOnly = false;

                for (int c = 0; c <fmtcDealsGrd.Rows.Count-1; c++)
                //for (int c = 0; c < 2; c++)
                {
                    pBar.Value = c;

                    var cat = ConvertCategory(fmtcDealsGrd[3, c].Value.ToString(),
                        fmtcDealsGrd[2, c].Value.ToString());

                    fmtcDealsGrd[col, c].Value = cat;

                    if (fmtcDealsGrd[4, c].Value.ToString().Length < 25)
                    {

                       // fmtcDealsGrd[4, c].Value = fmtcDealsGrd[3, c].Value.ToString() + " - " + fmtcDealsGrd[4, c].Value.ToString();
                    }

                  
                    // Toggle Accept

                    DataGridViewCellEventArgs e1 = new DataGridViewCellEventArgs(0, c);

                  //  fmtcDealsGrd_CellContentClick(sender, e1);
                    if (c == 0)
                    {
                        fmtcDealsGrd.CurrentCell = fmtcDealsGrd.Rows[1].Cells[2];
                    }
                    else
                    {
                        fmtcDealsGrd.CurrentCell = fmtcDealsGrd.Rows[0].Cells[2];    

                    }

                    if (cat != "")
                    {
                        var image = ReturnColumn("cImage");

                        if (fmtcDealsGrd[image, c].Value.ToString() == "")
                        {

                            int mid = ReturnColumn("nMerchantID");

                            string merchid = fmtcDealsGrd[mid, c].Value.ToString();

                            fmtcDealsGrd[image, c].Value = MerchantImage(fmtcDealsGrd[3, c].Value.ToString(), merchid);

                         

                            fmtcDealsGrd.ClearSelection();

                           
                        }
                         // ------------------------------------------------------------------------- Accept Block
                           if (fmtcDealsGrd[image, c].Value.ToString() != "")
                            {
                               

                                var exp = ReturnColumn("dtEndDate");
                                var expdate = fmtcDealsGrd[exp, c].Value;

                                bool expired = DateTime.Now > Convert.ToDateTime(expdate);

                                if (fmtcDealsGrd[image, c].Value.ToString() != ""
                                    && (fmtcDealsGrd[3, c].Value.ToString().ToLower().IndexOf("local") == -1)
                                    //  && (fmtcDealsGrd[1, c].Value.ToString().IndexOf("Men'") > -1)
                                    && (fmtcDealsGrd[2, c].Value.ToString().ToLower().IndexOf("local") == -1)
                                    && (!expired)
                                    && (cat != "")
                                    && (cat != null))
                                {
                                    DataRow dr = tempDS.FmtcDeals.Rows[c];
                                    dr["updateStatus"] = AppUtility.cAcceptedItems;

                                    dr.AcceptChanges();

                                    ARows.Add(dr);
                                }   
 
                              

                            }
                          // --------------------------------------------------------------------------------------

                      
                      
                    }
                }

                fmtcDealsGrd.Refresh();
               
            }

            this.updateStatusText.ReadOnly = true;
        }

        private void SaveAutoBtn_Click(object sender, EventArgs e)
        {
            openAzureConn();


            pBar.Maximum = ARows.Count;

            pBar.Value = 0;
         
            int _productCount = 0;
            int c = 0;
            foreach (DataRow _row in ARows)
            {
                if (_row.Field<int>("updateStatus") == 2)
                {
                    if (linkBaseCouponIdToDeals(_row))
                        _productCount++;
                    else
                        return;
                }
                updateDealStatusTo(_row, _row.Field<int>("updateStatus"));

                c++;
                pBar.Value = c;
            }
            _AzureConn.Close();
            loadFmtcDealsCmd_Click(sender, e);
            StatusMessage(String.Format("Added {0} Products", _productCount));
        }

        
        private void SetPricesBtn_Click(object sender, EventArgs e)
        {

            int ec = 0;
            // for (var c = 0; c < couponView.Rows.Count; c++)
            for (var c = 0; c < couponView.Rows.Count - 2; c++)
            {
                var description = couponView[3, c].Value.ToString();

                int w = description.IndexOf("Was:");
                int n = description.IndexOf("Now:");

              
               

                if ((w > 0)  && (n > 0))
                {

                    string was = description.Substring(w, (n - w));

                    Regex regex = new Regex(@"^-?\d+(?:\.\d+)?");
                    Match match = regex.Match(was);


                    was = Regex.Replace(was, "[^.0-9]", "");

                    if (was != "")
                    {
                        if (was.Length > 0)
                        {
                            if (was.Substring(was.Length - 1, 1) == ".")
                            {
                                was = was.Substring(0, was.Length - 1);

                            }
                        }
                        else
                        {
                            was = "";
                        }
                    }


                    string now = description.Substring(n, description.Length - n);

                    if (now != "")
                    {
                        now = Regex.Replace(now, "[^.0-9]", "");

                        if (now.Length > 0)
                        {
                            if (now.Substring(now.Length - 1, 1) == ".")
                            {
                                now = now.Substring(0, now.Length - 1);

                            }
                        }
                        else
                        {
                            now = "";
                        }
                    }

                    ec++;

                    SetRetailSale(couponView[0, c].Value.ToString(), was, now);

                }
                
                        
                       
            }

            couponView.Refresh();
            MessageBox.Show("Done updating " + ec + " prices");
        }

    }


}
