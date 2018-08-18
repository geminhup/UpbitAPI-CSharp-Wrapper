using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace UpbitAPI_CS_Wrapper
{
    public class UpbitAPI
    {
        public MyHttpClient httpClient;
        public UpbitAPI(string accessKey, string secretKey)
        {
            this.httpClient = new MyHttpClient(accessKey, secretKey);
        }

        public class MyHttpClient : HttpClient
        {
            public string accessKey;
            public string secretKey;

            public MyHttpClient(string accessKey, string secretKey)
            {
                this.accessKey = accessKey;
                this.secretKey = secretKey;
            }
        }

        public string GetAccount()
        {
            string url = "https://api.upbit.com/v1/accounts";
            return CallAPI_NoParam(url, HttpMethod.Get).Result;
        }
        public string GetTicker(string markets)
        {
            string url = "https://api.upbit.com/v1/ticker";
            return CallAPI_WithParam(url, new NameValueCollection { { "markets", markets} }, HttpMethod.Get).Result;
        }
        public string GetMarkets()
        {
            string url = "https://api.upbit.com/v1/market/all";
            return CallAPI_NoParam(url, HttpMethod.Get).Result;
        }
        public string GetAllOrder()
        {
            string url = "https://api.upbit.com/v1/orders";
            return CallAPI_NoParam(url, HttpMethod.Get).Result;
        }
        public string GetOrder(string uuid)
        {
            string url = "https://api.upbit.com/v1/order";
            return CallAPI_WithParam(url, new NameValueCollection { { "uuid", uuid } }, HttpMethod.Get).Result;
        }
        public string MakeOrder(string market, UpbitOrderSide side, decimal volume, decimal price, UpbitOrderType ord_type = UpbitOrderType.limit)
        {
            string url = "https://api.upbit.com/v1/orders";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "side", side.ToString()}, { "volume", volume.ToString() }, { "price", price.ToString() }, { "ord_type", ord_type.ToString() } }, HttpMethod.Post).Result;
        }
        public string CancelOrder(string uuid)
        {
            string url = "https://api.upbit.com/v1/order";
            return CallAPI_WithParam(url, new NameValueCollection { { "uuid", uuid} }, HttpMethod.Delete).Result;
        }
        public string GetOrderbook(string markets)
        {
            string url = "https://api.upbit.com/v1/orderbook";
            return CallAPI_WithParam(url, new NameValueCollection { { "markets", markets } }, HttpMethod.Get).Result;
        }
        public string GetOrderChance(string market)
        {
            string url = "https://api.upbit.com/v1/orders/chance";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market } }, HttpMethod.Get).Result;
        }
        public string GetCandles_Minute(string market, UpbitMinuteCandleType unit, DateTime to = default(DateTime), int count = 1)
        {
            string url = "https://api.upbit.com/v1/candles/minutes/" + (int)unit;
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get).Result;
        }
        public string GetCandles_Day(string market, DateTime to = default(DateTime), int count = 1)
        {
            string url = "https://api.upbit.com/v1/candles/days";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get).Result;
        }
        public string GetCandles_Week(string market, DateTime to = default(DateTime), int count = 1)
        {
            string url = "https://api.upbit.com/v1/candles/weeks";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get).Result;
        }
        public string GetCandles_Month(string market, DateTime to = default(DateTime), int count = 1)
        {
            string url = "https://api.upbit.com/v1/candles/months";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market }, { "to", (to == default(DateTime)) ? DateTime2String(DateTime.Now) : DateTime2String(to) }, { "count", count.ToString() } }, HttpMethod.Get).Result;
        }
        public string GetTicks(string market)
        {
            string url = "https://api.upbit.com/v1/trades/ticks";
            return CallAPI_WithParam(url, new NameValueCollection { { "market", market } }, HttpMethod.Get).Result;
        }

        async public Task<string> CallAPI_NoParam(string url, HttpMethod httpMethod)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, new Uri(url));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT_NoParameter());
            var response = await httpClient.SendAsync(requestMessage);
            var contents = await response.Content.ReadAsStringAsync();
            return contents;
        }
        async public Task<string> CallAPI_WithParam(string url, NameValueCollection nvc, HttpMethod httpMethod)
        {
            Dictionary<string, string> dic = Nvc2Dictionary(nvc);
            var requestMessage = new HttpRequestMessage(httpMethod, new Uri(url + ToQueryString(nvc)));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWT_WithParameter(dic));
            var response = await httpClient.SendAsync(requestMessage);
            var contents = await response.Content.ReadAsStringAsync();
            return contents;
        }

        string JWT_NoParameter()
        {
            TimeSpan diff = DateTime.Now - new DateTime(1970, 1, 1);
            var nonce = Convert.ToInt64(diff.TotalMilliseconds);
            var payload = new JwtPayload { { "access_key", httpClient.accessKey }, { "nonce", nonce } };
            byte[] keyBytes = Encoding.Default.GetBytes(httpClient.secretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            var authorizationToken = jwtToken;
            return authorizationToken;
        }
        string JWT_WithParameter(Dictionary<string, string> parameters)
        {
            TimeSpan diff = DateTime.Now - new DateTime(1970, 1, 1);
            var nonce = Convert.ToInt64(diff.TotalMilliseconds);

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
            }
            string queryString = builder.ToString().TrimEnd('&');

            var payload = new JwtPayload { { "access_key", httpClient.accessKey }, { "nonce", nonce }, { "query", queryString } };
            byte[] keyBytes = Encoding.Default.GetBytes(httpClient.secretKey);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            var authorizationToken = jwtToken;
            return authorizationToken;
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }
        static Dictionary<string, string> Nvc2Dictionary(NameValueCollection nvc)
        {
            Dictionary<string, string> dic = new Dictionary<string, string> { };
            foreach (string key in nvc)
            {
                dic.Add(key, nvc[key]);
            }
            return dic;
        }
        private string DateTime2String(DateTime to)
        {
            return to.ToString("s") + "+09:00";
        }
        public enum UpbitMinuteCandleType { _1 = 1, _3 = 3, _5 = 5, _10 = 10, _15 = 15, _30 = 30, _60 = 60, _240 = 240 }
        public enum UpbitOrderSide { ask, bid }
        public enum UpbitOrderType { limit }

    }
}
