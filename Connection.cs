using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace DesigoClimatixApi
{
    public class Connection 
    {
        private readonly string _baseUrl;
        private readonly string _pin;
        private readonly string _authHeaderValue;
        private readonly bool _dev = false;
        private readonly HttpClient _client ;

        public string GetBaseUrl()
        {
            return _baseUrl;
        }
        public string GetAuthHeaderValue()
        {
            return _authHeaderValue;
        }
        /// <summary>
        /// Create a connection to a Controler
        /// </summary>
        /// <param name="username">Username used to connect to  controler</param>
        /// <param name="password">Password used to connect to controler</param>
        /// <param name="ip">IP addres of Controller you must include http:// or https://</param>
        /// <param name="pin">Pin used for controller</param>
        public Connection(string username, string password, string ip, string pin)
        {
            _baseUrl = $"{ip}/JSONGEN.HTML?FN=";
            _pin = pin;
            var authString = $"{username}:{password}";
            _authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            _dev = false;
            _client = new() { Timeout = TimeSpan.FromSeconds(15) };
        }
        public Connection(string username, string password, string ip, string pin, bool dev)
        {
            _baseUrl = $"{ip}/JSONGEN.HTML?FN=";
            _pin = pin;
            var authString = $"{username}:{password}";
            _authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            _dev = dev;
            _client = new() { Timeout = TimeSpan.FromSeconds(15) };
        }
        internal Connection(string username, string password, string ip, string pin,HttpClient client,bool dev = false)
        {
            _baseUrl = $"{ip}/JSONGEN.HTML?FN=";
            _pin = pin;
            var authString = $"{username}:{password}";
            _authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            _dev = dev;
            _client = client;
        }
        /// <summary>
        /// Function used to read a point
        /// </summary>
        /// <param name="base64Id">A base64 Id of a point you want to read</param>
        /// <returns>Return a value or an error if something went wrong</returns>
        public object ReadValue(string base64Id)
        { 
            return  SendRequest(BuildReadUrl(base64Id)).ToString(_dev,base64Id,BuildReadUrl(base64Id));
        }
        /// <summary>
        /// Function used to write a value to a point
        /// </summary>
        /// <param name="base64Id">A base64 Id of a point you want to read</param>
        /// <param name="value">Value that will be writen to a point</param>
        /// <returns>Return a value or an error if something went wrong</returns>
        public object WriteValue(string base64Id, string value)
        {
            return SendRequest(BuildWriteUrl(base64Id,value)).ToString(_dev,base64Id,BuildWriteUrl(base64Id,value));
        }
        /// <summary>
        /// Used to send a request to a endpoint woth auth
        /// </summary>
        internal ApiResponse SendRequest(string url)
        {
            var result = new ApiResponse();
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _authHeaderValue);
                    
                    using (var response = _client.SendAsync(request).GetAwaiter().GetResult())
                    {
                        result.StatusCode = (int)response.StatusCode;
                        result.IsSuccess = response.IsSuccessStatusCode;
                        result.Content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ErrorMessage = e.Message;
                return result;
            }   
        }

        internal string BuildReadUrl(string base64Id)
        {
            return $"{_baseUrl}Read&OA={base64Id}&PIN={_pin}";
        }
        internal string BuildWriteUrl(string base64Id , string value)
        {
            return $"{_baseUrl}Write&OA={base64Id};{value}&PIN={_pin}";
        }
    }   

   public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Content { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public object ToString(bool devMode,string base64Id,string  apiCall)
        {
            if (devMode)
            {
                if (string.IsNullOrEmpty(this.ErrorMessage))
                {
                return new 
                    {
                        IsSuccess = this.IsSuccess,
                        StatusCode = this.StatusCode,
                        Content = this.Content,
                        ErrorMessage = this.ErrorMessage,
                        PointId = base64Id,
                        APICaall = apiCall 
                    };              
                }
                else
                {
                  return new 
                    {
                        IsSuccess = this.IsSuccess,
                        StatusCode = this.StatusCode,
                        Content = this.Content,
                        ErrorMessage = this.ErrorMessage,
                        PointId = base64Id,
                        APICaall = apiCall 
                    };              
                }
                }
            else
            {
                if (string.IsNullOrEmpty(this.ErrorMessage))
                {
                    var obj = JObject.Parse(this.Content);
                    var value = obj["values"][base64Id][0];
                    return value;
                }
                else
                {
                    return new
                    {
                        Error = this.ErrorMessage
                    };
                }
            }
        }
    }

}