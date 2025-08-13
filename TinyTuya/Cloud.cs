using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TinyTuya.Core;

namespace TinyTuya
{
    /// <summary>
    /// Represents a Tuya cloud connection
    /// </summary>
    public class Cloud
    {
        private readonly HttpClient _httpClient;
        private readonly string _configFile = "tinytuya.json";
        private string _apiRegion;
        private string _apiKey;
        private string _apiSecret;
        private string _apiDeviceId;
        private string _urlHost;
        private string _uid;
        private string _token;
        private string _error;
        private bool _newSignAlgorithm;
        private long _serverTimeOffset;
        private bool _useOldDeviceList;
        private Dictionary<string, object> _mappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cloud"/> class
        /// </summary>
        /// <param name="apiRegion">The API region</param>
        /// <param name="apiKey">The API key</param>
        /// <param name="apiSecret">The API secret</param>
        /// <param name="apiDeviceId">The API device ID</param>
        /// <param name="newSignAlgorithm">Whether to use the new sign algorithm</param>
        /// <param name="initialToken">The initial token</param>
        public Cloud(
            string apiRegion = null,
            string apiKey = null,
            string apiSecret = null,
            string apiDeviceId = null,
            bool newSignAlgorithm = true,
            string initialToken = null)
        {
            _httpClient = new HttpClient();
            _apiRegion = apiRegion;
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _apiDeviceId = apiDeviceId;
            _urlHost = "";
            _uid = null;
            _token = initialToken;
            _error = null;
            _newSignAlgorithm = newSignAlgorithm;
            _serverTimeOffset = 0;
            _useOldDeviceList = true;
            _mappings = null;

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
            {
                try
                {
                    // Load defaults from config file if available
                    var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(System.IO.File.ReadAllText(_configFile));
                    _apiRegion = config["apiRegion"].ToString();
                    _apiKey = config["apiKey"].ToString();
                    _apiSecret = config["apiSecret"].ToString();
                    if (config.ContainsKey("apiDeviceID"))
                    {
                        _apiDeviceId = config["apiDeviceID"].ToString();
                    }
                }
                catch
                {
                    _error = JsonConvert.SerializeObject(ErrorHelper.CreateCloudKeyError("Tuya Cloud Key and Secret required"));
                    throw new ArgumentException("Tuya Cloud Key and Secret required");
                }
            }

            SetRegion(apiRegion);

            if (string.IsNullOrEmpty(_token))
            {
                // Attempt to connect to cloud and get token
                GetToken();
            }
        }

        /// <summary>
        /// Sets the API region
        /// </summary>
        /// <param name="apiRegion">The API region</param>
        public void SetRegion(string apiRegion = null)
        {
            if (string.IsNullOrEmpty(apiRegion))
            {
                apiRegion = _apiRegion;
            }
            _apiRegion = apiRegion.ToLower();
            _urlHost = "openapi.tuyacn.com";          // China Data Center
            if (_apiRegion == "us" || _apiRegion == "az")
            {
                _urlHost = "openapi.tuyaus.com";      // Western America Data Center
            }
            else if (_apiRegion == "us-e" || _apiRegion == "ue")
            {
                _urlHost = "openapi-ueaz.tuyaus.com"; // Eastern America Data Center
            }
            else if (_apiRegion == "eu")
            {
                _urlHost = "openapi.tuyaeu.com";      // Central Europe Data Center
            }
            else if (_apiRegion == "eu-w" || _apiRegion == "we")
            {
                _urlHost = "openapi-weaz.tuyaeu.com"; // Western Europe Data Center
            }
            else if (_apiRegion == "in")
            {
                _urlHost = "openapi.tuyain.com";      // India Datacenter
            }
            else if (_apiRegion == "sg")
            {
                _urlHost = "openapi-sg.iotbing.com";  // Singapore Data Center
            }
        }

