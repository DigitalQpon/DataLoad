using Newtonsoft.Json.Linq;
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
using System.Net;
using System.Threading;

namespace QponCollector
{
    public partial class MainF : Form
    {
        public MainF()
        {
            InitializeComponent();
            // default timer to 10:00 pm daily
            DateTime _dt = DateTime.Today;
            TimeSpan _t = new TimeSpan(22, 0, 0);
            _dt = _dt.Date + _t;
            setTimeDownloadEdt.Value = _dt;
        }
        static String _connStr = @"Server=tcp:oud11hbwrc.database.windows.net,1433;Database=intermediate-qponcrush_db;User ID=saviobernard@oud11hbwrc;Password=QponCrush15;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

        SqlConnection _conn = new SqlConnection(_connStr);
        bool _isInProcess = false;
        DateTime _lastTriggerDate = DateTime.Today.AddDays(-1);
        CouponFeedProcess _proc = new CouponFeedProcess();

        private void StatusMsg(String pMsg) {
          /*  Invoke((MethodInvoker)delegate
            {
                statusLB.Text = pMsg;
                Console.WriteLine(pMsg);
            }); */
        }

        private bool doProcessMerchants(SqlConnection pConn, CouponFeedProcess pProc)
        {
            ///
            /// ------------------
            /// process Merchants
            ///------------------
            ///
            Dictionary<String, Object> _query = null;
            SqlCommand _cmd = null;
            Uri _url = null;
            int _recordsProcessed = 0;
            String _msg = "";
            //
            _cmd = CouponFeedProcess.CmdForMerchant();
            _cmd.Connection = pConn;
            _query = new Dictionary<string, object>{
                        {"limit", 100}
                        };
            _url = _url = BaseWebConn.BuildUriFrom("http://api.prosperent.com/api/merchant?",
                _query);
            _recordsProcessed = 0;
            pProc.DownloadJsonData(_url, true,
                // preprocess just after the download occurs
                (JObject _obj) =>
                {
                    //const int _maxPages = 1;
                    //return ++_count <= _maxPages; // 1 page has all the information
                    _recordsProcessed += _obj.Value<int>("totalRecords");
                    //StatusMsg(String.Format("Merchants Page {0}. Records {1}", ++_count, _recordsProcessed));
                    return _recordsProcessed < 500;
                },
                // process each record
                (JToken ptoken) =>
                {
                    return pProc.UpdateMerchantToDb(ptoken, _cmd);
                },
                // indica if the fetch should continue
                (JObject _obj) =>
                {
                    int _limit = _obj.Value<int>("limit");
                    int _totalrecords = _obj.Value<int>("totalRecords");
                    //int _page = _prosperentData.Value<int>("page");
                    return _totalrecords >= _limit;
                }
            );
            _msg = String.Format("{0} Merchants {1}", _msg, _recordsProcessed);
            StatusMsg(_msg);
            Application.DoEvents();
            return true;
        }


        private bool doProcessProducts(SqlConnection pConn, CouponFeedProcess pProc)
        {
            ///
            /// ------------------
            /// process Products
            ///------------------
            ///

            SqlCommand _merchantCmd = new SqlCommand();
            _merchantCmd.Connection = pConn;
            _merchantCmd.CommandText = "Select merchant from [dbo].[ProsperentMerchant] WHERE updateStatus = 2 ";
            DataTable _merchants = new DataTable();
            SqlDataAdapter _merDa = new SqlDataAdapter(_merchantCmd);
            _merDa.Fill(_merchants);
            int _recordsProcessed = 0;
            int _recordsAdded = 0;
            String _msg = "";
            foreach (DataRow _row in _merchants.Rows)
            {
                Dictionary<String, Object> _query = null;
                SqlCommand _cmd = null;
                Uri _url = null;
                //
                ///------------------
                /// process Products
                ///------------------
                _cmd = CouponFeedProcess.CmdForProduct();
                _cmd.Connection = pConn;
                int _recordsAddedForMerchant = 0;
                _query = new Dictionary<string, object>{
                        {"limit", 1000}, // limt for testng - number of merchants
                        {"filterMerchant", 
                            String.Format("{0}", _row["merchant"]) 
                            //"FineJewelers.com"
                        }
                        };
                _url = BaseWebConn.BuildUriFrom("http://api.prosperent.com/api/search?api_key=633bab575e0bf71d77701e75de8d6712",
                    _query);
                _msg = String.Format("Processing {0} - ", _row["merchant"].ToString(), _msg);
                StatusMsg(_msg);
                pProc.DownloadJsonData(_url, true,
                    // preprocess the download infor
                    (JObject _obj) =>
                    {
                        _recordsProcessed += _obj.Value<int>("totalRecords");
                        //StatusMsg(String.Format("Products Loop {0}. Records {1} Added {2}", _count + 1, _recordsProcessed, _recordsAdded));
                        return _recordsProcessed < 5000;
                    },
                    // process the data record
                    (JToken ptoken) =>
                    {
                        _recordsAdded++;
                        _recordsAddedForMerchant++;
                        return pProc.UpdateProductToDb(ptoken, _cmd);
                    },
                    // continue fetching?
                    (JObject _obj) =>
                    {
                        int _limit = _obj.Value<int>("limit");
                        int _totalrecords = _obj.Value<int>("totalRecords");
                        //int _page = _prosperentData.Value<int>("page");
                        return _totalrecords >= _limit;
                    }
                );
                _msg = String.Format(". Processed {0}, Records {1}", _row["merchant"].ToString(), _recordsAddedForMerchant);
                StatusMsg(_msg);
            }
            _msg = String.Format("{0} Products Added {1}", _msg, _recordsAdded);
            StatusMsg(_msg);
            return true;
        }

