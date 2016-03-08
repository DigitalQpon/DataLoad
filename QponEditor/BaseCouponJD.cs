using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QponEditor
{
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


    public class QponData
    {
        public QponData(BaseCouponJD pCouponProcessor)
        {
            coupon = pCouponProcessor;
        }
        BaseCouponJD coupon;
    }

    public class QponBaseCoupon
    {
        public QponBaseCoupon(BaseCouponJD pCouponProcessor)
        {
            data = new QponData(pCouponProcessor);
        }
        public QponData data;
    }

    public class RegisterVendorJS
    {
        public string Name;
        public string Password;
        public string Email;
        public string WebsiteUrl;
        public string StreetLine1;
        public string StreetLine2;
        public string City;
        public string Province;
        public string Country;
        public string ZipCode;
        public string Size;
        public string VendorId;
        public String LogoImageURL;
        //public string AccessToken;
    }

    public class BaseCouponJS
    {
        public String BaseCouponId;
        public String BarCode;
        public String Title;
        public String PurchaseUrl;
        public String ImageURL;
        public String Description;
        public String Terms;
        public String VideoUrl;
        public String VendorImage;
        public int CommentCount;
        public int QrushCount;
        public int BagCount;
        public int FavoriteCount;
        public String CouponType;
        public String CategoryType;
        public String DiscountType;
        public String ShareType;
        public String ExpiryType;
        public String Image;
        public String MaximumIssue;
        public String BaseValue;
        public String MaxValue;
        public String ShareThreshold;
        public String ShareIncrease;
        public String ExpiryDate;
        public String AccessToken;
    }
    
    public class CategoryTypeJD
    {
        public int CouponCategoryId;
        public String CategoryName;
        public String ImageUrl;
    }

    public class BaseCouponJD
    {
        /*
        public class CouponType {
            public int CouponTypeId;
            public String Type;
        }
         * */
        /*
         * */
        public class clDiscountType {
            public int DiscountTypeId;
            public String Type;
        }
        public class clShareType {
            public int ShareTypeId;
            public String Type;
        }
        public class clExpiryType {
            public int ExpiryTypeId;
            public String Type;
        }
        public class clImage {
            public String ImageId;
            //public String Image;
            public int Width;
            public int Height;
            public String Format;
            public String Title;
        }
        public String VendorId;
        //public String BaseCouponId;
        public String BarCode;
        public String Title;
        public String PurchaseUrl;
        public String Description;
        public String Terms;
        //public CouponType CouponType;
        public String CouponType;
        //public CategoryType CategoryType;
        public String CategoryType;
        //public clDiscountType DiscountType;
        public String DiscountType;
        //public clShareType ShareType;
        public String ShareType;
        //public clExpiryType ExpiryType;
        public String ExpiryType;
        public int MaximumIssue;
        public int BaseValue;
        public int MaxValue;
        public int ShareThreshold;
        public int ShareIncrease;
        public String VideoUrl;
        public String ProductImageUrl;
        //public DateTime ExpiryDate;
        public String expiryDate;
        public String AccessToken;
        public String logoImageUrl;
    }
}
