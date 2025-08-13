using System;
using System.Collections.Generic;
using Xunit;
using TinyTuya;
using TinyTuya.Core;

namespace TinyTuya.Tests
{
    /// <summary>
    /// Tests for the Device class
    /// </summary>
    public class DeviceTests
    {
        /// <summary>
        /// Tests the device constructor
        /// </summary>
        [Fact]
        public void TestDeviceConstructor()
        {
            var device = new Device(
                deviceId: "12345678901234567890",
                address: "192.168.1.100",
                localKey: "1234567890abcdef",
                version: 3.3
            );

            Assert.Equal("12345678901234567890", device.Id);
            Assert.Equal("192.168.1.100", device.Address);
            Assert.Equal(3.3, device.Version);
        }

        /// <summary>
        /// Tests the merge DPS results method
        /// </summary>
        [Fact]
        public void TestMergeDpsResults()
        {
            var dest = new Dictionary<string, object>
            {
                { "dps", new Dictionary<string, object>
                    {
                        { "1", true },
                        { "2", 100 }
                    }
                }
            };

            var src = new Dictionary<string, object>
            {
                { "dps", new Dictionary<string, object>
                    {
                        { "3", false },
                        { "4", 200 }
                    }
                }
            };

            Device.MergeDpsResults(dest, src);

            Assert.True(((Dictionary<string, object>)dest["dps"]).ContainsKey("1"));
            Assert.True(((Dictionary<string, object>)dest["dps"]).ContainsKey("2"));
            Assert.True(((Dictionary<string, object>)dest["dps"]).ContainsKey("3"));
            Assert.True(((Dictionary<string, object>)dest["dps"]).ContainsKey("4"));
            Assert.Equal(true, ((Dictionary<string, object>)dest["dps"])["1"]);
            Assert.Equal(100, ((Dictionary<string, object>)dest["dps"])["2"]);
            Assert.Equal(false, ((Dictionary<string, object>)dest["dps"])["3"]);
            Assert.Equal(200, ((Dictionary<string, object>)dest["dps"])["4"]);
        }
    }
}