        private bool doProcessFmtcMerchants(SqlConnection pConn, CouponFeedProcess pProc)
        {
            Dictionary<String, Object> _query = null;
            SqlCommand _cmd = null;
            Uri _url = null;
            int _count = 0;
            int _recordsProcessed = 0;
            String _msg = "";
            //
            ///------------------
            /// process Products
            ///------------------
            _cmd = CouponFeedProcess.CmdForFmtcMerchant();
            _cmd.Connection = pConn;
            _query = new Dictionary<string, object> {
            };
            _url = BaseWebConn.BuildUriFrom("http://services.fmtc.co/v2/getMerchants?key=ae6eaeba6d5fac959544bc24b087950e",
                _query);
            CouponFeedProcess.JsonDataDownloadType _jsonFrom = new CouponFeedProcess.JsonDataDownloadType();
            _jsonFrom.fromUri = _url;
            pProc.DownloadJsonDataArray(
                false,
                _jsonFrom,
                // Called after download of json data
                (JArray _obj) =>
                {
                    _count = _obj.Count;
                    StatusMsg(String.Format("Fmtc Merchant Records {0}. Records {1}", _count, _recordsProcessed));
                    return true; // 1 page has all the information
                },
                //What do we do with the data
                (JToken ptoken) =>
                {
                    _recordsProcessed++;
                    StatusMsg(String.Format("Fmtc Merchant Records {0}. Records {1}", _count, _recordsProcessed));
                    return pProc.UpdateFmtcMerchantsToDb(ptoken, _cmd);
                }
            );
            _msg = String.Format("{0} Merchants {1}", _msg, _recordsProcessed);
            StatusMsg(_msg);
            return true;
        }

        private void updateFmtcToDb(JToken pToken, SqlCommand pCmd)
        {
            Invoke((MethodInvoker)delegate
            {
                _proc.UpdateFmtcDealsToDb(pToken, pCmd);
            });
        }

        int temp_nMerchantID = 0;

