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
        // Should only be set in Init()
        private Action onPluginError;

        private PluginInitContext _context;

        private string _iconPath;

        private bool _disposed;

        public static string PluginID => "2FA48E560F1D45C09FB969D6C403AA13";

        public string Name => Properties.Resources.plugin_name;

        public string Description => Properties.Resources.plugin_description;

        private static readonly CompositeFormat InBrowserName = System.Text.CompositeFormat.Parse(Properties.Resources.plugin_in_browser_name);
        private static readonly CompositeFormat Open = System.Text.CompositeFormat.Parse(Properties.Resources.plugin_open);
        private static readonly CompositeFormat SearhFailed = System.Text.CompositeFormat.Parse(Properties.Resources.plugin_search_failed);

        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            return new List<ContextMenuResult>(0);
        }

        public List<Result> Query(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var results = new List<Result>();

            // empty query
            if (string.IsNullOrEmpty(query.Search))
            {
                string arguments = "https://chatgpt.com/";
                results.Add(new Result
                {
                    Title = Properties.Resources.plugin_description,
                    SubTitle = string.Format(CultureInfo.CurrentCulture, InBrowserName, BrowserInfo.Name ?? BrowserInfo.MSEdgeName),
                    QueryTextDisplay = string.Empty,
                    IcoPath = _iconPath,
                    ProgramArguments = arguments,
                    Action = action =>
                    {
                        if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, arguments))
                        {
                            onPluginError();
                            return false;
                        }

                        return true;
                    },
                });
                return results;
            }
            else
            {
                string searchTerm = query.Search;

                var result = new Result
                {
                    Title = searchTerm,
                    SubTitle = string.Format(CultureInfo.CurrentCulture, Open, BrowserInfo.Name ?? BrowserInfo.MSEdgeName),
                    QueryTextDisplay = searchTerm,
                    IcoPath = _iconPath,
                };

                string arguments = $"https://chatgpt.com/?q={HttpUtility.UrlEncode(searchTerm)}";

                result.ProgramArguments = arguments;
                result.Action = action =>
                {
                    if (!Helper.OpenCommandInShell(BrowserInfo.Path, BrowserInfo.ArgumentsPattern, arguments))
                    {
                        onPluginError();
                        return false;
                    }

                    return true;
                };

                results.Add(result);
            }

            return results;
        }

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(_context.API.GetCurrentTheme());
            BrowserInfo.UpdateIfTimePassed();

            onPluginError = () =>
            {
                string errorMsgString = string.Format(CultureInfo.CurrentCulture, SearhFailed, BrowserInfo.Name ?? BrowserInfo.MSEdgeName);

                Log.Error(errorMsgString, this.GetType());
                _context.API.ShowMsg(
                    $"Plugin: {Properties.Resources.plugin_name}",
                    errorMsgString);
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
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                _iconPath = "Images/ChatGPT.light.png";
            }
            else
            {
                _iconPath = "Images/ChatGPT.dark.png";
            }
        }

        public void ReloadData()
        {
            if (_context is null)
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
                if (_context != null && _context.API != null)
                {
                    _context.API.ThemeChanged -= OnThemeChanged;
                }

                _disposed = true;
            }
        }
    }
}
