using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QponEditor
{
    public class AppUtility
    {
        static public int cNewItems = 1;
        static public int cAcceptedItems = 2;
        static public int cRejectedItems = 3;
        static public int cLaterItems = 4;

        public static SqlParameter setNullable(SqlParameter pParam)
        {
            pParam.IsNullable = true;
            pParam.Value = DBNull.Value;
            return pParam;
        }


    }
}