        private void doFmtDealsFetch(CouponFeedProcess.JsonDataDownloadType pUrlOrFilename, CouponFeedProcess pProc, SqlCommand pCmd)
        {
            int _count = 0;
            int _recordsProcessed = 0, _recordsAdded = 0;
            //String _msg = "";
            pProc.DownloadJsonDataArray(
                pUrlOrFilename.DownloadType == CouponFeedProcess.JsonDataDownloadType.EnumDownloadType.isUrl ? true : false,
                pUrlOrFilename,
                //      null, // send a null for reading from the file
                //After download of json information from fmtc
                (JArray _obj) =>
                {
                    _count = _obj.Count;
                    StatusMsg(String.Format("Downloaded Fmtc Deals Records {0}. Processing From {1}", _count, _recordsProcessed));
                    return true; // 1 page has all the information
                },
                // process the downloaded information (as jToken/JObject) here
                (JToken ptoken) =>
                {
                    _recordsProcessed++;
                    StatusMsg(String.Format("Fmtc Count {0}. Processed {1} Added {2}", _count, _recordsProcessed, _recordsAdded));
                    if (pProc.FmtcDealsSemaphore)
                    {
                        String _discS = "", _percS = "", _saleS = "";
                        if (ptoken["fDiscount"] != null && ptoken["fDiscount"].ToString() != "")
                            _discS = ptoken["fDiscount"].ToString();
                        if (ptoken["nPercent"] != null && ptoken["nPercent"].ToString() != "")
                            _percS = ptoken["fDiscount"].ToString();
                        if (ptoken["fSalePrice"] != null && ptoken["fSalePrice"].ToString() != "")
                            _percS = ptoken["fDiscount"].ToString();
                        if (_discS == "" && _percS == "" && _saleS == "")
                            return true;
                        Double _disc = 0, _perc = 0, _sale = 0;
                        if (_discS != "")
                            _disc = Double.Parse(_discS);
                        if (_percS != "")
                            _perc = Double.Parse(_percS);
                        if (_saleS != "")
                            _sale = Double.Parse(_saleS);
                        if (_disc == 0 && _perc == 0 && _sale == 0)
                            return true;
                        if (pCmd.Connection.State == ConnectionState.Closed)
                            pCmd.Connection.Open();
                        String _sMerchantId =  ptoken["nMerchantID"].ToString();
                        int _nMerchantID = int.Parse(_sMerchantId);
                        if (temp_nMerchantID != _nMerchantID)
                            Console.WriteLine(String.Format("Requested {0} - downloaded {1} - {2}", temp_nMerchantID, _nMerchantID, ptoken["cMerchant"]));
                        _recordsAdded++;
                        updateFmtcToDb(ptoken, pCmd);
                        return true;
                    }
                    else
                    {
                        _recordsAdded++;
                        return pProc.UpdateFmtcDealsToDb(ptoken, pCmd);
                    }
                }
            );
        }

        private void updateFmtcMessage(String pMsg)
        {
            Invoke((MethodInvoker)delegate
            {
                fmtcDwnMsg.Text = pMsg;
            });
        }

