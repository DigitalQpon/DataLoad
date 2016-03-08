using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Threading;

/*
 * http://qponcrush.azurewebsites.net/api/User/addBaseCoupon?api_key=CE2C6279-D782-4FA8-B474-51A4E166BA65
 * 
 */

namespace QponCollector
{
    public class WebRequestState
    {
        public Func<JObject, bool> RespCallback;
        public HttpWebRequest Request;
    }

    public class CouponFeedProcess
    {
        private String _cBaseAzureCall = @"http://qponcrush.azurewebsites.net:80/api/User/";
        private String _cSwaggerAppKey = @"CE2C6279-D782-4FA8-B474-51A4E166BA65";
        private String _cAddbaseCoupon = @"addbaseCoupon";
        private String _cRegisterAdmin = "registerAdmin";
        private String _cLoginAdmin = "loginAdmin";
        bool _fmtcDealsSemaphore = false;
        bool _fmtcCancelRequested = false;

        enum UpdateAction { uaInsert = 1, uaModify = 2, auDelete = 3 }
        const int _cTimeOutForDownload = 8; // todo:magic number for fmtc download

        public String DbConnectionString { get; set; }
        public ManualResetEvent EventProcNextMerchant = null;
        public String FmtcDownloadOpMessage;

        public bool FmtcDealsSemaphore { get { return _fmtcDealsSemaphore; } }
        public void RequestFmtcDownloadCancel()
        {
            if (!_fmtcDealsSemaphore)
                return;
            _fmtcCancelRequested = true;
        }
        public void RaiseFmtcDealsSemaphore()
        {
            _fmtcDealsSemaphore = true;
            _fmtcCancelRequested = false;
        }
        public void ResetFmtcDealsSemaphore()
        {
            _fmtcDealsSemaphore = false;
            _fmtcCancelRequested = false;
        }

        private Uri setUrlPageTo(Uri pUrl, int pPage)
        {
            string _urlAsString = pUrl.AbsoluteUri;
            string[] _baseUrl = _urlAsString.Split('?');
            // assuming has the separator...
            NameValueCollection _queryValues = HttpUtility.ParseQueryString(_baseUrl[1]);
            if (_queryValues.Get("page") == null)
                _queryValues.Add("page", Convert.ToString(pPage));
            else
                _queryValues["page"] = Convert.ToString(pPage);
            return new Uri(String.Format("{0}?{1}", _baseUrl[0], _queryValues.ToString()));
        }

        public bool doAsyncDownload(String pJsonData, String pApiTag, Func<JObject, bool> pRespCallback)
        {
            Uri _azureCall = new Uri(String.Format("{0}/{1}", _cBaseAzureCall, pApiTag));
            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(_azureCall);
            _request.Method = "POST";
            _request.ContentType = @"application/json; charset=utf-8";
            _request.Headers.Add("AppKey", _cSwaggerAppKey);
            //
            WebRequestState _reqS = new WebRequestState();
            _reqS.RespCallback = pRespCallback;
            _reqS.Request = _request;

            using (StreamWriter _webStream = new StreamWriter(_request.GetRequestStream()))
            {
                _webStream.Write(pJsonData);
            }
            IAsyncResult _asyn = (IAsyncResult) _request.BeginGetResponse(new AsyncCallback((IAsyncResult ar) => {
                WebRequestState _aReqS = (WebRequestState)ar.AsyncState;
                HttpWebRequest _aRequest = _reqS.Request;
                HttpWebResponse _webResponse = (HttpWebResponse)_aRequest.EndGetResponse(ar);
                using (StreamReader _webReader = new StreamReader(_webResponse.GetResponseStream()))
                {
                    JObject _response = JsonConvert.DeserializeObject<JObject>(_webReader.ReadToEnd());
                    //Console.WriteLine(_response);
                    if (!_aReqS.RespCallback(_response))
                        return;
                }
            }), _reqS);
            return true;
        }