        /// <summary>
        /// Gets a token from the Tuya cloud
        /// </summary>
        /// <returns>The token</returns>
        private string GetToken()
        {
            var result = TuyaPlatform("token?grant_type=1", "GET");
            if (result != null && result.ContainsKey("result") && result["result"] is Dictionary<string, object> resultDict)
            {
                if (resultDict.ContainsKey("access_token"))
                {
                    _token = resultDict["access_token"].ToString();
                    return _token;
                }
            }
            return null;
        }

        /// <summary>
        /// Makes a request to the Tuya platform
        /// </summary>
        /// <param name="uri">The URI</param>
        /// <param name="action">The HTTP method</param>
        /// <param name="post">The post data</param>
        /// <param name="ver">The API version</param>
        /// <param name="recursive">Whether the call is recursive</param>
        /// <param name="query">The query parameters</param>
        /// <param name="contentType">The content type</param>
        /// <returns>The response</returns>
        private Dictionary<string, object> TuyaPlatform(
            string uri,
            string action = "GET",
            object post = null,
            string ver = "v1.0",
            bool recursive = false,
            object query = null,
            string contentType = null)
        {
            // Build URL and Header
            string url;
            if (!string.IsNullOrEmpty(ver))
            {
                url = $"https://{_urlHost}/{ver}/{uri}";
            }
            else if (uri.StartsWith("/"))
            {
                url = $"https://{_urlHost}{uri}";
            }
            else
            {
                url = $"https://{_urlHost}/{uri}";
            }

            var headers = new Dictionary<string, string>();
            string body = null;
            string signUrl = url;

            if (post != null)
            {
                body = JsonConvert.SerializeObject(post);
            }

            if (action != "GET" && action != "POST" && action != "PUT" && action != "DELETE")
            {
                action = post != null ? "POST" : "GET";
            }

            if (action == "POST" && string.IsNullOrEmpty(contentType))
            {
                contentType = "application/json";
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                headers["Content-type"] = contentType;
            }

            if (query != null)
            {
                // TODO: Handle query parameters
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var signatureHeaders = string.Join(":", headers.Keys);
            if (!string.IsNullOrEmpty(signatureHeaders))
            {
                headers["Signature-Headers"] = signatureHeaders;
            }

            string payload;
            if (string.IsNullOrEmpty(_token))
            {
                payload = _apiKey + now;
                headers["secret"] = _apiSecret;
            }
            else
            {
                payload = _apiKey + _token + now;
            }

            // If running the post 6-30-2021 signing algorithm update the payload to include its data
            if (_newSignAlgorithm)
            {
                payload += $"{action}\n" +
                    $"{(string.IsNullOrEmpty(body) ? "" : ComputeSha256Hash(body))}\n" +
                    string.Join("", headers.Select(h => $"{h.Key}:{h.Value}\n")) +
                    $"/{signUrl.Split("//")[1].Split("/")[1]}";
            }

            // Sign Payload
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret));
            var signature = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).Replace("-", "").ToUpper();
            headers["client_id"] = _apiKey;
            headers["sign"] = signature;
            headers["sign_method"] = "HMAC-SHA256";
            headers["t"] = now.ToString();
            headers["access_token"] = _token;

