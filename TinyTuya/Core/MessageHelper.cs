using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TinyTuya.Core
{
    /// <summary>
    /// Represents a Tuya message payload
    /// </summary>
    public class MessagePayload
    {
        /// <summary>
        /// Gets or sets the device ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the gateway ID
        /// </summary>
        public string GatewayId { get; set; }

        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public Dictionary<string, object> Command { get; set; }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the protocol
        /// </summary>
        public int? Protocol { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePayload"/> class
        /// </summary>
        public MessagePayload()
        {
            Command = new Dictionary<string, object>();
        }

        /// <summary>
        /// Converts the payload to a JSON string
        /// </summary>
        /// <returns>The JSON string</returns>
        public string ToJson()
        {
            var payload = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(DeviceId))
            {
                payload["devId"] = DeviceId;
            }

            if (!string.IsNullOrEmpty(GatewayId))
            {
                payload["gwId"] = GatewayId;
            }

            if (!string.IsNullOrEmpty(UserId))
            {
                payload["uid"] = UserId;
            }

            if (!string.IsNullOrEmpty(Timestamp))
            {
                payload["t"] = Timestamp;
            }

            if (Protocol.HasValue)
            {
                payload["protocol"] = Protocol.Value;
            }

            if (Data != null)
            {
                payload["data"] = Data;
            }

            foreach (var kvp in Command)
            {
                payload[kvp.Key] = kvp.Value;
            }

            return JsonConvert.SerializeObject(payload);
        }
    }

    /// <summary>
    /// Represents a Tuya message
    /// </summary>
    public class TuyaMessage
    {
        /// <summary>
        /// Gets or sets the prefix
        /// </summary>
        public byte[] Prefix { get; set; }

        /// <summary>
        /// Gets or sets the sequence number
        /// </summary>
        public uint SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public int Command { get; set; }

        /// <summary>
        /// Gets or sets the payload
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        /// Gets or sets the CRC32
        /// </summary>
        public uint CRC32 { get; set; }

        /// <summary>
        /// Gets or sets the suffix
        /// </summary>
        public byte[] Suffix { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TuyaMessage"/> class
        /// </summary>
        public TuyaMessage()
        {
            Prefix = new byte[] { 0x00, 0x00, 0x55, 0xaa };
            Suffix = new byte[] { 0x00, 0x00, 0xaa, 0x55 };
        }
    }

    /// <summary>
    /// Helper class for message operations
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// Packs a message into a byte array
        /// </summary>
        /// <param name="message">The message to pack</param>
        /// <returns>The packed message as a byte array</returns>
        public static byte[] PackMessage(TuyaMessage message)
        {
            var payloadLength = message.Payload?.Length ?? 0;
            var result = new byte[message.Prefix.Length + 4 + 4 + 4 + payloadLength + 4 + message.Suffix.Length];
            var index = 0;

            // Prefix
            Buffer.BlockCopy(message.Prefix, 0, result, index, message.Prefix.Length);
            index += message.Prefix.Length;

            // Sequence number
            var seqBytes = BitConverter.GetBytes(message.SequenceNumber);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(seqBytes);
            }
            Buffer.BlockCopy(seqBytes, 0, result, index, 4);
            index += 4;

            // Command
            var cmdBytes = BitConverter.GetBytes(message.Command);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(cmdBytes);
            }
            Buffer.BlockCopy(cmdBytes, 0, result, index, 4);
            index += 4;

            // Payload length
            var lenBytes = BitConverter.GetBytes(payloadLength);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenBytes);
            }
            Buffer.BlockCopy(lenBytes, 0, result, index, 4);
            index += 4;

            // Payload
            if (payloadLength > 0)
            {
                Buffer.BlockCopy(message.Payload, 0, result, index, payloadLength);
                index += payloadLength;
            }

            // CRC32
            var crcBytes = BitConverter.GetBytes(message.CRC32);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(crcBytes);
            }
            Buffer.BlockCopy(crcBytes, 0, result, index, 4);
            index += 4;

            // Suffix
            Buffer.BlockCopy(message.Suffix, 0, result, index, message.Suffix.Length);

            return result;
        }

        /// <summary>
        /// Unpacks a byte array into a message
        /// </summary>
        /// <param name="data">The data to unpack</param>
        /// <returns>The unpacked message</returns>
        public static TuyaMessage UnpackMessage(byte[] data)
        {
            if (data.Length < 16)
            {
                throw new DecodeException("Data too short to be a valid message");
            }

            var message = new TuyaMessage();
            var index = 0;

            // Prefix
            message.Prefix = new byte[4];
            Buffer.BlockCopy(data, index, message.Prefix, 0, 4);
            index += 4;

            // Sequence number
            var seqBytes = new byte[4];
            Buffer.BlockCopy(data, index, seqBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(seqBytes);
            }
            message.SequenceNumber = BitConverter.ToUInt32(seqBytes, 0);
            index += 4;

            // Command
            var cmdBytes = new byte[4];
            Buffer.BlockCopy(data, index, cmdBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(cmdBytes);
            }
            message.Command = BitConverter.ToInt32(cmdBytes, 0);
            index += 4;

            // Payload length
            var lenBytes = new byte[4];
            Buffer.BlockCopy(data, index, lenBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenBytes);
            }
            var payloadLength = BitConverter.ToInt32(lenBytes, 0);
            index += 4;

            if (payloadLength > 0)
            {
                // Payload
                message.Payload = new byte[payloadLength];
                Buffer.BlockCopy(data, index, message.Payload, 0, payloadLength);
                index += payloadLength;
            }
            else
            {
                message.Payload = new byte[0];
            }

            // CRC32
            var crcBytes = new byte[4];
            Buffer.BlockCopy(data, index, crcBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(crcBytes);
            }
            message.CRC32 = BitConverter.ToUInt32(crcBytes, 0);
            index += 4;

            // Suffix
            message.Suffix = new byte[4];
            Buffer.BlockCopy(data, index, message.Suffix, 0, 4);

            return message;
        }

        /// <summary>
        /// Parses the header of a message
        /// </summary>
        /// <param name="data">The data to parse</param>
        /// <returns>A dictionary containing the header information</returns>
        public static Dictionary<string, object> ParseHeader(byte[] data)
        {
            if (data.Length < 16)
            {
                throw new DecodeException("Data too short to be a valid message");
            }

            var result = new Dictionary<string, object>();
            var index = 0;

            // Prefix
            var prefix = new byte[4];
            Buffer.BlockCopy(data, index, prefix, 0, 4);
            result["prefix"] = prefix;
            index += 4;

            // Sequence number
            var seqBytes = new byte[4];
            Buffer.BlockCopy(data, index, seqBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(seqBytes);
            }
            result["seq"] = BitConverter.ToUInt32(seqBytes, 0);
            index += 4;

            // Command
            var cmdBytes = new byte[4];
            Buffer.BlockCopy(data, index, cmdBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(cmdBytes);
            }
            result["cmd"] = BitConverter.ToInt32(cmdBytes, 0);
            index += 4;

            // Payload length
            var lenBytes = new byte[4];
            Buffer.BlockCopy(data, index, lenBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenBytes);
            }
            result["len"] = BitConverter.ToInt32(lenBytes, 0);

            return result;
        }
    }
}