        public bool doSendDataWithWait(String pJsonData, String pApiTag, Func<JObject, bool> pRespCallback)
        {
            Uri _azureCall = new Uri(String.Format("{0}/{1}", _cBaseAzureCall, pApiTag));
            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(_azureCall);
            _request.Method = "POST";
            _request.ContentType = @"application/json; charset=utf-8";
            _request.Headers.Add("AppKey", _cSwaggerAppKey);
            _request.Timeout = 1000 * _cTimeOutForDownload; //
            using (StreamWriter _webStream = new StreamWriter(_request.GetRequestStream()))
            {
                _webStream.Write(pJsonData);
            }

            HttpWebResponse _webResponse = (HttpWebResponse)_request.GetResponse();
            using (StreamReader _webReader = new StreamReader(_webResponse.GetResponseStream()))
            {
                JObject _response = JsonConvert.DeserializeObject<JObject>(_webReader.ReadToEnd());
                Console.WriteLine(_response);
                if (!pRespCallback(_response))
                    return false;
            }
            return true;
        }

        public class JsonDataDownloadType
        {
            public enum EnumDownloadType { isUrl, isFileName, isString };
            public EnumDownloadType DownloadType;
            Uri _fromUri;
            String _fromString;
            String _fromFileName;
            public Uri fromUri { get { return _fromUri; } set { _fromUri = value; DownloadType = EnumDownloadType.isUrl; } }
            public String fromString { get { return _fromString; } set { _fromString = value; DownloadType = EnumDownloadType.isString; } }
            public String fromFileName { get { return _fromFileName; } set { _fromFileName = value; DownloadType = EnumDownloadType.isFileName; } } 
        }

        public bool DownloadJsonDataArray(
            bool pDoAsyncDownload,
            JsonDataDownloadType pFromUrl,
            Func<JArray, bool> pPostDownloadDelegate,
            Func<JToken, bool> pProcessDataRecord
            )
        {
            FmtcDownloadOpMessage = "";
            Uri _FromUrl = pFromUrl.DownloadType == JsonDataDownloadType.EnumDownloadType.isUrl ? pFromUrl.fromUri :null;
            String _fileName = pFromUrl.DownloadType == JsonDataDownloadType.EnumDownloadType.isFileName ? pFromUrl.fromFileName : "";
            string _jsonData = pFromUrl.DownloadType == JsonDataDownloadType.EnumDownloadType.isString ? pFromUrl.fromString : "";
            if (pFromUrl.DownloadType == JsonDataDownloadType.EnumDownloadType.isFileName)
            {
                _jsonData = File.ReadAllText(_fileName);
            }
            else
                if (pFromUrl.DownloadType == JsonDataDownloadType.EnumDownloadType.isUrl)
                {
                    if (!pDoAsyncDownload)
                        _jsonData = BaseWebConn.doFetchJsonData(_FromUrl);
                    else
                    {
                        using (EventProcNextMerchant = new ManualResetEvent(false))
                        EventProcNextMerchant = new ManualResetEvent(false);
                        {
                            BaseWebConn.doFetchJsonDataAsync(_FromUrl, (object sender, DownloadStringCompletedEventArgs e) =>
                            {
                                _jsonData = "";
                                if (!_fmtcCancelRequested)
                                {
                                    if (!e.Cancelled && e.Error == null)
                                    {
                                        _jsonData = (string)e.Result;
                                        /*
                                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\Documents\DownloadFmtc.txt", true))
                                        {
                                            file.WriteLine(_jsonData);
                                        }*/
                                    }
                                    else
                                    {
                                        Console.WriteLine("Download returned - with error - " + e.Error.Message);
                                    }
                                }
                                EventProcNextMerchant.Set();
                            });
                            EventProcNextMerchant.WaitOne();
                            if (_fmtcCancelRequested)
                            {
                                FmtcDownloadOpMessage = "Download operation cancelled";
                                ResetFmtcDealsSemaphore();
                                return false;
                            }
                            if (_jsonData == "")
                            {
                                return false;
                            }
                        }
                    }
                }

            //Console.WriteLine("Processing download data");
            JArray _arrayMain = JArray.Parse(_jsonData);
            if (!pPostDownloadDelegate(_arrayMain))
                return true;
            foreach (JToken _ele in _arrayMain)
            {
                //JObject _prosperentData = JsonConvert.DeserializeObject(_jsonData) as JObject;
                try
                {
                    JObject _prosperentData = JsonConvert.DeserializeObject(_ele.ToString()) as JObject;
                }
                catch (JsonReaderException _e)
                {
                    FmtcDownloadOpMessage = _e.Message;
                    return false;
                }
                if (!pProcessDataRecord(_ele))
                    return false;
            }
            return true;
        }

