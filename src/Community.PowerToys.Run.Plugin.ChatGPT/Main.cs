// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using ManagedCommon;
using Wox.Infrastructure;
using Wox.Plugin;
using Wox.Plugin.Logger;
using BrowserInfo = Wox.Plugin.Common.DefaultBrowserInfo;

namespace Community.PowerToys.Run.Plugin.ChatGPT
{
    public class Main : IPlugin, IPluginI18n, IContextMenu, IReloadable, IDisposable
    {
        private Action onPluginError;
        private PluginInitContext _context;
        private string _iconPath;
        private bool _disposed;

        public static string PluginID => "2FA48E560F1D45C09FB969D6C403AA13";

        public string Name => Properties.Resources.plugin_name;

        public string Description => Properties.Resources.plugin_description;

        private static readonly CompositeFormat InBrowserName =
            CompositeFormat.Parse(Properties.Resources.plugin_in_browser_name);

        private static readonly CompositeFormat Open =
            CompositeFormat.Parse(Properties.Resources.plugin_open);

        private static readonly CompositeFormat SearchFailed =
            CompositeFormat.Parse(Properties.Resources.plugin_search_failed);

        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            return new List<ContextMenuResult>();
        }

        public List<Result> Query(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var results = new List<Result>();
            string raw = query.Search?.Trim() ?? string.Empty;

            // Split by space for subcommands
            string[] parts = raw.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            string command = parts.Length > 0 ? parts[0].ToLowerInvariant() : string.Empty;
            string argument = parts.Length > 1 ? parts[1] : string.Empty;

            // TEMPORARY CHAT: gpt tm <query>
            if (command == "tm")
            {
                string url;

                if (string.IsNullOrEmpty(argument))
                {
                    url = "https://chatgpt.com/?temporary-chat=true";
                }
                else
                {
                    url = $"https://chatgpt.com/?temporary-chat=true&q={HttpUtility.UrlEncode(argument)}";
                }

                results.Add(new Result
                {
                    Title = string.IsNullOrEmpty(argument)
                        ? "New Temporary Chat"
                        : $"Temporary Chat: {argument}",
                    SubTitle = $"Open in {BrowserInfo.Name ?? BrowserInfo.MSEdgeName}",
                    IcoPath = _iconPath,
                    QueryTextDisplay = raw,
                    ProgramArguments = url,
                    Action = action =>
                    {
                        if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, url))
                        {
                            onPluginError();
                            return false;
                        }

                        return true;
                    },
                });

                return results;
            }

            // NORMAL CHAT: gpt <query>
            if (!string.IsNullOrEmpty(raw))
            {
                string url = $"https://chatgpt.com/?q={HttpUtility.UrlEncode(raw)}";

                results.Add(new Result
                {
                    Title = raw,
                    SubTitle = string.Format(CultureInfo.CurrentCulture, Open, BrowserInfo.Name ?? BrowserInfo.MSEdgeName),
                    QueryTextDisplay = raw,
                    IcoPath = _iconPath,
                    ProgramArguments = url,
                    Action = action =>
                    {
                        if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, url))
                        {
                            onPluginError();
                            return false;
                        }

                        return true;
                    },
                });

                return results;
            }

            {
                string url = "https://chatgpt.com/";

                results.Add(new Result
                {
                    Title = Properties.Resources.plugin_description,
                    SubTitle = string.Format(CultureInfo.CurrentCulture, InBrowserName, BrowserInfo.Name ?? BrowserInfo.MSEdgeName),
                    IcoPath = _iconPath,
                    ProgramArguments = url,
                    Action = action =>
                    {
                        if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, url))
                        {
                            onPluginError();
                            return false;
                        }

                        return true;
                    },
                });

                return results;
            }
        }

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(_context.API.GetCurrentTheme());
            BrowserInfo.UpdateIfTimePassed();

            onPluginError = () =>
            {
                string errorMsg = string.Format(
                    CultureInfo.CurrentCulture,
                    SearchFailed,
                    BrowserInfo.Name ?? BrowserInfo.MSEdgeName);

                Log.Error(errorMsg, GetType());
                _context.API.ShowMsg($"Plugin: {Properties.Resources.plugin_name}", errorMsg);
            };
        }

        public string GetTranslatedPluginTitle()
        {
            return Properties.Resources.plugin_name;
        }

        public string GetTranslatedPluginDescription()
        {
            return Properties.Resources.plugin_description;
        }

        private void OnThemeChanged(Theme oldtheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private void UpdateIconPath(Theme theme)
        {
            _iconPath = (theme == Theme.Light || theme == Theme.HighContrastWhite)
                ? "Images/ChatGPT.light.png"
                : "Images/ChatGPT.dark.png";
        }

        public void ReloadData()
        {
            if (_context == null)
            {
                return;
            }

            UpdateIconPath(_context.API.GetCurrentTheme());
            BrowserInfo.UpdateIfTimePassed();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_context?.API != null)
                {
                    _context.API.ThemeChanged -= OnThemeChanged;
                }

                _disposed = true;
            }
        }
    }
}