        private bool doProcessFmtcDeals(SqlConnection pConn, CouponFeedProcess pProc)
        {
            //
            ///------------------
            /// process Deals
            ///------------------
            ///
            if (useSourceFileOpt.Checked)
            {
                CouponFeedProcess.JsonDataDownloadType _jsonfromFile = new CouponFeedProcess.JsonDataDownloadType() {fromFileName = sourcefmtcEdt.Text};
                SqlCommand _cmd = null;
                _cmd = CouponFeedProcess.CmdForFmtcDeals();
                _cmd.Connection = pConn;
                doFmtDealsFetch(_jsonfromFile, pProc, _cmd);
                return true;
                //sourcefmtcEdt.Text as Object : _url as Object,
            }
            else
            {
                if (pProc.FmtcDealsSemaphore)
                {
                    if (MessageBox.Show("Fmtc download already in progress. Cancel Download?", "Fmtc Download", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        pProc.RequestFmtcDownloadCancel();
                    }
                    else
                        return false;
                }
                pProc.RaiseFmtcDealsSemaphore();
                int _limitToMerchants = -1; //todo:debug purposes only countdown to number of merchants to download - remove or make it (-1)
                System.Threading.Thread _thread = new Thread(
                    delegate()
                    {
                        using (SqlConnection _conn = new SqlConnection(_connStr))
                        {
                            SqlCommand _cmd = null;
                            _cmd = CouponFeedProcess.CmdForFmtcDeals();
                            _cmd.Connection = _conn;
                            Dictionary<String, Object> _query = null;
                            Uri _url = null;
                            SqlCommand _merchantCmd = new SqlCommand();
                            _merchantCmd.Connection = _conn;
                            _merchantCmd.CommandText = "Select nMerchantID, cName from [dbo].[fmtcMerchants] WHERE updateStatus = 2 ";
                            DataTable _merchants = new DataTable();
                            SqlDataAdapter _merDa = new SqlDataAdapter(_merchantCmd);
                            _merDa.Fill(_merchants);
                            Console.WriteLine("Merchants in list {0}", _merchants.Rows.Count);
                            foreach (DataRow _row in _merchants.Rows)
                            {
                                _query = new Dictionary<string, object> { 
                                    {"merchantids", String.Format("{0}", _row["nMerchantID"])}
                                };
                                temp_nMerchantID = _row.Field<int>("nMerchantID");
                                Console.WriteLine("Processing merchant {0}", temp_nMerchantID);
                                //dc86f28b5368eb84fds38b0291907d03
                                //http://services.fmtc.co/v2/getDeals?key=ae6eaeba6d5fac959544bc24b087950e
                                //_url = BaseWebConn.BuildUriFrom("http://services.fmtc.co/v2/getDeals?key=ae6eaeba6d5fac959544bc24b087950e&dealtype=rebates,sale,offer",_query);
                                _url = new Uri(String.Format(@"http://services.fmtc.co/v2/getDeals?key=ae6eaeba6d5fac959544bc24b087950e&merchantids={0}",
                                    temp_nMerchantID));
                                String _msg = String.Format("Downloading for {0}\nURL:{1}", _row["cName"].ToString(), _url.ToString());
                                updateFmtcMessage(_msg);
                                CouponFeedProcess.JsonDataDownloadType _fromJson = new CouponFeedProcess.JsonDataDownloadType() { fromUri = _url };
                                doFmtDealsFetch(_fromJson, pProc, _cmd);
                                updateFmtcMessage("");
                                if (pProc.FmtcDownloadOpMessage != null && pProc.FmtcDownloadOpMessage != "")
                                {
                                    updateFmtcMessage(pProc.FmtcDownloadOpMessage);
                                    return;
                                }
                                _limitToMerchants--;
                                /*
                                if (_limitToMerchants == 0)
                                {
                                    pProc.ResetFmtcDealsSemaphore();
                                    return;
                                }
                                 * */
                            }
                            pProc.ResetFmtcDealsSemaphore();
                        }
                    });
                _thread.Start();
                return true;
            }
        }

        private void downloadInfoCM_Click(object sender, EventArgs e)
        {
            // test
            //if (!doObj())
             //   return;
            if (_isInProcess)
                return;
            _isInProcess = true;
            try
            {
                using (SqlConnection _conn = new SqlConnection(_connStr))
                {
                    _conn.Open();
                    if (downloadProsperentOP.Checked)
                    {
                        if (merchantOpt.Checked)
                            doProcessMerchants(_conn, _proc);
                        if (productOpt.Checked)
                            doProcessProducts(_conn, _proc);
                    }
                    //-----
                    if (downloadFmtcOP.Checked)
                    {
                        if (fmtcMerchantOpt.Checked)
                            doProcessFmtcMerchants(_conn, _proc);
                        if (fmtcProductOpt.Checked)
                            doProcessFmtcDeals(_conn, _proc);
                    }
                }
            }
            finally
            {
                _isInProcess = false;
            }
        }

        private void getAdminTokenCM_Click(object sender, EventArgs e)
        {
            CouponFeedProcess _proc = new CouponFeedProcess();
            _proc.doRegisterAdmin(firstnameED.Text, lastNameED.Text, passwordED.Text, emailProfileED.Text);
        }

        private void openFileCmd_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                sourcefmtcEdt.Text = openFileDialog1.FileName;
        }

        private void setScheduleCM_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                setScheduleCM.Text = "Stop Schedule";
                timer1.Start();
            }
            else
            {
                setScheduleCM.Text = "Start Schedule";
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_isInProcess)
                return;
            DateTime _now = DateTime.Now;
            // trigger it if it is within the time span plus the timer trigger
            TimeSpan _trigStart = setTimeDownloadEdt.Value.TimeOfDay;
            TimeSpan _trigEnd = _trigStart.Add(TimeSpan.FromMilliseconds(timer1.Interval));
            TimeSpan _nowTime = _now.TimeOfDay;
            if (_nowTime >= _trigStart && _nowTime <= _trigEnd)
            {
                if (_lastTriggerDate < DateTime.Today.Date) // only if not executed today
                    downloadInfoCM_Click(null, null);
            }
        }

        private void syncModeCmd_Click(object sender, EventArgs e)
        {
            downloadProsperentOP.Checked = true;
            merchantOpt.Checked = true;
            productOpt.Checked = true;
            downloadFmtcOP.Checked = true;
            fmtcMerchantOpt.Checked = true;
            fmtcProductOpt.Checked = false;
        }

        private void productOpt_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void asyncModeCmd_Click(object sender, EventArgs e)
        {
            downloadProsperentOP.Checked = false;
            merchantOpt.Checked = false;
            productOpt.Checked = false;
            downloadFmtcOP.Checked = true;
            fmtcMerchantOpt.Checked = false;
            fmtcProductOpt.Checked = true;
        }

    }
}