        T convertToObject<T>(JObject obj)
        {
            JsonSerializer serializer = new JsonSerializer();
            T _retVal = (T)serializer.Deserialize(new JTokenReader(obj), typeof(T));
            return _retVal;
        }

        QponResponse convertToResponse(JObject obj)
        {
            return convertToObject<QponResponse>(obj);
        }

        public QponRegisterAdmin doRegisterAdmin(String pFirstname, String pLastName, String pPassword, String pEmail)
        {
            QponRegisterAdmin _retVal = null;
            QponRegisterAdmin.Admin _jObj = new QponRegisterAdmin.Admin();
            _jObj.FirstName = pFirstname;
            _jObj.LastName = pLastName;
            _jObj.Password = pPassword;
            _jObj.Email = pEmail;
            //_jObj.data.admin.AdminUserProfileId = pEmail;
            String _sendJson = JsonConvert.SerializeObject(_jObj);
            Console.WriteLine(_sendJson);
            QponResponse _res = null;
            doSendDataWithWait(_sendJson, _cRegisterAdmin, (JObject response) =>
            {
                _res = convertToResponse(response);
                return true;
            });
            if (!_res.success)
            {
                //acquire 
                doSendDataWithWait(_sendJson, _cLoginAdmin, (JObject response) =>
                {
                    _res = convertToResponse(response);
                    return true;
                });
            }
            if (_res.success)
            {
                _retVal = convertToObject<QponRegisterAdmin>(_res.data);
                Console.WriteLine(_retVal.admin.AdminUserProfileId + " : " + _retVal.admin.AccessToken);
            }
            return _retVal;
        }

        public bool DownloadJsonData(
            Uri pFromUrl, bool pUsePageBasedLoad,
            Func<JObject, bool> pPostDownloadDelegate, 
            Func<JToken, bool> pProcessDataRecord,
            Func<JObject, bool> pContinueFetch 
            )
        {
            String _jsonData = "";
            if (pFromUrl == null)
                _jsonData = File.ReadAllText(@"F:\WorkRelated\Qpon\deals.json");
            else
                _jsonData = BaseWebConn.doFetchJsonData(pFromUrl);
            Console.WriteLine(pFromUrl);
            JObject _prosperentData = JsonConvert.DeserializeObject(_jsonData) as JObject;
            int _page = 0;
            while (_prosperentData.HasValues)
            {
                if (!pPostDownloadDelegate(_prosperentData))
                    return true;
                JToken _data = null;
                if (_prosperentData.TryGetValue("data", out _data))
                {
                    if (_data is JArray)
                    {
                        JArray _array = _data as JArray;
                        int _procCount = 0;
                        foreach (JToken _ele in _array)
                        {
                            _procCount++;
                            Console.WriteLine(String.Format("JArray {0} of {1}", _procCount, _array.Count));
                            if (!pProcessDataRecord(_ele))
                                return false;
                        }
                    }
                }
                //perhaps use non generic in case api chanes?
                /* for Prosperent processing use this in the calling method
                int _totalrecords = _prosperentData.Value<int>("totalRecords");
                int _limit = _prosperentData.Value<int>("limit");
                int _page = _prosperentData.Value<int>("page");
                if (_totalrecords < _limit)
                    return true;
                 * */
                if (!pContinueFetch(_prosperentData))
                    return true;
                if (pFromUrl != null)
                {
                    if (pUsePageBasedLoad)
                    {
                        pFromUrl = setUrlPageTo(pFromUrl, ++_page);
                        _jsonData = BaseWebConn.doFetchJsonData(pFromUrl);
                    }
                }
                _prosperentData = JsonConvert.DeserializeObject(_jsonData) as JObject;
            }
            return true;
        }

