using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Oxide.Game.Rust.Cui;
using Oxide.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("RLXMenu", "RLX", "1.3.0")]
    [Description("Stylized main menu and rules panel for players")]
    public class RLXMenu : RustPlugin
    {
        private const string UI_MENU = "RLXMenu.Main";
        private const string UI_RULES = "RLXMenu.Rules";

        // Russian UI text is stored as escapes on purpose: the plugin compiler reads sources
        // in the system ANSI code page and mangles non-ASCII literals. Escapes keep the source
        // pure ASCII while the strings themselves stay correct Unicode.
        private const string TXT_WelcomeText = "\u0414\u043e\u0431\u0440\u043e \u043f\u043e\u0436\u0430\u043b\u043e\u0432\u0430\u0442\u044c \u043d\u0430 RLX PROJECT RUST";
        private const string TXT_Subtitle = "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0440\u0430\u0437\u0434\u0435\u043b";
        private const string TXT_RulesTitle = "\u041f\u0420\u0410\u0412\u0418\u041b\u0410 \u0421\u0415\u0420\u0412\u0415\u0420\u0410";
        private const string TXT_CloseLabel = "\u0417\u0430\u043a\u0440\u044b\u0442\u044c";
        private const string TXT_BtnKit = "\u041d\u0430\u0431\u043e\u0440 \u043d\u043e\u0432\u0438\u0447\u043a\u0430";
        private const string TXT_BtnHome = "\u0422\u0435\u043b\u0435\u043f\u043e\u0440\u0442 \u0434\u043e\u043c\u043e\u0439";
        private const string TXT_BtnWho = "\u0418\u0433\u0440\u043e\u043a\u0438 \u043e\u043d\u043b\u0430\u0439\u043d";
        private const string TXT_BtnRules = "\u041f\u0440\u0430\u0432\u0438\u043b\u0430 \u0441\u0435\u0440\u0432\u0435\u0440\u0430";
        private const string TXT_Rule1 = "\u0423\u0432\u0430\u0436\u0430\u0439\u0442\u0435 \u0434\u0440\u0443\u0433\u0438\u0445 \u0438\u0433\u0440\u043e\u043a\u043e\u0432.";
        private const string TXT_Rule2 = "\u0417\u0430\u043f\u0440\u0435\u0449\u0435\u043d\u044b \u0447\u0438\u0442\u044b \u0438 \u0441\u0442\u043e\u0440\u043e\u043d\u043d\u0438\u0435 \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u044b.";
        private const string TXT_Rule3 = "\u0421\u0442\u0440\u043e\u0439\u0442\u0435 \u043a\u043e\u043c\u043f\u0430\u043a\u0442\u043d\u044b\u0435 \u0431\u0430\u0437\u044b.";
        private const string TXT_Rule4 = "\u041d\u0435 \u043e\u0441\u0442\u0430\u0432\u043b\u044f\u0439\u0442\u0435 \u043c\u0443\u0441\u043e\u0440 \u0432 \u0433\u043e\u0440\u043e\u0434\u0430\u0445.";
        private const string TXT_Rule5 = "\u0420\u0435\u0439\u0434\u044b \u043f\u043e \u0434\u043e\u0433\u043e\u0432\u043e\u0440\u0451\u043d\u043d\u043e\u0441\u0442\u0438.";
        private const string TXT_Rule6 = "\u041d\u0430\u0431\u043e\u0440 \u043d\u043e\u0432\u0438\u0447\u043a\u0430: /kit, \u043e\u0434\u0438\u043d \u0440\u0430\u0437 \u043d\u0430 \u0438\u0433\u0440\u043e\u043a\u0430.";
        private const string TXT_Rule7 = "\u041c\u0435\u043d\u044e: /menu";

        private ConfigData _config;
        private readonly HashSet<ulong> _menuOpen = new HashSet<ulong>();
        private readonly HashSet<ulong> _rulesOpen = new HashSet<ulong>();

        private class ConfigData
        {
            public bool ShowMenuOnJoin = true;
            public float ShowDelaySeconds = 12f;
            public bool WelcomeMessage = true;
            public string WelcomeColor = "#f5a623";
            public string OverlayColor = "0 0 0 0.55";
            public string PanelColor = "0.05 0.07 0.1 0.96";
            public string AccentColor = "0.95 0.65 0.15 1";
            public string TextColor = "0.9 0.93 0.97 1";
            public string ButtonColor = "0.13 0.45 0.7 1";
            public string ButtonHoverColor = "0.22 0.58 0.85 1";
            public string CloseColor = "0.55 0.18 0.18 1";
        }

        private void Init()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            _config = Config.ReadObject<ConfigData>();
            Config.WriteObject(_config, true);
        }

        private void OnPlayerConnected(BasePlayer player)
        {
            if (player == null) return;

            if (_config.WelcomeMessage)
            {
                player.ChatMessage($"<color={_config.WelcomeColor}>{TXT_WelcomeText}</color>");
            }

            if (!_config.ShowMenuOnJoin) return;

            TryShowMenu(player, 0);
        }

        private void TryShowMenu(BasePlayer player, int attempt)
        {
            timer.Once(attempt == 0 ? _config.ShowDelaySeconds : 2f, () =>
            {
                if (player == null || !player.IsConnected) return;

                if (player.IsAlive())
                {
                    if (!_menuOpen.Contains(player.userID))
                    {
                        ShowMenu(player);
                    }

                    return;
                }

                if (attempt < 15)
                {
                    TryShowMenu(player, attempt + 1);
                }
            });
        }

        private void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            if (player == null) return;

            _menuOpen.Remove(player.userID);
            _rulesOpen.Remove(player.userID);
        }

        [ChatCommand("menu")]
        private void CmdMenu(BasePlayer player, string command, string[] args)
        {
            if (player == null) return;

            if (_menuOpen.Contains(player.userID))
            {
                CloseMenu(player);
                return;
            }

            ShowMenu(player);
        }

        [ChatCommand("rules")]
        private void CmdRules(BasePlayer player, string command, string[] args)
        {
            if (player == null) return;

            if (_rulesOpen.Contains(player.userID))
            {
                CloseRules(player);
                return;
            }

            ShowRules(player);
        }

        private void ShowMenu(BasePlayer player)
        {
            CloseMenu(player);
            CloseRules(player);

            var buttons = new[]
            {
                new KeyValuePair<string, string>(TXT_BtnKit, "kit"),
                new KeyValuePair<string, string>(TXT_BtnHome, "home"),
                new KeyValuePair<string, string>(TXT_BtnWho, "who"),
                new KeyValuePair<string, string>(TXT_BtnRules, "rules"),
                new KeyValuePair<string, string>(TXT_CloseLabel, "menu")
            };

            var json = new StringBuilder();
            json.Append('[');
            AppendOverlay(json, null, UI_MENU, _config.OverlayColor);
            AppendPanel(json, UI_MENU, UI_MENU + ".Panel", "0.32 0.2", "0.68 0.8", _config.PanelColor);
            AppendText(json, UI_MENU + ".Panel", UI_MENU + ".Title", "0.06 0.87 0.94 0.97",
                "RLX PROJECT RUST", 26, _config.AccentColor, "MiddleCenter", "robotocjk-bold");
            AppendText(json, UI_MENU + ".Panel", UI_MENU + ".Subtitle", "0.06 0.82 0.94 0.87",
                TXT_Subtitle, 14, _config.TextColor, "MiddleCenter", "robotocjk-regular");

            var top = 0.78f;
            var bottom = 0.06f;
            var slot = (top - bottom) / buttons.Length;
            var gap = slot * 0.16f;

            for (var i = 0; i < buttons.Length; i++)
            {
                var min = top - (i + 1) * slot + gap;
                var max = top - i * slot - gap;
                var color = buttons[i].Value == "menu" ? _config.CloseColor : _config.ButtonColor;

                AppendButton(json, UI_MENU + ".Panel", UI_MENU + ".Btn." + buttons[i].Value,
                    "0.06 " + F(min) + " 0.94 " + F(max), buttons[i].Key, buttons[i].Value, color);
            }

            json.Append(']');

            CuiHelper.AddUi(player, json.ToString());
            _menuOpen.Add(player.userID);
        }

        private void CloseMenu(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, UI_MENU);
            _menuOpen.Remove(player.userID);
        }

        private void ShowRules(BasePlayer player)
        {
            CloseRules(player);
            CloseMenu(player);

            var lines = new[] { TXT_Rule1, TXT_Rule2, TXT_Rule3, TXT_Rule4, TXT_Rule5, TXT_Rule6, TXT_Rule7 };

            var json = new StringBuilder();
            json.Append('[');
            AppendOverlay(json, null, UI_RULES, _config.OverlayColor);
            AppendPanel(json, UI_RULES, UI_RULES + ".Panel", "0.25 0.2", "0.75 0.8", _config.PanelColor);
            AppendText(json, UI_RULES + ".Panel", UI_RULES + ".Title", "0.06 0.9 0.94 0.98",
                TXT_RulesTitle, 22, _config.AccentColor, "MiddleCenter", "robotocjk-bold");

            var step = 0.84f / lines.Length;

            for (var i = 0; i < lines.Length; i++)
            {
                var min = 0.85f - (i + 1) * step + step * 0.07f;
                var max = 0.85f - i * step - step * 0.07f;

                AppendText(json, UI_RULES + ".Panel", UI_RULES + ".Line" + i,
                    "0.06 " + F(min) + " 0.94 " + F(max), lines[i], 16, _config.TextColor, "MiddleLeft", "robotocjk-regular");
            }

            AppendButton(json, UI_RULES + ".Panel", UI_RULES + ".Close",
                "0.3 0.03 0.7 0.1", TXT_CloseLabel, "rules", _config.CloseColor);

            json.Append(']');

            CuiHelper.AddUi(player, json.ToString());
            _rulesOpen.Add(player.userID);
        }

        private void CloseRules(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, UI_RULES);
            _rulesOpen.Remove(player.userID);
        }

        private void AppendOverlay(StringBuilder json, string parent, string name, string color)
        {
            Comma(json);
            json.Append('{');
            json.Append("\"type\":\"Image\"");
            if (parent != null) json.Append(",\"parent\":\"" + parent + "\"");
            json.Append(",\"name\":\"" + name + "\"");
            json.Append(",\"image\":{\"color\":\"" + color + "\"}");
            json.Append(",\"recttransform\":{\"anchormin\":\"0 0\",\"anchormax\":\"1 1\"}");
            json.Append('}');
        }

        private void AppendPanel(StringBuilder json, string parent, string name, string anchorMin, string anchorMax, string color)
        {
            Comma(json);
            json.Append('{');
            json.Append("\"type\":\"Image\"");
            json.Append(",\"parent\":\"" + parent + "\"");
            json.Append(",\"name\":\"" + name + "\"");
            json.Append(",\"image\":{\"color\":\"" + color + "\"}");
            json.Append(",\"recttransform\":{\"anchormin\":\"" + anchorMin + "\",\"anchormax\":\"" + anchorMax + "\"}");
            json.Append('}');
        }

        private void AppendText(StringBuilder json, string parent, string name, string rect, string text,
            int fontSize, string color, string align, string font)
        {
            Comma(json);
            json.Append('{');
            json.Append("\"type\":\"Text\"");
            json.Append(",\"parent\":\"" + parent + "\"");
            json.Append(",\"name\":\"" + name + "\"");
            json.Append(",\"text\":{\"text\":\"" + Escape(text) + "\",\"fontSize\":" + fontSize +
                        ",\"font\":\"" + font + "\",\"align\":\"" + align + "\",\"color\":\"" + color + "\"}");
            json.Append(",\"recttransform\":{\"anchormin\":\"" + Min(rect) + "\",\"anchormax\":\"" + Max(rect) + "\"}");
            json.Append('}');
        }

        private void AppendButton(StringBuilder json, string parent, string name, string rect, string label,
            string command, string color)
        {
            Comma(json);
            json.Append('{');
            json.Append("\"type\":\"Button\"");
            json.Append(",\"parent\":\"" + parent + "\"");
            json.Append(",\"name\":\"" + name + "\"");
            json.Append(",\"button\":{\"color\":\"" + color + "\",\"normalColor\":\"" + color +
                        "\",\"highlightedColor\":\"" + _config.ButtonHoverColor + "\",\"command\":\"" + command + "\"}");
            json.Append(",\"text\":{\"text\":\"" + Escape(label) + "\",\"fontSize\":18,\"font\":\"robotocjk-bold\",\"align\":\"MiddleCenter\",\"color\":\"" +
                        _config.TextColor + "\"}");
            json.Append(",\"recttransform\":{\"anchormin\":\"" + Min(rect) + "\",\"anchormax\":\"" + Max(rect) + "\"}");
            json.Append('}');
        }

        private static void Comma(StringBuilder json)
        {
            if (json.Length > 1 && json[json.Length - 1] != '[') json.Append(',');
        }

        private static string A(string rect, int index)
        {
            var parts = rect.Split(' ');
            return parts[index];
        }

        private static string Min(string rect)
        {
            var parts = rect.Split(' ');
            return parts[0] + " " + parts[1];
        }

        private static string Max(string rect)
        {
            var parts = rect.Split(' ');
            return parts[2] + " " + parts[3];
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string F(float value)
        {
            return value.ToString("0.000", CultureInfo.InvariantCulture);
        }
    }
}