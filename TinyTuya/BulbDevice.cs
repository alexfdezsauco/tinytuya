using System;
using System.Collections.Generic;
using TinyTuya.Core;

namespace TinyTuya
{
    /// <summary>
    /// Represents a Tuya smart bulb
    /// </summary>
    public class BulbDevice : Device
    {
        /// <summary>
        /// White mode
        /// </summary>
        public const string DPS_MODE_WHITE = "white";

        /// <summary>
        /// Color mode
        /// </summary>
        public const string DPS_MODE_COLOUR = "colour";

        /// <summary>
        /// Scene mode
        /// </summary>
        public const string DPS_MODE_SCENE = "scene";

        /// <summary>
        /// Music mode
        /// </summary>
        public const string DPS_MODE_MUSIC = "music";

        /// <summary>
        /// Scene 1 mode (nature)
        /// </summary>
        public const string DPS_MODE_SCENE_1 = "scene_1";

        /// <summary>
        /// Scene 2 mode
        /// </summary>
        public const string DPS_MODE_SCENE_2 = "scene_2";

        /// <summary>
        /// Scene 3 mode (rave)
        /// </summary>
        public const string DPS_MODE_SCENE_3 = "scene_3";

        /// <summary>
        /// Scene 4 mode (rainbow)
        /// </summary>
        public const string DPS_MODE_SCENE_4 = "scene_4";

        /// <summary>
        /// Mode feature
        /// </summary>
        public const string BULB_FEATURE_MODE = "mode";

        /// <summary>
        /// Brightness feature
        /// </summary>
        public const string BULB_FEATURE_BRIGHTNESS = "brightness";

        /// <summary>
        /// Color temperature feature
        /// </summary>
        public const string BULB_FEATURE_COLOURTEMP = "colourtemp";

        /// <summary>
        /// Color feature
        /// </summary>
        public const string BULB_FEATURE_COLOUR = "colour";

        /// <summary>
        /// Scene feature
        /// </summary>
        public const string BULB_FEATURE_SCENE = "scene";

        /// <summary>
        /// Scene data feature
        /// </summary>
        public const string BULB_FEATURE_SCENE_DATA = "scene_data";

        /// <summary>
        /// Timer feature
        /// </summary>
        public const string BULB_FEATURE_TIMER = "timer";

        /// <summary>
        /// Music feature
        /// </summary>
        public const string BULB_FEATURE_MUSIC = "music";

        /// <summary>
        /// Jump music transition
        /// </summary>
        public const int MUSIC_TRANSITION_JUMP = 0;

        /// <summary>
        /// Fade music transition
        /// </summary>
        public const int MUSIC_TRANSITION_FADE = 1;

