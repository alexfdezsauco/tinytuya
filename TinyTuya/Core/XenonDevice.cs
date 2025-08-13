using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TinyTuya.Core
{
    /// <summary>
    /// Base class for Tuya devices
    /// </summary>
    public class XenonDevice : IDisposable
    {
        private readonly object _socketLock = new object();
        private TcpClient _socket;
        private NetworkStream _stream;
        private CryptoHelper _cipher;
        private readonly Dictionary<string, Dictionary<string, object>> _payloadDict;

        /// <summary>
        /// Gets or sets the device ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the child ID (for sub-devices)
        /// </summary>
        public string ChildId { get; set; }

        /// <summary>
        /// Gets or sets the device address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the IP address is automatically determined
        /// </summary>
        public bool AutoIp { get; set; }

        /// <summary>
        /// Gets or sets the device type
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the device type is automatically determined
        /// </summary>
        public bool DeviceTypeAuto { get; set; }

        /// <summary>
        /// Gets or sets the last device type
        /// </summary>
        public string LastDeviceType { get; set; }

        /// <summary>
        /// Gets or sets the connection timeout
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to retry failed operations
        /// </summary>
        public bool Retry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable device type detection
        /// </summary>
        public bool DisableDetect { get; set; }

        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to keep the socket connection persistent
        /// </summary>
        public bool SocketPersistent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to set TCP_NODELAY on the socket
        /// </summary>
        public bool SocketNoDelay { get; set; }

        /// <summary>
        /// Gets or sets the socket retry limit
        /// </summary>
        public int SocketRetryLimit { get; set; }

        /// <summary>
        /// Gets or sets the socket retry delay
        /// </summary>
        public int SocketRetryDelay { get; set; }

        /// <summary>
        /// Gets or sets the protocol version
        /// </summary>
        public double Version { get; set; }

        /// <summary>
        /// Gets or sets the protocol version as a string
        /// </summary>
        public string VersionString { get; set; }

        /// <summary>
        /// Gets or sets the protocol version as bytes
        /// </summary>
        public byte[] VersionBytes { get; set; }

        /// <summary>
        /// Gets or sets the protocol version header
        /// </summary>
        public byte[] VersionHeader { get; set; }

        /// <summary>
        /// Gets or sets the data points to request
        /// </summary>
        public Dictionary<string, object> DpsToRequest { get; set; }

        /// <summary>
        /// Gets or sets the sequence number
        /// </summary>
        public uint SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the send wait time
        /// </summary>
        public int SendWait { get; set; }

        /// <summary>
        /// Gets or sets the data points cache
        /// </summary>
        public Dictionary<string, object> DpsCache { get; set; }

        /// <summary>
        /// Gets or sets the parent device
        /// </summary>
        public XenonDevice Parent { get; set; }

        /// <summary>
        /// Gets or sets the child devices
        /// </summary>
        public Dictionary<string, XenonDevice> Children { get; set; }

        /// <summary>
        /// Gets or sets the local key
        /// </summary>
        public byte[] LocalKey { get; set; }

        /// <summary>
        /// Gets or sets the real local key
        /// </summary>
        public byte[] RealLocalKey { get; set; }

        /// <summary>
        /// Gets or sets the local nonce
        /// </summary>
        public byte[] LocalNonce { get; set; }

        /// <summary>
        /// Gets or sets the remote nonce
        /// </summary>
        public byte[] RemoteNonce { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of simultaneous data points
        /// </summary>
        public int MaxSimultaneousDps { get; set; }

        /// <summary>
        /// Gets or sets the raw sent data
        /// </summary>
        public byte[] RawSent { get; set; }

        /// <summary>
        /// Gets or sets the raw received data
        /// </summary>
        public List<byte[]> RawReceived { get; set; }

        /// <summary>
        /// Gets or sets the command return code
        /// </summary>
        public int? CommandReturnCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XenonDevice"/> class
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <param name="address">The device address</param>
        /// <param name="localKey">The local key</param>
        /// <param name="deviceType">The device type</param>
        /// <param name="connectionTimeout">The connection timeout</param>
        /// <param name="version">The protocol version</param>
        /// <param name="persist">Whether to keep the socket connection persistent</param>
        /// <param name="childId">The child ID (for sub-devices)</param>
        /// <param name="parent">The parent device</param>
        /// <param name="connectionRetryLimit">The connection retry limit</param>
        /// <param name="connectionRetryDelay">The connection retry delay</param>
        /// <param name="port">The port</param>
        /// <param name="maxSimultaneousDps">The maximum number of simultaneous data points</param>
        public XenonDevice(
            string deviceId,
            string address = null,
            string localKey = "",
            string deviceType = "default",
            int connectionTimeout = 5,
            double version = 3.1,
            bool persist = false,
            string childId = null,
            XenonDevice parent = null,
            int connectionRetryLimit = 5,
            int connectionRetryDelay = 5,
            int port = Constants.TCPPORT,
            int maxSimultaneousDps = 0)
        {
            Id = deviceId;
            ChildId = childId;
            Address = address;
            AutoIp = false;
            DeviceType = deviceType;
            DeviceTypeAuto = deviceType == "default";
            LastDeviceType = "";
            ConnectionTimeout = connectionTimeout;
            Retry = true;
            DisableDetect = false;
            Port = port;
            SocketPersistent = persist;
            SocketNoDelay = true;
            SocketRetryLimit = connectionRetryLimit;
            SocketRetryDelay = connectionRetryDelay;
            Version = 0;
            VersionString = null;
            VersionBytes = null;
            VersionHeader = null;
            DpsToRequest = new Dictionary<string, object>();
            SequenceNumber = 1;
            SendWait = Constants.DEFAULT_SEND_WAIT;
            DpsCache = new Dictionary<string, object>();
            Parent = parent;
            Children = new Dictionary<string, XenonDevice>();
            LocalNonce = Encoding.ASCII.GetBytes("0123456789abcdef");
            RemoteNonce = new byte[0];
            _payloadDict = null;
            MaxSimultaneousDps = maxSimultaneousDps;
            RawReceived = new List<byte[]>();

            if (string.IsNullOrEmpty(localKey))
            {
                localKey = "";
                if (parent == null)
                {
                    // TODO: Load from device file
                }
            }

            LocalKey = Encoding.Latin1.GetBytes(localKey);
            RealLocalKey = LocalKey;
            _cipher = null;

            if (parent != null)
            {
                if (string.IsNullOrEmpty(ChildId))
                {
                    // TODO: Load from device file
                }

                SetVersion(parent.Version);
                parent.RegisterChild(this);
            }
            else if (string.IsNullOrEmpty(address) || address == "Auto" || address == "0.0.0.0")
            {
                // TODO: Auto-detect IP address
            }
            else if (version > 0)
            {
                SetVersion(version);
            }
            else
            {
                SetVersion(3.1);
            }
        }

        /// <summary>
        /// Registers a child device
        /// </summary>
        /// <param name="child">The child device</param>
        public void RegisterChild(XenonDevice child)
        {
            if (child.ChildId != null)
            {
                Children[child.ChildId] = child;
            }
            else
            {
                Children[child.Id] = child;
            }
        }

        /// <summary>
        /// Sets the protocol version
        /// </summary>
        /// <param name="version">The protocol version</param>
        public virtual void SetVersion(double version)
        {
            Version = version;
            VersionString = "v" + version.ToString("0.0");
            VersionBytes = Encoding.ASCII.GetBytes(version.ToString("0.0"));
            VersionHeader = Header.GetFullHeader(version);
            _payloadDict = null;
        }

        /// <summary>
        /// Sets whether to keep the socket connection persistent
        /// </summary>
        /// <param name="persistent">Whether to keep the socket connection persistent</param>
        public void SetSocketPersistent(bool persistent)
        {
            SocketPersistent = persistent;
        }

        /// <summary>
        /// Sets whether to set TCP_NODELAY on the socket
        /// </summary>
        /// <param name="noDelay">Whether to set TCP_NODELAY on the socket</param>
        public void SetSocketNoDelay(bool noDelay)
        {
            SocketNoDelay = noDelay;
        }

        /// <summary>
        /// Sets the socket retry limit
        /// </summary>
        /// <param name="retryLimit">The socket retry limit</param>
        public void SetSocketRetryLimit(int retryLimit)
        {
            SocketRetryLimit = retryLimit;
        }

        /// <summary>
        /// Sets the socket timeout
        /// </summary>
        /// <param name="timeout">The socket timeout</param>
        public void SetSocketTimeout(int timeout)
        {
            ConnectionTimeout = timeout;
        }

        /// <summary>
        /// Sets the data points to request
        /// </summary>
        /// <param name="dpsToRequest">The data points to request</param>
        public void SetDpsUsed(Dictionary<string, object> dpsToRequest)
        {
            DpsToRequest = dpsToRequest;
        }

        /// <summary>
        /// Adds a data point to request
        /// </summary>
        /// <param name="index">The data point index</param>
        public void AddDpsToRequest(string index)
        {
            DpsToRequest[index] = null;
        }

        /// <summary>
        /// Sets whether to retry failed operations
        /// </summary>
        /// <param name="retry">Whether to retry failed operations</param>
        public void SetRetry(bool retry)
        {
            Retry = retry;
        }

        /// <summary>
        /// Sets the send wait time
        /// </summary>
        /// <param name="sendWait">The send wait time</param>
        public void SetSendWait(int sendWait)
        {
            SendWait = sendWait;
        }

        /// <summary>
        /// Gets the socket
        /// </summary>
        /// <param name="renew">Whether to renew the socket</param>
        /// <returns>True if the socket is ready, false otherwise</returns>
        protected bool GetSocket(bool renew)
        {
            lock (_socketLock)
            {
                if (renew && _socket != null)
                {
                    _socket.Close();
                    _socket = null;
                    _stream = null;
                }

                if (_socket == null)
                {
                    int retries = 0;
                    string err = ErrorCodes.ERR_OFFLINE;

                    while (retries < SocketRetryLimit)
                    {
                        if (AutoIp && string.IsNullOrEmpty(Address))
                        {
                            // TODO: Auto-detect IP address
                            return false;
                        }

                        if (string.IsNullOrEmpty(Address))
                        {
                            return false;
                        }

                        if ((Version > 3.1) && ((LocalKey == null) || (LocalKey.Length != 16)))
                        {
                            return false;
                        }

                        _socket = new TcpClient();
                        _socket.NoDelay = SocketNoDelay;

                        try
                        {
                            retries++;
                            var connectTask = _socket.ConnectAsync(Address, Port);
                            if (!connectTask.Wait(ConnectionTimeout))
                            {
                                _socket.Close();
                                _socket = null;
                                err = ErrorCodes.ERR_OFFLINE;
                                continue;
                            }

                            _stream = _socket.GetStream();
                            _stream.ReadTimeout = ConnectionTimeout;
                            _stream.WriteTimeout = ConnectionTimeout;

                            if (Version >= 3.4)
                            {
                                if (NegotiateSessionKey())
                                {
                                    return true;
                                }
                                else
                                {
                                    if (_socket != null)
                                    {
                                        _socket.Close();
                                        _socket = null;
                                        _stream = null;
                                    }
                                    return false;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        catch (Exception)
                        {
                            if (_socket != null)
                            {
                                _socket.Close();
                                _socket = null;
                                _stream = null;
                            }
                            err = ErrorCodes.ERR_CONNECT;
                        }

                        if (retries < SocketRetryLimit)
                        {
                            Thread.Sleep(SocketRetryDelay);
                        }

                        if (AutoIp)
                        {
                            // TODO: Auto-detect IP address
                        }
                    }

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Negotiates a session key for protocol version 3.4 and above
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        protected bool NegotiateSessionKey()
        {
            // TODO: Implement session key negotiation
            return false;
        }

        /// <summary>
        /// Closes the socket
        /// </summary>
        public void Close()
        {
            lock (_socketLock)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                    _stream = null;
                }
            }
        }

        /// <summary>
        /// Disposes the device
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Generates a payload for a command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="data">The data</param>
        /// <returns>The payload</returns>
        public byte[] GeneratePayload(int command, object data = null)
        {
            // TODO: Implement payload generation
            return null;
        }

        /// <summary>
        /// Sends a payload to the device
        /// </summary>
        /// <param name="payload">The payload</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Send(byte[] payload)
        {
            // TODO: Implement send
            return false;
        }

        /// <summary>
        /// Receives a response from the device
        /// </summary>
        /// <returns>The response</returns>
        public Dictionary<string, object> Receive()
        {
            // TODO: Implement receive
            return null;
        }

        /// <summary>
        /// Sends a payload to the device and receives a response
        /// </summary>
        /// <param name="payload">The payload</param>
        /// <param name="retries">The number of retries</param>
        /// <param name="getResponse">Whether to get a response</param>
        /// <returns>The response</returns>
        protected Dictionary<string, object> SendReceive(byte[] payload, int retries = 1, bool getResponse = true)
        {
            // TODO: Implement send/receive
            return null;
        }

        /// <summary>
        /// Gets the status of the device
        /// </summary>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The status</returns>
        public virtual Dictionary<string, object> Status(bool nowait = false)
        {
            // TODO: Implement status
            return null;
        }
    }
}

