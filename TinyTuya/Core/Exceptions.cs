using System;

namespace TinyTuya.Core
{
    /// <summary>
    /// Base exception for all TinyTuya exceptions
    /// </summary>
    public class TinyTuyaException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TinyTuyaException"/> class
        /// </summary>
        public TinyTuyaException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyTuyaException"/> class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public TinyTuyaException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyTuyaException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public TinyTuyaException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when there is an error decoding data
    /// </summary>
    public class DecodeException : TinyTuyaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecodeException"/> class
        /// </summary>
        public DecodeException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecodeException"/> class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public DecodeException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecodeException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public DecodeException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when a device is offline
    /// </summary>
    public class DeviceOfflineException : TinyTuyaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceOfflineException"/> class
        /// </summary>
        public DeviceOfflineException() : base("Device is offline") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceOfflineException"/> class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public DeviceOfflineException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when there is an error connecting to a device
    /// </summary>
    public class ConnectionException : TinyTuyaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class
        /// </summary>
        public ConnectionException() : base("Error connecting to device") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ConnectionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public ConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}