        private readonly Dictionary<string, Dictionary<string, object>> _defaultDpSet;
        private bool _bulbConfigured;
        private string _bulbType;
        private bool? _hasBrightness;
        private bool? _hasColourtemp;
        private bool? _hasColour;
        private bool _triedStatus;
        private Dictionary<string, object> _dpSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulbDevice"/> class
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
        public BulbDevice(
            string deviceId,
            string address = null,
            string localKey = "",
            string deviceType = "default",
            int connectionTimeout = 5,
            double? version = null,
            bool persist = false,
            string childId = null,
            XenonDevice parent = null,
            int connectionRetryLimit = 5,
            int connectionRetryDelay = 5,
            int port = Constants.TCPPORT,
            int maxSimultaneousDps = 0)
            : base(
                deviceId,
                address,
                localKey,
                deviceType,
                connectionTimeout,
                version ?? 0,
                persist,
                childId,
                parent,
                connectionRetryLimit,
                connectionRetryDelay,
                port,
                maxSimultaneousDps)
        {
            _bulbConfigured = false;
            _bulbType = null;
            _hasBrightness = null;
            _hasColourtemp = null;
            _hasColour = null;
            _triedStatus = false;
            _dpSet = new Dictionary<string, object>
            {
                { "switch", null },
                { "mode", null },
                { "brightness", null },
                { "colourtemp", null },
                { "colour", null },
                { "scene", null },
                { "scene_data", null },
                { "timer", null },
                { "music", null },
                { "value_min", -1 },
                { "value_max", -1 },
                { "value_hexformat", "hsv16" }
            };

            _defaultDpSet = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "A", new Dictionary<string, object>
                    {
                        { "switch", 1 },
                        { "mode", 2 },
                        { "brightness", 3 },
                        { "colourtemp", 4 },
                        { "colour", 5 },
                        { "scene", 6 },
                        { "scene_data", null },
                        { "timer", 7 },
                        { "music", 8 },
                        { "value_min", 25 },
                        { "value_max", 255 },
                        { "value_hexformat", "rgb8" }
                    }
                },
                {
                    "B", new Dictionary<string, object>
                    {
                        { "switch", 20 },
                        { "mode", 21 },
                        { "brightness", 22 },
                        { "colourtemp", 23 },
                        { "colour", 24 },
                        { "scene", 25 },
                        { "scene_data", 25 },
                        { "timer", 26 },
                        { "music", 28 },
                        { "value_min", 10 },
                        { "value_max", 1000 },
                        { "value_hexformat", "hsv16" }
                    }
                },
                {
                    "C", new Dictionary<string, object>
                    {
                        { "switch", 1 },
                        { "mode", null },
                        { "brightness", 2 },
                        { "colourtemp", 3 },
                        { "colour", null },
                        { "scene", null },
                        { "scene_data", null },
                        { "timer", null },
                        { "music", null },
                        { "value_min", 25 },
                        { "value_max", 255 },
                        { "value_hexformat", "rgb8" }
                    }
                },
                {
                    "None", new Dictionary<string, object>
                    {
                        { "switch", 1 },
                        { "mode", null },
                        { "brightness", null },
                        { "colourtemp", null },
                        { "colour", null },
                        { "scene", null },
                        { "scene_data", null },
                        { "timer", null },
                        { "music", null },
                        { "value_min", 0 },
                        { "value_max", 255 },
                        { "value_hexformat", "rgb8" }
                    }
                }
            };
        }

        /// <summary>
        /// Gets the status of the device
        /// </summary>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The status</returns>
        public override Dictionary<string, object> Status(bool nowait = false)
        {
            var result = base.Status(nowait);
            _triedStatus = true;
            if (result != null && !_bulbConfigured && result.ContainsKey("dps"))
            {
                DetectBulb(result, nowait);
            }
            return result;
        }

        /// <summary>
        /// Detects the bulb type
        /// </summary>
        /// <param name="response">The response</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The bulb type</returns>
        public string DetectBulb(Dictionary<string, object> response = null, bool nowait = false)
        {
            if (response == null)
            {
                response = Status(nowait);
            }

            if (response == null || !response.ContainsKey("dps") || !(response["dps"] is Dictionary<string, object> dps))
            {
                return null;
            }

            // Check for Type B - has DPS 20 for switch
            if (dps.ContainsKey("20"))
            {
                SetBulbType("B");
                return "B";
            }

            // Check for Type A - has DPS 1 for switch and DPS 2 for mode
            if (dps.ContainsKey("1") && dps.ContainsKey("2"))
            {
                SetBulbType("A");
                return "A";
            }

            // Check for Type C - has DPS 1 for switch and DPS 2 for brightness
            if (dps.ContainsKey("1") && dps.ContainsKey("2") && !dps.ContainsKey("3"))
            {
                SetBulbType("C");
                return "C";
            }

            // Default to Type A
            SetBulbType("A");
            return "A";
        }

        /// <summary>
        /// Sets the bulb type
        /// </summary>
        /// <param name="bulbType">The bulb type</param>
        /// <param name="mapping">The mapping</param>
        public void SetBulbType(string bulbType = null, Dictionary<string, object> mapping = null)
        {
            if (bulbType != null)
            {
                _bulbType = bulbType;
            }

            if (mapping != null)
            {
                SetBulbCapabilities(mapping);
            }
            else if (_bulbType != null && _defaultDpSet.ContainsKey(_bulbType))
            {
                SetBulbCapabilities(_defaultDpSet[_bulbType]);
            }
            else
            {
                SetBulbCapabilities(_defaultDpSet["None"]);
            }
        }

        /// <summary>
        /// Sets the bulb capabilities
        /// </summary>
        /// <param name="mapping">The mapping</param>
        public void SetBulbCapabilities(Dictionary<string, object> mapping)
        {
            foreach (var kvp in mapping)
            {
                _dpSet[kvp.Key] = kvp.Value;
            }

            _hasBrightness = _dpSet["brightness"] != null;
            _hasColourtemp = _dpSet["colourtemp"] != null;
            _hasColour = _dpSet["colour"] != null;
            _bulbConfigured = true;
        }

        /// <summary>
        /// Checks if the bulb has a capability
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>True if the bulb has the capability, false otherwise</returns>
        public bool BulbHasCapability(string feature, bool nowait = false)
        {
            if (!_bulbConfigured && !_triedStatus)
            {
                Status(nowait);
            }

            switch (feature)
            {
                case BULB_FEATURE_MODE:
                    return _dpSet["mode"] != null;
                case BULB_FEATURE_BRIGHTNESS:
                    return _hasBrightness ?? false;
                case BULB_FEATURE_COLOURTEMP:
                    return _hasColourtemp ?? false;
                case BULB_FEATURE_COLOUR:
                    return _hasColour ?? false;
                case BULB_FEATURE_SCENE:
                    return _dpSet["scene"] != null;
                case BULB_FEATURE_SCENE_DATA:
                    return _dpSet["scene_data"] != null;
                case BULB_FEATURE_TIMER:
                    return _dpSet["timer"] != null;
                case BULB_FEATURE_MUSIC:
                    return _dpSet["music"] != null;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Sets the mode
        /// </summary>
        /// <param name="mode">The mode</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetMode(string mode = DPS_MODE_WHITE, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_MODE, nowait))
            {
                return null;
            }

            return SetValue(_dpSet["mode"].ToString(), mode, nowait);
        }

        /// <summary>
        /// Sets the scene
        /// </summary>
        /// <param name="scene">The scene</param>
        /// <param name="sceneData">The scene data</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetScene(string scene, object sceneData = null, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_SCENE, nowait))
            {
                return null;
            }

            if (_bulbType == "A")
            {
                return SetMode(scene, nowait);
            }
            else if (_bulbType == "B" && sceneData != null)
            {
                return SetValue(_dpSet["scene"].ToString(), sceneData, nowait);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the color
        /// </summary>
        /// <param name="r">The red component</param>
        /// <param name="g">The green component</param>
        /// <param name="b">The blue component</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetColor(int r, int g, int b, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_COLOUR, nowait))
            {
                return null;
            }

            // TODO: Implement color conversion
            return null;
        }

        /// <summary>
        /// Sets the HSV color
        /// </summary>
        /// <param name="h">The hue</param>
        /// <param name="s">The saturation</param>
        /// <param name="v">The value</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetHsv(int h, int s, int v, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_COLOUR, nowait))
            {
                return null;
            }

            // TODO: Implement HSV conversion
            return null;
        }

        /// <summary>
        /// Sets the white mode
        /// </summary>
        /// <param name="brightness">The brightness</param>
        /// <param name="colorTemp">The color temperature</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetWhite(int brightness, int colorTemp, bool nowait = false)
        {
            var result = SetMode(DPS_MODE_WHITE, nowait);
            if (result == null)
            {
                return null;
            }

            if (BulbHasCapability(BULB_FEATURE_BRIGHTNESS, nowait))
            {
                SetValue(_dpSet["brightness"].ToString(), brightness, nowait);
            }

            if (BulbHasCapability(BULB_FEATURE_COLOURTEMP, nowait))
            {
                SetValue(_dpSet["colourtemp"].ToString(), colorTemp, nowait);
            }

            return result;
        }

        /// <summary>
        /// Sets the white mode with percentage values
        /// </summary>
        /// <param name="brightness">The brightness percentage</param>
        /// <param name="colorTemp">The color temperature percentage</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetWhitePercentage(int brightness = 100, int colorTemp = 0, bool nowait = false)
        {
            var valueMin = Convert.ToInt32(_dpSet["value_min"]);
            var valueMax = Convert.ToInt32(_dpSet["value_max"]);
            var brightnessValue = valueMin + (valueMax - valueMin) * brightness / 100;
            var colorTempValue = valueMin + (valueMax - valueMin) * colorTemp / 100;
            return SetWhite(brightnessValue, colorTempValue, nowait);
        }

        /// <summary>
        /// Sets the brightness
        /// </summary>
        /// <param name="brightness">The brightness</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetBrightness(int brightness, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_BRIGHTNESS, nowait))
            {
                return null;
            }

            return SetValue(_dpSet["brightness"].ToString(), brightness, nowait);
        }

        /// <summary>
        /// Sets the brightness percentage
        /// </summary>
        /// <param name="brightness">The brightness percentage</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetBrightnessPercentage(int brightness = 100, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_BRIGHTNESS, nowait))
            {
                return null;
            }

            var valueMin = Convert.ToInt32(_dpSet["value_min"]);
            var valueMax = Convert.ToInt32(_dpSet["value_max"]);
            var brightnessValue = valueMin + (valueMax - valueMin) * brightness / 100;
            return SetBrightness(brightnessValue, nowait);
        }

        /// <summary>
        /// Sets the color temperature
        /// </summary>
        /// <param name="colorTemp">The color temperature</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetColorTemp(int colorTemp, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_COLOURTEMP, nowait))
            {
                return null;
            }

            return SetValue(_dpSet["colourtemp"].ToString(), colorTemp, nowait);
        }

        /// <summary>
        /// Sets the color temperature percentage
        /// </summary>
        /// <param name="colorTemp">The color temperature percentage</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetColorTempPercentage(int colorTemp = 100, bool nowait = false)
        {
            if (!BulbHasCapability(BULB_FEATURE_COLOURTEMP, nowait))
            {
                return null;
            }

            var valueMin = Convert.ToInt32(_dpSet["value_min"]);
            var valueMax = Convert.ToInt32(_dpSet["value_max"]);
            var colorTempValue = valueMin + (valueMax - valueMin) * colorTemp / 100;
            return SetColorTemp(colorTempValue, nowait);
        }

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="state">The state</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The value</returns>
        public object GetValue(string feature, Dictionary<string, object> state = null, bool nowait = false)
        {
            if (state == null)
            {
                state = Status(nowait);
            }

            if (state == null || !state.ContainsKey("dps") || !(state["dps"] is Dictionary<string, object> dps))
            {
                return null;
            }

            if (!BulbHasCapability(feature, nowait))
            {
                return null;
            }

            switch (feature)
            {
                case BULB_FEATURE_MODE:
                    return dps.ContainsKey(_dpSet["mode"].ToString()) ? dps[_dpSet["mode"].ToString()] : null;
                case BULB_FEATURE_BRIGHTNESS:
                    return dps.ContainsKey(_dpSet["brightness"].ToString()) ? dps[_dpSet["brightness"].ToString()] : null;
                case BULB_FEATURE_COLOURTEMP:
                    return dps.ContainsKey(_dpSet["colourtemp"].ToString()) ? dps[_dpSet["colourtemp"].ToString()] : null;
                case BULB_FEATURE_COLOUR:
                    return dps.ContainsKey(_dpSet["colour"].ToString()) ? dps[_dpSet["colour"].ToString()] : null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the mode
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The mode</returns>
        public string GetMode(Dictionary<string, object> state = null, bool nowait = false)
        {
            return GetValue(BULB_FEATURE_MODE, state, nowait)?.ToString();
        }

        /// <summary>
        /// Gets the brightness percentage
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The brightness percentage</returns>
        public int? GetBrightnessPercentage(Dictionary<string, object> state = null, bool nowait = false)
        {
            var brightness = GetValue(BULB_FEATURE_BRIGHTNESS, state, nowait);
            if (brightness == null)
            {
                return null;
            }

            var valueMin = Convert.ToInt32(_dpSet["value_min"]);
            var valueMax = Convert.ToInt32(_dpSet["value_max"]);
            var brightnessValue = Convert.ToInt32(brightness);
            return (brightnessValue - valueMin) * 100 / (valueMax - valueMin);
        }

        /// <summary>
        /// Gets the color temperature percentage
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The color temperature percentage</returns>
        public int? GetColorTempPercentage(Dictionary<string, object> state = null, bool nowait = false)
        {
            var colorTemp = GetValue(BULB_FEATURE_COLOURTEMP, state, nowait);
            if (colorTemp == null)
            {
                return null;
            }

            var valueMin = Convert.ToInt32(_dpSet["value_min"]);
            var valueMax = Convert.ToInt32(_dpSet["value_max"]);
            var colorTempValue = Convert.ToInt32(colorTemp);
            return (colorTempValue - valueMin) * 100 / (valueMax - valueMin);
        }

        /// <summary>
        /// Gets the RGB color
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The RGB color</returns>
        public (int, int, int)? GetColorRgb(Dictionary<string, object> state = null, bool nowait = false)
        {
            var color = GetValue(BULB_FEATURE_COLOUR, state, nowait);
            if (color == null)
            {
                return null;
            }

            // TODO: Implement color conversion
            return (0, 0, 0);
        }

        /// <summary>
        /// Gets the HSV color
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The HSV color</returns>
        public (int, int, int)? GetColorHsv(Dictionary<string, object> state = null, bool nowait = false)
        {
            var color = GetValue(BULB_FEATURE_COLOUR, state, nowait);
            if (color == null)
            {
                return null;
            }

            // TODO: Implement color conversion
            return (0, 0, 0);
        }

        /// <summary>
        /// Gets the state
        /// </summary>
        /// <param name="state">The state</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The state</returns>
        public bool? GetState(Dictionary<string, object> state = null, bool nowait = false)
        {
            if (state == null)
            {
                state = Status(nowait);
            }

            if (state == null || !state.ContainsKey("dps") || !(state["dps"] is Dictionary<string, object> dps))
            {
                return null;
            }

            if (!dps.ContainsKey(_dpSet["switch"].ToString()))
            {
                return null;
            }

            return Convert.ToBoolean(dps[_dpSet["switch"].ToString()]);
        }
    }
}

