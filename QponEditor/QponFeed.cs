using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Data;

namespace QponEditor
{
    public class QponFeed
    {
        /// <API Call Samples>
        ///http://qponcrush.azurewebsites.net:80/api/User/addBaseCoupon?api_key=CE2C6279-D782-4FA8-B474-51A4E166BA65
        ///
        /// </summary>
        private String _cBaseAzureCall = @"http://qponcrush.azurewebsites.net:80/api/User/";
        //private String _cBaseAzureCall = @"http://social.krisco.ca:80/api/User/";
        private String _cSwaggerAppKey = @"CE2C6279-D782-4FA8-B474-51A4E166BA65";
        private String _cAddbaseCoupon = @"addbaseCoupon";
        private String _cRegisterAdmin = "registerAdmin";
        private String _cLoginAdmin = "loginAdmin";
        private String _cRegisterVendor = "registerVendor";
        private String _cGetVendorDetailAdmin = "getVendorDetailAdmin";
        //private String _cGetVendorById = "GetVendorById";
        private String _cgetCouponCategory = "getCouponCategory";
        private String _cAddBaseCouponAdmin = "addBaseCouponAdmin";
        //debug AccessToken = "f03a7bc1-03b8-403d-8036-57f372dbab5c";
        private QponRegisterAdmin _defaultAccessToken = null;
        public int EditUserId { get; set; }

        public bool doSendDataWithWait(String pJsonData, String pApiTag, Func<JObject, bool> pRespCallback)
        {
            Uri _azureCall = new Uri(String.Format("{0}/{1}", _cBaseAzureCall, pApiTag));
            HttpWebRequest _request = (HttpWebRequest) WebRequest.Create(_azureCall);
            _request.Method = "POST";
            _request.ContentType = @"application/json; charset=utf-8";
            _request.Headers.Add("AppKey", _cSwaggerAppKey);
            using (StreamWriter _webStream = new StreamWriter(_request.GetRequestStream())) {
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

        private bool jWriteValue(JsonWriter _writer, String pProperty, String pValue) 
        {
            _writer.WritePropertyName(pProperty);
            _writer.WriteValue(pValue);
            return true;
        }

        private bool jWriteRowValue(JsonWriter _writer, DataRow pRow, String pProperty, String pFieldName)
        {
            jWriteValue(_writer, "BaseCouponId", pRow[pFieldName].ToString());
            return true;
        }

        public T convertToObject<T>(JObject obj)
        {
            JsonSerializer serializer = new JsonSerializer();
            T _retVal = (T)serializer.Deserialize(new JTokenReader(obj), typeof(T));
            return _retVal;
        }

        QponResponse convertToResponse(JObject obj)
        {
            return convertToObject<QponResponse>(obj);
        }

        class GetVendorDetailAdminCl { public String AccessToken; public String Username; public String Email;}
        public QponResponse doRegisterVendor(RegisterVendorJS pVendor)
        {
            QponResponse _retVal = null;
            String _sendJson = JsonConvert.SerializeObject(pVendor);
            Console.WriteLine(_sendJson);
            doSendDataWithWait(_sendJson, _cRegisterVendor, (JObject response) =>
            {
                Console.WriteLine(response);
                _retVal = convertToResponse(response);
                return true;
            });
            if (!_retVal.success)
            {
                if (_retVal.error.code == "3")
                {
                    
                    GetVendorDetailAdminCl _getVendorDetailAdmin = new GetVendorDetailAdminCl();
                    _getVendorDetailAdmin.AccessToken = DefaultAccessToken().admin.AccessToken;
                    _getVendorDetailAdmin.Username = pVendor.Name;
                    _getVendorDetailAdmin.Email = pVendor.Email;

                    /*_sendJson = String.Format(@"{\"AccessToken\":\"{0}\", \"Username\":\"{1}\", \"Email\":\"{2}\" }",
                        DefaultAccessToken().admin.AccessToken, pVendor.Name, pVendor.Email);*/
                    _sendJson = JsonConvert.SerializeObject(_getVendorDetailAdmin);
                    doSendDataWithWait(_sendJson, _cGetVendorDetailAdmin, (JObject response) =>
                    {
                        Console.WriteLine(response);
                        _retVal = convertToResponse(response);
                        return true;
                    });
                }
            }
            return _retVal;
        }

        public QponResponse doAddbaseCouponAdmin(BaseCouponJD pCoupon)
        {
            QponResponse _retVal = null;
            String _sendJson = JsonConvert.SerializeObject(pCoupon);
            Console.WriteLine(_sendJson);
            doSendDataWithWait(_sendJson, _cAddBaseCouponAdmin, (JObject response) =>
            {
                Console.WriteLine(response);
                _retVal = convertToResponse(response);
                return true;
            });
            return _retVal;
        }

        public JArray getCategoryList()
        {
            JArray _retVal = null;
            String _sendJson = "{ \"AccessToken\": \"{" + DefaultAccessToken().admin.AccessToken + "}\" }";
            QponResponse _res = null;
            doSendDataWithWait(_sendJson, _cgetCouponCategory, (JObject response) =>
            {
                _res = convertToResponse(response);
                if (_res.success)
                {
                    _retVal = _res.data.Value<JArray>("couponCategories");
                }
                return true;
            });

         

            return _retVal;
        }

        public QponRegisterAdmin DefaultAccessToken()
        {
            if (_defaultAccessToken == null)
                _defaultAccessToken = doRegisterAdmin("Munish", "Madan", "Munish@", "admin@socialqpon.com");
            return _defaultAccessToken;
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

    }

}