            // Send Request
            try
            {
                var request = new HttpRequestMessage(new HttpMethod(action), url);
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                if (!string.IsNullOrEmpty(body))
                {
                    request.Content = new StringContent(body, Encoding.UTF8, contentType);
                }

                var response = _httpClient.SendAsync(request).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

                if (responseDict != null && responseDict.ContainsKey("success") && (bool)responseDict["success"])
                {
                    return responseDict;
                }
                else if (responseDict != null && responseDict.ContainsKey("code") && responseDict["code"].ToString() == "1010")
                {
                    if (!recursive)
                    {
                        _token = GetToken();
                        return TuyaPlatform(uri, action, post, ver, true, query, contentType);
                    }
                }

                return responseDict;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "error", ex.Message }
                };
            }
        }

        /// <summary>
        /// Computes the SHA256 hash of a string
        /// </summary>
        /// <param name="rawData">The raw data</param>
        /// <returns>The SHA256 hash</returns>
        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the devices
        /// </summary>
        /// <param name="verbose">Whether to include verbose information</param>
        /// <returns>The devices</returns>
        public Dictionary<string, object> GetDevices(bool verbose = false)
        {
            if (string.IsNullOrEmpty(_uid))
            {
                if (string.IsNullOrEmpty(_apiDeviceId))
                {
                    return new Dictionary<string, object>
                    {
                        { "error", "No Device ID or User ID available to get device list" }
                    };
                }

                var result = TuyaPlatform($"devices/{_apiDeviceId}", "GET");
                if (result != null && result.ContainsKey("result") && result["result"] is Dictionary<string, object> resultDict && resultDict.ContainsKey("uid"))
                {
                    _uid = resultDict["uid"].ToString();
                }
                else
                {
                    return new Dictionary<string, object>
                    {
                        { "error", "Unable to get User ID" }
                    };
                }
            }

            var devices = TuyaPlatform($"users/{_uid}/devices", "GET");
            return devices;
        }

        /// <summary>
        /// Gets the status of a device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <returns>The status</returns>
        public Dictionary<string, object> GetStatus(string deviceId)
        {
            var result = TuyaPlatform($"devices/{deviceId}", "GET");
            return result;
        }

        /// <summary>
        /// Gets the functions of a device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <returns>The functions</returns>
        public Dictionary<string, object> GetFunctions(string deviceId)
        {
            var result = TuyaPlatform($"devices/{deviceId}/functions", "GET");
            return result;
        }

        /// <summary>
        /// Gets the properties of a device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <returns>The properties</returns>
        public Dictionary<string, object> GetProperties(string deviceId)
        {
            var result = TuyaPlatform($"devices/{deviceId}/properties", "GET");
            return result;
        }

        /// <summary>
        /// Gets the data points of a device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <returns>The data points</returns>
        public Dictionary<string, object> GetDps(string deviceId)
        {
            var result = TuyaPlatform($"devices/{deviceId}/status", "GET");
            return result;
        }

        /// <summary>
        /// Sends a command to a device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <param name="commands">The commands</param>
        /// <param name="uri">The URI</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SendCommand(string deviceId, object commands, string uri = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                uri = $"devices/{deviceId}/commands";
            }
            var result = TuyaPlatform(uri, "POST", commands);
            return result;
        }

        /// <summary>
        /// Gets the connection status of a device
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <returns>The connection status</returns>
        public Dictionary<string, object> GetConnectStatus(string deviceId)
        {
            var result = TuyaPlatform($"devices/{deviceId}/status", "GET");
            return result;
        }

        /// <summary>
        /// Gets the device log
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <param name="evType">The event type</param>
        /// <param name="size">The size</param>
        /// <param name="params">The parameters</param>
        /// <returns>The device log</returns>
        public Dictionary<string, object> GetDeviceLog(
            string deviceId,
            DateTime? start = null,
            DateTime? end = null,
            string evType = "1,2,3,4,5,6,7,8,9,10",
            int size = 100,
            Dictionary<string, object> @params = null)
        {
            if (start == null)
            {
                start = DateTime.UtcNow.AddDays(-1);
            }
            else if (start < DateTime.UtcNow.AddDays(-7))
            {
                start = DateTime.UtcNow.AddDays(-7);
            }

            if (end == null)
            {
                end = DateTime.UtcNow;
            }

            var startTime = ((DateTimeOffset)start).ToUnixTimeMilliseconds();
            var endTime = ((DateTimeOffset)end).ToUnixTimeMilliseconds();

            var query = new Dictionary<string, object>
            {
                { "device_id", deviceId },
                { "start_time", startTime },
                { "end_time", endTime },
                { "type", evType },
                { "size", size }
            };

            if (@params != null)
            {
                foreach (var param in @params)
                {
                    query[param.Key] = param.Value;
                }
            }

            var result = TuyaPlatform("devices/logs", "GET", null, "v1.0", false, query);
            return result;
        }
    }
}

