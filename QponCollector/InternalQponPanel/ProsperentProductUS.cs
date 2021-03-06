﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InternalQponPanel
{
    public class ProsperentProductUS
    {
        [JsonProperty(PropertyName = "catalogId")]
        public String catalogId { get; set; }
        [JsonProperty(PropertyName = "productId")]
        public String productId { get; set; }
        [JsonProperty(PropertyName = "affiliate_url")]
        public String affiliate_url { get; set; }
        [JsonProperty(PropertyName = "image_url")]
        public String image_url { get; set; }
        [JsonProperty(PropertyName = "keyword")]
        public String keyword { get; set; }
        [JsonProperty(PropertyName = "description")]
        public String description { get; set; }
        [JsonProperty(PropertyName = "category")]
        public String category { get; set; }
        [JsonProperty(PropertyName = "merchant")]
        public String merchant { get; set; }
        [JsonProperty(PropertyName = "brand")]
        public String brand { get; set; }
        [JsonProperty(PropertyName = "upc")]
        public String upc { get; set; }
        [JsonProperty(PropertyName = "isbn")]
        public String isbn { get; set; }
        [JsonProperty(PropertyName = "sales")]
        public String sales { get; set; }
        [JsonProperty(PropertyName = "minPrice")]
        public double minPrice { get; set; }
        [JsonProperty(PropertyName = "maxPrice")]
        public double maxPrice { get; set; }
        [JsonProperty(PropertyName = "minPriceSale")]
        public double minPriceSale { get; set; }
        [JsonProperty(PropertyName = "maxPriceSale")]
        public double maxPriceSale { get; set; }
        [JsonProperty(PropertyName = "groupCount")]
        public double groupCount { get; set; }
    }
}
