using System;
using System.Text;

namespace TinyTuya.Core
{
    /// <summary>
    /// Header constants and utilities for Tuya protocol
    /// </summary>
    public static class Header
    {
        /// <summary>
        /// Protocol 3.1 header
        /// </summary>
        public static readonly byte[] PROTOCOL_31_HEADER = Encoding.ASCII.GetBytes("3.1");

        /// <summary>
        /// Protocol 3.3 header
        /// </summary>
        public static readonly byte[] PROTOCOL_33_HEADER = Encoding.ASCII.GetBytes("3.3");

        /// <summary>
        /// Protocol 3.x header
        /// </summary>
        public static readonly byte[] PROTOCOL_3X_HEADER = Encoding.ASCII.GetBytes("");

        /// <summary>
        /// Protocol 3.4 header
        /// </summary>
        public static readonly byte[] PROTOCOL_34_HEADER = Encoding.ASCII.GetBytes("3.4");

        /// <summary>
        /// Protocol 3.5 header
        /// </summary>
        public static readonly byte[] PROTOCOL_35_HEADER = Encoding.ASCII.GetBytes("3.5");

        /// <summary>
        /// Gets the protocol header for a specific version
        /// </summary>
        /// <param name="version">The protocol version</param>
        /// <returns>The protocol header</returns>
        public static byte[] GetProtocolHeader(double version)
        {
            if (version == 3.1)
            {
                return PROTOCOL_31_HEADER;
            }
            else if (version == 3.3)
            {
                return PROTOCOL_33_HEADER;
            }
            else if (version == 3.4)
            {
                return PROTOCOL_34_HEADER;
            }
            else if (version == 3.5)
            {
                return PROTOCOL_35_HEADER;
            }
            else
            {
                return PROTOCOL_3X_HEADER;
            }
        }

        /// <summary>
        /// Gets the protocol version bytes for a specific version
        /// </summary>
        /// <param name="version">The protocol version</param>
        /// <returns>The protocol version bytes</returns>
        public static byte[] GetVersionBytes(double version)
        {
            return Encoding.ASCII.GetBytes(version.ToString("0.0"));
        }

        /// <summary>
        /// Gets the full protocol header (version + header) for a specific version
        /// </summary>
        /// <param name="version">The protocol version</param>
        /// <returns>The full protocol header</returns>
        public static byte[] GetFullHeader(double version)
        {
            var versionBytes = GetVersionBytes(version);
            var headerBytes = GetProtocolHeader(version);
            var fullHeader = new byte[versionBytes.Length + headerBytes.Length];
            Buffer.BlockCopy(versionBytes, 0, fullHeader, 0, versionBytes.Length);
            Buffer.BlockCopy(headerBytes, 0, fullHeader, versionBytes.Length, headerBytes.Length);
            return fullHeader;
        }
    }
}