        #region parameter and sql command helper methods
        private static DateTime? parseIsoDate(String pDateTimeAsStr)
        {
            const DateTimeStyles style = DateTimeStyles.AllowWhiteSpaces;
            const String _cIso8601 = "yyyy-MM-ddTHH:mm:ss";
            DateTime? _retVal = null;
            DateTime _date;
            /*
            if (DateTime.TryParseExact(pDateTimeAsStr, _cIso8601,
                CultureInfo.InvariantCulture, style, out _date))
             */
            if (DateTime.TryParse(pDateTimeAsStr, out _date))
                _retVal = _date;
            return _retVal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pCmd"></param>
        /// <param name="pParamName"></param>
        /// <param name="pToken"></param>
        /// <returns></returns>
        public static bool SetParameterValueFromToken<T>(SqlCommand pCmd, String pParamName, JToken pToken)
        {
            String _paramName = String.Format("@{0}", pParamName);
            if (pCmd.Parameters.IndexOf(_paramName) < 0)
                return false;
            if (pToken[pParamName] == null || pToken[pParamName].ToString() == "")
            {
                pCmd.Parameters[_paramName].Value = DBNull.Value;
                return true;
            }
            Type _type = typeof(T);
            object _tData = null;
            String _tDataS = pToken[pParamName].ToString();
            

            if (typeof(T) == typeof(System.DateTime))
            {
                if (_tDataS == "")
                    _tData = null;
                else
                    _tData = parseIsoDate(_tDataS.ToString());
            }
            else
                if (typeof(T) == typeof(System.Int32))
            {
                if (_tDataS == "")
                    _tData = 0;
                else
                    _tData = int.Parse(_tDataS);
            }
            else
            if (typeof(T) == typeof(System.Double))
            {
                if (_tDataS == "")
                    _tData = 0.0;
                else
                    _tData = Double.Parse(_tDataS);
            }
            else
                _tData = _tDataS; // default to string
            if (_tData != null)
                pCmd.Parameters[_paramName].Value = (T)Convert.ChangeType(_tData, _type, CultureInfo.InvariantCulture);
            else
                pCmd.Parameters[_paramName].Value = null;
            return true;
        }

        private static SqlParameter setNullable(SqlParameter pParam)
        {
            pParam.IsNullable = true;
            pParam.Value = DBNull.Value;
            return pParam;
        }

        #endregion

        #region methods for "product" table
                /// <summary>
                /// methods for "product" table
                /// </summary>
                /// <returns></returns>
                public static SqlCommand CmdForProduct()
                {
                    SqlCommand _cmd = new SqlCommand("dbo.spProsperentProduct");
                    _cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    _cmd.Parameters.Add(new SqlParameter("@updateAction", System.Data.SqlDbType.Int));
                    _cmd.Parameters.Add(new SqlParameter("@catalogId", System.Data.SqlDbType.NVarChar, 50));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@productId", System.Data.SqlDbType.NVarChar, 50)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@affiliate_url", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@image_url", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@keyword", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@description", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@category", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@merchant", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@brand", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@upc", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@isbn", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@sales", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@price", System.Data.SqlDbType.Money)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@percentOff", System.Data.SqlDbType.Money)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@price_sale", System.Data.SqlDbType.Money)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@merchantId", System.Data.SqlDbType.NVarChar, 250)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@currency", System.Data.SqlDbType.NVarChar, 50)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@createdOn", System.Data.SqlDbType.DateTime)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@updatedOn", System.Data.SqlDbType.DateTime)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));
                    _cmd.Parameters.Add(setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));
                    return _cmd;
                }

                public bool UpdateProductToDb(JToken pjToken, SqlCommand pCmd)
                {
                    SetParameterValueFromToken<String>(pCmd, "catalogId", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "productId", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "affiliate_url", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "image_url", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "keyword", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "description", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "category", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "merchant", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "brand", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "upc", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "isbn", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "sales", pjToken);
                    SetParameterValueFromToken<System.Double>(pCmd, "price", pjToken);
                    SetParameterValueFromToken<System.Double>(pCmd, "price_sale", pjToken);
                    SetParameterValueFromToken<System.Double>(pCmd, "percentOff", pjToken);
                    SetParameterValueFromToken<System.Double>(pCmd, "merchantId", pjToken);
                    SetParameterValueFromToken<String>(pCmd, "currency", pjToken);
                    try
                    {
                        pCmd.Parameters["@updateAction"].Value = 1;
                        pCmd.Parameters["@updateUser"].Value = 0;
                        pCmd.ExecuteNonQuery();
                    }
                    catch (Exception _e)
                    {
                        Console.WriteLine(_e.Message);
                        return false;
                    }
                    return true;
                }
        #endregion

        #region Methods For "Merchants"
        public static SqlCommand CmdForMerchant()
        {
            SqlCommand _cmd = new SqlCommand("dbo.spProsperentMerchant");
            _cmd.CommandType = System.Data.CommandType.StoredProcedure;
            _cmd.Parameters.Add(new SqlParameter("@updateAction", System.Data.SqlDbType.Int));
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@merchant", System.Data.SqlDbType.NVarChar, 50)));//NVARCHAR (50)  NOT NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@merchantId", System.Data.SqlDbType.NVarChar, 50)));//NVARCHAR (50) NOT NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@logoUrl", System.Data.SqlDbType.NVarChar, 250)));//NVARCHAR (250) NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@image_url", System.Data.SqlDbType.NVarChar, 250)));//NVARCHAR (250) NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@domain", System.Data.SqlDbType.NVarChar, 250)));//NVARCHAR (250) NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@category", System.Data.SqlDbType.NVarChar, 250)));//NVARCHAR (250) NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@productDatafeed", System.Data.SqlDbType.Int)));//int NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@numProducts", System.Data.SqlDbType.Int)));//int NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@numProductsCA", System.Data.SqlDbType.Int)));//int NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@numProductsUK", System.Data.SqlDbType.Int)));//int NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@numCouponsUs", System.Data.SqlDbType.Int)));//int NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@numLocalDealsUS", System.Data.SqlDbType.Int)));//int NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@numTravelOffersUS", System.Data.SqlDbType.Int)));//int NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@minPaymentPercentage", System.Data.SqlDbType.Real)));//real NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@maxPaymentPercentage", System.Data.SqlDbType.Real)));//real NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@averagePaymentPercentage", System.Data.SqlDbType.Real)));//real NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@conversionRate", System.Data.SqlDbType.Real)));//real NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@epc", System.Data.SqlDbType.Real)));//real NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@merchantWeight", System.Data.SqlDbType.Real)));//real NULL,
	        _cmd.Parameters.Add(setNullable(new SqlParameter("@dateActive", System.Data.SqlDbType.NVarChar, 50)));//NVARCHAR (250) NULL,
            //_cmd.Parameters.Add(setNullable(new SqlParameter("@vendorId", System.Data.SqlDbType.NVarChar, 100)));//NVARCHAR (100) NULL,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@createdOn", System.Data.SqlDbType.NVarChar, 50)));//DATE			NULL,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updatedOn", System.Data.SqlDbType.NVarChar, 50)));//DATE			NULL,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int)));//INTEGER			NULL,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int)));//NVARCHAR (31)	NULL,
            return _cmd;
        }

        public bool UpdateMerchantToDb(JToken pjToken, SqlCommand pCmd)
        {
            SetParameterValueFromToken<String>(pCmd, "merchant", pjToken);
            SetParameterValueFromToken<String>(pCmd, "merchantId", pjToken);
            SetParameterValueFromToken<String>(pCmd, "logoUrl", pjToken);
            SetParameterValueFromToken<String>(pCmd, "image_url", pjToken);
            SetParameterValueFromToken<String>(pCmd, "domain", pjToken);
            SetParameterValueFromToken<String>(pCmd, "category", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "productDatafeed", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "numProducts", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "numProductsCA", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "numProductsUK", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "numCouponsUs", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "numLocalDealsUS", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "numTravelOffersUS", pjToken);
            SetParameterValueFromToken<System.Double>(pCmd, "minPaymentPercentage", pjToken);
            SetParameterValueFromToken<System.Double>(pCmd, "maxPaymentPercentage", pjToken);
            SetParameterValueFromToken<System.Double>(pCmd, "averagePaymentPercentage", pjToken);
            SetParameterValueFromToken<System.Double>(pCmd, "conversionRate", pjToken);
            SetParameterValueFromToken<System.Double>(pCmd, "epc", pjToken);
            SetParameterValueFromToken<System.Double>(pCmd, "merchantWeight", pjToken);
            SetParameterValueFromToken<String>(pCmd, "dateActive", pjToken);
            try
            {
                pCmd.Parameters["@updateAction"].Value = 1;
                pCmd.Parameters["@updateUser"].Value = 0;
                pCmd.ExecuteNonQuery();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
                return false;
            }
            return true;
        }

        #endregion Methods For "Merchants"

        #region Methods For "FMTC Deals"
        public static SqlCommand CmdForFmtcDeals()
        {
            SqlCommand _cmd = new SqlCommand("dbo.spFmtcDeals");
            _cmd.CommandType = System.Data.CommandType.StoredProcedure;
            _cmd.Parameters.Add(new SqlParameter("@updateAction", System.Data.SqlDbType.Int));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nCouponID", System.Data.SqlDbType.Int, 50))); //int NOT NULL,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cMerchant", System.Data.SqlDbType.NVarChar, 150))); //NVARCHAR (150)  NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nMerchantID", System.Data.SqlDbType.Int))); //int NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nMasterMerchantID", System.Data.SqlDbType.Int))); //int NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cNetwork", System.Data.SqlDbType.NVarChar, 50))); //NVARCHAR(50) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cStatus", System.Data.SqlDbType.NVarChar, 50))); //NVARCHAR(50) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cLabel", System.Data.SqlDbType.NVarChar, 50))); //NVARCHAR(50) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cImage", System.Data.SqlDbType.NVarChar, 250))); //cImage
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cRestrictions", System.Data.SqlDbType.NVarChar, 250))); //NVARCHAR (250) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cCode", System.Data.SqlDbType.NVarChar, 250))); //NVARCHAR (250) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@dtStartDate", System.Data.SqlDbType.DateTime))); //DATE NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@dtEndDate", System.Data.SqlDbType.DateTime))); //DATE NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cLastUpdated", System.Data.SqlDbType.DateTime))); //DATE NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cCreated", System.Data.SqlDbType.DateTime))); //DATE NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cAffiliateURL", System.Data.SqlDbType.NVarChar, 250))); //NVARCHAR (250) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cDirectURL", System.Data.SqlDbType.NVarChar, 250))); //cDirectURLcDirectURL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cSkimlinksURL", System.Data.SqlDbType.NVarChar, 250))); //NVARCHAR (250) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cFMTCURL", System.Data.SqlDbType.NVarChar, 250))); //NVARCHAR (250) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@fmtcCategory", System.Data.SqlDbType.NVarChar, 250))); //NVARCHAR (250) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@aTypes", System.Data.SqlDbType.NVarChar, 50))); //NVARCHAR (250) NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@fSalePrice", System.Data.SqlDbType.Money))); //MONEY NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@fWasPrice", System.Data.SqlDbType.Money))); //MONEY NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@fDiscount", System.Data.SqlDbType.Money))); //MONEY NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nPercent", System.Data.SqlDbType.Money))); //MONEY NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@createdOn", System.Data.SqlDbType.DateTime))); //DATE			NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updatedOn", System.Data.SqlDbType.DateTime))); //DATE			NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int))); //INTEGER			NULL
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int))); //
            return _cmd;
        }

        public bool UpdateFmtcDealsToDb(JToken pjToken, SqlCommand pCmd)
        {
            //aTypes":["coupon","rebates","category-coupon"]
            //"aCategories":["food-cooking","online-grocery"]
            SetParameterValueFromToken<System.Int32>(pCmd, "nCouponID", pjToken); //int NOT NULL,
            SetParameterValueFromToken<String>(pCmd, "cMerchant", pjToken); //NVARCHAR (150)  NULL,
            SetParameterValueFromToken<System.Int32>(pCmd, "nMerchantID", pjToken); //int NULL,
            SetParameterValueFromToken<System.Int32>(pCmd, "nMasterMerchantID", pjToken); //int NULL,
            SetParameterValueFromToken<String>(pCmd, "cNetwork", pjToken); //NVARCHAR(50) NULL,
            SetParameterValueFromToken<String>(pCmd, "cStatus", pjToken); //NVARCHAR(50) NULL,
            SetParameterValueFromToken<String>(pCmd, "cLabel", pjToken); //NVARCHAR(50) NULL,
            SetParameterValueFromToken<String>(pCmd, "cImage", pjToken); //NVARCHAR (250) NULL,
            SetParameterValueFromToken<String>(pCmd, "cRestrictions", pjToken); //NVARCHAR (250) NULL,
            SetParameterValueFromToken<String>(pCmd, "cCode", pjToken); //NVARCHAR (250) NULL,
            SetParameterValueFromToken<DateTime>(pCmd, "dtStartDate", pjToken); //DATE NULL,
            SetParameterValueFromToken<DateTime>(pCmd, "dtEndDate", pjToken); //DATE NULL,
            SetParameterValueFromToken<DateTime>(pCmd, "cLastUpdated", pjToken); //DATE NULL,
            SetParameterValueFromToken<DateTime>(pCmd, "cCreated", pjToken); //DATE NULL,
            SetParameterValueFromToken<String>(pCmd, "cAffiliateURL", pjToken); //NVARCHAR (250) NULL,
            SetParameterValueFromToken<String>(pCmd, "cDirectURL", pjToken); //NVARCHAR (250) NULL,
            SetParameterValueFromToken<String>(pCmd, "cSkimlinksURL", pjToken); //NVARCHAR (250) NULL,
            SetParameterValueFromToken<String>(pCmd, "cFMTCURL", pjToken); //NVARCHAR (250) NULL,
            //SetParameterValueFromToken<System.Double>(pCmd, "aTypes", pjToken); //NVARCHAR (250) NULL,
            SetParameterValueFromToken<System.Double>(pCmd, "fSalePrice", pjToken); //MONEY NULL,
            SetParameterValueFromToken<System.Double>(pCmd, "fWasPrice", pjToken); //MONEY NULL,
            SetParameterValueFromToken<System.Double>(pCmd, "fDiscount", pjToken); //MONEY NULL,
            SetParameterValueFromToken<System.Double>(pCmd, "nPercent", pjToken); //MONEY NULL,
            SetParameterValueFromToken<System.DateTime>(pCmd, "createdOn", pjToken); //DATE			NULL,
            SetParameterValueFromToken<System.DateTime>(pCmd, "updatedOn", pjToken); //DATE			NULL,
            //SetParameterValueFromToken<System.Int32>(pCmd, "updateStatus", pjToken); //INTEGER			NULL,
            //SetParameterValueFromToken<System.String>(pCmd, "updateUser", pjToken); //int	NULL,
            if (pjToken["aCategories"] != null)
            {
                JArray _cats = (JArray)pjToken["aCategories"];
                StringBuilder _category = new StringBuilder();
                foreach (JToken _tok in _cats)
                {
                    _category.Append(_tok.Value<String>() + ";");
                }
                pCmd.Parameters["@fmtcCategory"].Value = _category.ToString();
            }

            try
            {
                pCmd.Parameters["@updateAction"].Value = 1;
                pCmd.Parameters["@updateUser"].Value = 0;
                pCmd.ExecuteNonQuery();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
                return false;
            }
            return true;
        }

        #endregion Methods For "FMTC Deals"

        #region Methods For "FMTC Merchants "
        public static SqlCommand CmdForFmtcMerchant()
        {
            SqlCommand _cmd = new SqlCommand("dbo.spFmtcMerchants");
            _cmd.CommandType = System.Data.CommandType.StoredProcedure;
            _cmd.Parameters.Add(new SqlParameter("@updateAction", System.Data.SqlDbType.Int));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nMerchantID", System.Data.SqlDbType.Int, 50))); //int NOT NULL,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nMasterMerchantID", System.Data.SqlDbType.Int))); //int NULL,,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nSkimlinksID", System.Data.SqlDbType.Int)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cName", System.Data.SqlDbType.NVarChar, 250)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cNetwork", System.Data.SqlDbType.NVarChar, 250)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cProgramID", System.Data.SqlDbType.NVarChar, 250)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@nParentMerchantID", System.Data.SqlDbType.Int))); //int NULL,,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cAffiliateURL", System.Data.SqlDbType.NVarChar, 250)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cSkimlinksURL", System.Data.SqlDbType.NVarChar, 250)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cFMTCURL", System.Data.SqlDbType.NVarChar, 250)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@cHomepageURL", System.Data.SqlDbType.NVarChar, 250)));
            _cmd.Parameters.Add(setNullable(new SqlParameter("@dtCreated", System.Data.SqlDbType.DateTime))); //DATE NULL,,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@dtLastUpdated", System.Data.SqlDbType.DateTime))); //DATE NULL,,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@logoUrl", System.Data.SqlDbType.NVarChar, 250)));
            
            _cmd.Parameters.Add(setNullable(new SqlParameter("@createdOn", System.Data.SqlDbType.DateTime))); //DATE			NULL,,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updatedOn", System.Data.SqlDbType.DateTime))); //DATE			NULL,,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updateStatus", System.Data.SqlDbType.Int))); //INTEGER			NULL,,
            _cmd.Parameters.Add(setNullable(new SqlParameter("@updateUser", System.Data.SqlDbType.Int))); //
            return _cmd;
        }

        public bool UpdateFmtcMerchantsToDb(JToken pjToken, SqlCommand pCmd)
        {
            SetParameterValueFromToken<System.Int32>(pCmd, "nMerchantID", pjToken); //int NOT NULL,
            SetParameterValueFromToken<System.Int32>(pCmd, "nMasterMerchantID", pjToken); //int NOT NULL,
            SetParameterValueFromToken<System.Int32>(pCmd, "nSkimlinksID", pjToken); //int NOT NULL,
            SetParameterValueFromToken<String>(pCmd, "cName", pjToken);
            SetParameterValueFromToken<String>(pCmd, "cNetwork", pjToken);
            SetParameterValueFromToken<String>(pCmd, "cMerchant", pjToken);
            SetParameterValueFromToken<System.Int32>(pCmd, "nParentMerchantID", pjToken); //int NOT NULL,
            SetParameterValueFromToken<String>(pCmd, "cAffiliateURL", pjToken);
            SetParameterValueFromToken<String>(pCmd, "cSkimlinksURL", pjToken);
            SetParameterValueFromToken<String>(pCmd, "cFMTCURL", pjToken);
            SetParameterValueFromToken<String>(pCmd, "cHomepageURL", pjToken);
            SetParameterValueFromToken<DateTime>(pCmd, "dtCreated", pjToken); //DATE NULL,
            SetParameterValueFromToken<DateTime>(pCmd, "dtLastUpdated", pjToken); //DATE NULL,
            /*
            if (pjToken["aLogos"] != null)
            {
                JArray _logos = pjToken.Value<JArray>("aLogos");
                if (_logos.Count > 0) 
                {
                    pCmd.Parameters["@logoUrl"].Value = _logos[0].Value<string>("cURL");
                }
            }*/

            //SetParameterValueFromToken<System.DateTime>(pCmd, "createdOn", pjToken); //DATE			NULL,
            //SetParameterValueFromToken<System.DateTime>(pCmd, "updatedOn", pjToken); //DATE			NULL,
            //SetParameterValueFromToken<System.Int32>(pCmd, "updateStatus", pjToken); //INTEGER			NULL,
            //SetParameterValueFromToken<System.String>(pCmd, "updateUser", pjToken); //int	NULL,
            try
            {
                pCmd.Parameters["@updateAction"].Value = 1;
                pCmd.Parameters["@updateUser"].Value = 0;
                pCmd.ExecuteNonQuery();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
                return false;
            }
            return true;
        }

        #endregion Methods For "FMTC Merchants"


        #region unused code
        public bool SaveData(JToken pjToken, SqlCommand pCmd)
        {
            if (!pjToken.HasValues)
                return false;
            JToken _token = pjToken.First;
            while (_token != null)
            {
                if (_token is JProperty) { 
                    JProperty _prop = _token as JProperty;
                    //Console.WriteLine(String.Format("{0}={1}", _prop.Name, _prop.Value.ToString()));
                    String _paramName = String.Format("@{0}", _prop.Name);
                    if (pCmd.Parameters.IndexOf(_paramName) >= 0)
                    {
                        if (!_prop.Value.HasValues)
                        {
                            pCmd.Parameters[_paramName].Value = null;
                        }
                        else
                        {
                            pCmd.Parameters[_paramName].Value = _prop.Value;
                        }
                    }
                    _token = _token.Next;
                }
            }
            try
            {
                pCmd.Parameters["@updateAction"].Value = 1;
                pCmd.ExecuteNonQuery();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
                return false;
            }
            return true;
        }
    #endregion

    }

    public class QponRegisterAdmin
    {
        public class Admin
        {
            public String AdminUserProfileId;
            public String FirstName;
            public String LastName;
            public String Password;
            public String Email;
            public String AccessToken;
        }
        public QponRegisterAdmin()
        {
            admin = new Admin();
        }
        public Admin admin;
    }

    public class QponResponseError
    {
        public String code;
        public String desc;
    }

    public class QponResponse
    {
        public JObject data;
        public bool success;
        public QponResponseError error;
    }
}
