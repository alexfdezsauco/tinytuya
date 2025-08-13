# TinyTuya C#

C# port of the [TinyTuya](https://github.com/jasonacox/tinytuya) Python library for Tuya WiFi smart devices using a direct local area network (LAN) connection or the cloud (TuyaCloud API).

## Overview

This library allows you to control Tuya WiFi smart devices, such as smart bulbs, outlets, switches, and covers, using a direct local area network (LAN) connection or the cloud (TuyaCloud API). It's a C# port of the popular [TinyTuya](https://github.com/jasonacox/tinytuya) Python library.

## Features

- **Local Control**: Control Tuya devices directly on your local network without relying on the cloud
- **Cloud Control**: Control Tuya devices via the Tuya Cloud API
- **Device Discovery**: Discover Tuya devices on your local network
- **Multiple Device Types**: Support for various device types including:
  - Smart Bulbs (with color, brightness, and scene control)
  - Smart Outlets/Plugs
  - Smart Covers/Curtains
- **Protocol Support**: Support for various Tuya protocol versions (3.1, 3.3, 3.4, 3.5)

## Installation

You can install the TinyTuya C# library via NuGet (coming soon):

```
Install-Package TinyTuya
```

Or clone this repository and build it yourself:

```
git clone https://github.com/alexfdezsauco/tinytuya.git
cd tinytuya
dotnet build
```

## Usage

### Device Discovery

```csharp
using System;
using System.Threading.Tasks;
using TinyTuya;

// Discover devices on the local network
var devices = await Scanner.DiscoverDevices();
foreach (var device in devices)
{
    Console.WriteLine($"Device ID: {device.Value["gwId"]}, IP: {device.Key}, Version: {device.Value["version"]}");
}
```

### Controlling a Device

```csharp
using System;
using TinyTuya;

// Create a device instance
var device = new Device(
    deviceId: "your_device_id",
    address: "device_ip_address",
    localKey: "your_device_local_key",
    version: 3.3
);

// Get device status
var status = device.Status();

// Turn the device on
device.TurnOn();

// Turn the device off
device.TurnOff();
```

### Controlling a Bulb

```csharp
using System;
using TinyTuya;

// Create a bulb device instance
var bulb = new BulbDevice(
    deviceId: "your_device_id",
    address: "device_ip_address",
    localKey: "your_device_local_key",
    version: 3.3
);

// Set the bulb to white mode with brightness 100% and color temperature 0%
bulb.SetWhitePercentage(100, 0);

// Set the bulb color to red
bulb.SetColor(255, 0, 0);

// Set the bulb to scene mode
bulb.SetScene("scene_1");
```

### Cloud API

```csharp
using System;
using TinyTuya;

// Create a cloud instance
var cloud = new Cloud(
    apiRegion: "us",
    apiKey: "your_api_key",
    apiSecret: "your_api_secret",
    apiDeviceId: "your_device_id"
);

// Get devices
var devices = cloud.GetDevices();

// Get device status
var status = cloud.GetStatus("your_device_id");

// Send command to device
var command = new Dictionary<string, object>
{
    { "commands", new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "code", "switch_led" },
                { "value", true }
            }
        }
    }
};
var result = cloud.SendCommand("your_device_id", command);
```

## Requirements

- .NET Standard 2.0 or higher
- Newtonsoft.Json

## Credits

- [TinyTuya](https://github.com/jasonacox/tinytuya) by Jason A. Cox - The original Python library that this project is based on
- [TuyaAPI](https://github.com/codetheweb/tuyapi) by codetheweb and blackrozes - For protocol reverse engineering
- [PyTuya](https://github.com/clach04/python-tuya) by clach04 - The origin of the Python module (now abandoned)
- [LocalTuya](https://github.com/rospogrigio/localtuya-homeassistant) by rospogrigio - Updated pytuya to support devices with Device IDs of 22 characters

## License

This project is licensed under the MIT License - see the LICENSE file for details.

