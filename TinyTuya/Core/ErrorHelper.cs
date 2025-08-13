using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TinyTuya.Core
{
    /// <summary>
    /// Helper class for error handling
    /// </summary>
    public static class ErrorHelper
    {
        /// <summary>
        /// Creates a JSON error object
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreateErrorJson(string errorCode, string errorMessage)
        {
            return new Dictionary<string, object>
            {
                { "Error", errorCode },
                { "Msg", errorMessage }
            };
        }

        /// <summary>
        /// Creates a JSON error object for connection errors
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreateConnectionError(string message = "Connection error")
        {
            return CreateErrorJson(ErrorCodes.ERR_CONNECT, message);
        }

        /// <summary>
        /// Creates a JSON error object for offline devices
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreateOfflineError(string message = "Device is offline")
        {
            return CreateErrorJson(ErrorCodes.ERR_OFFLINE, message);
        }

        /// <summary>
        /// Creates a JSON error object for JSON parsing errors
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreateJsonError(string message = "JSON error")
        {
            return CreateErrorJson(ErrorCodes.ERR_JSON, message);
        }

        /// <summary>
        /// Creates a JSON error object for key or version errors
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreateKeyOrVersionError(string message = "Key or version error")
        {
            return CreateErrorJson(ErrorCodes.ERR_KEY_OR_VER, message);
        }

        /// <summary>
        /// Creates a JSON error object for payload errors
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreatePayloadError(string message = "Payload error")
        {
            return CreateErrorJson(ErrorCodes.ERR_PAYLOAD, message);
        }

        /// <summary>
        /// Creates a JSON error object for device type errors
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreateDeviceTypeError(string message = "Device type error")
        {
            return CreateErrorJson(ErrorCodes.ERR_DEVTYPE, message);
        }

        /// <summary>
        /// Creates a JSON error object for cloud key errors
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A dictionary representing the error</returns>
        public static Dictionary<string, object> CreateCloudKeyError(string message = "Cloud key error")
        {
            return CreateErrorJson(ErrorCodes.ERR_CLOUDKEY, message);
        }

        /// <summary>
        /// Checks if a dictionary contains an error
        /// </summary>
        /// <param name="data">The dictionary to check</param>
        /// <returns>True if the dictionary contains an error, false otherwise</returns>
        public static bool HasError(Dictionary<string, object> data)
        {
            return data != null && (data.ContainsKey("Error") || data.ContainsKey("Err"));
        }

        /// <summary>
        /// Gets the error message from a dictionary
        /// </summary>
        /// <param name="data">The dictionary to get the error message from</param>
        /// <returns>The error message</returns>
        public static string GetErrorMessage(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return "Unknown error";
            }

            if (data.ContainsKey("Error") && data.ContainsKey("Msg"))
            {
                return $"{data["Error"]}: {data["Msg"]}";
            }
            else if (data.ContainsKey("Err"))
            {
                return data["Err"].ToString();
            }
            else
            {
                return "Unknown error";
            }
        }
    }
}

