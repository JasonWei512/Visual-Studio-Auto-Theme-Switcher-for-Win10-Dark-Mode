using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VsAutoThemeSwitcherForWin10DarkMode.Helpers;
using VsAutoThemeSwitcherForWin10DarkMode.Options;
using VsAutoThemeSwitcherForWin10DarkMode.Properties;
using static System.Environment;

namespace VsAutoThemeSwitcherForWin10DarkMode.Logic
{
    /// <summary>Provides methods to manage installed Visual Studio themes.</summary>
    internal static class ThemeManager
    {
        #region Private Members

        private const string WindowsThemeRegestryKeyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private const string WindowsThemeRegistryValueName = "AppsUseLightTheme";

        /// <summary>Gets the DTE automation object.</summary>
        private static DTE2 Dte2
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return (DTE2)Package.GetGlobalService(typeof(DTE));
            }
        }

        #endregion

        #region Public Members

        /// <summary>Gets whether the current Windows version supports dark theme.</summary>
        /// <returns>A boolean which indicates whether the current Windows version supports dark theme.</returns>
        public static bool OsSupportsDarkTheme => Registry.GetValue(WindowsThemeRegestryKeyName, WindowsThemeRegistryValueName, null) != null;

        /// <summary>Gets whether Windows 10 is currently in light theme.</summary>
        /// <returns>A boolean which suggets whether Windows 10 is currently in light theme.</returns>
        /// <exception cref="NotSupportedException">Occurs if the current Windows version does not support light/dark theme.</exception>
        public static bool OsInLightTheme
        {
            get 
            {
                object registryValue = Registry.GetValue(WindowsThemeRegestryKeyName, WindowsThemeRegistryValueName, null);

                if (registryValue != null)
                {
                    return Convert.ToUInt32(registryValue) == 1;
                }
                else
                {
                    throw new NotSupportedException(
                        Resources.Registry_value_not_found_ + NewLine + 
                        Path.Combine(WindowsThemeRegestryKeyName, WindowsThemeRegistryValueName));
                }
            }
        }

        /// <summary>Gets all themes installed in Visual Studio.</summary>
        /// <returns>All themes installed in Visual Studio.</returns>
        public static IEnumerable<Theme> GetInstalledThemes()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string registryPath = Dte2.RegistryRoot + "_Config\\Themes";
            var themes = new List<Theme>();
            string[] installedThemesKeys;
            object themeName;

            using (RegistryKey themesKey = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (themesKey != null)
                {
                    installedThemesKeys = themesKey.GetSubKeyNames();

                    foreach (string key in installedThemesKeys)
                    {
                        using (RegistryKey themeKey = themesKey.OpenSubKey(key))
                        {
                            if (themeKey != null)
                            {
                                themeName = themeKey.GetValue(null);
                                if (themeName != null)
                                {
                                    themes.Add(new Theme(key, themeKey.GetValue(null).ToString()));
                                }
                            }
                        }
                    }
                }
            }

            return themes;
        }

        /// <summary>Gets the current Visual Studio Theme.</summary>
        /// <returns>The current Visual Studio theme or null if the current theme cannot be determined.</returns>
        public static Theme GetCurrentTheme()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string keyName = string.Format(CultureInfo.InvariantCulture, @"{0}\ApplicationPrivateSettings\Microsoft\VisualStudio", Dte2.RegistryRoot);
            IEnumerable<Theme> allThemes = GetInstalledThemes();
            Theme result = null;
            string storedSetting;
            string[] settings;
            string id;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
            {
                if (key != null)
                {
                    storedSetting = (string)key.GetValue("ColorTheme", string.Empty);

                    if (!string.IsNullOrEmpty(storedSetting))
                    {
                        settings = storedSetting.Split('*');

                        if (settings.Length > 2)
                        {
                            id = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", settings[2]);
                            result = allThemes.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>Gets a theme by its id.</summary>
        /// <param name="id">The id of the theme.</param>
        /// <returns>The theme with the given <paramref name="id" /> or null if the theme does not exist.</returns>
        /// <exception cref="ArgumentNullException">Occurs if <paramref name="id" /> is null.</exception>
        public static Theme GetThemeById(string id)
        {
            IEnumerable<Theme> allThemes;

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            allThemes = GetInstalledThemes();

            return allThemes.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Applies a given <see cref="Theme" />.</summary>
        /// <param name="theme">The theme to apply.</param>
        /// <exception cref="ArgumentNullException">Occurs if <paramref name="theme" /> is null.</exception>
        public static void ApplyTheme(Theme theme)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string keyName = string.Format(CultureInfo.InvariantCulture, @"{0}\ApplicationPrivateSettings\Microsoft\VisualStudio", Dte2.RegistryRoot);

            if (theme == null)
            {
                throw new ArgumentNullException(nameof(theme));
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
            {
                if (key != null)
                {
                    key.SetValue("ColorTheme", "0*System.String*" + theme.Id.Trim('{', '}'));
                    key.SetValue("ColorThemeNew", "0*System.String*" + theme.Id);
                }
            }

            NativeMethods.SendNotifyMessage(
                new IntPtr(NativeMethods.HWND_BROADCAST),
                NativeMethods.WM_SYSCOLORCHANGE,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        /// <summary>Applies the Visual Studio theme corresponding to the current Windows 10 light/dark theme.</summary>
        public static void ApplyCurrentOsTheme()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                IThemeSwitcherOptions options = Settings.ThemeSwitcherOptions;
                Theme themeToSwitchTo = GetThemeById(OsInLightTheme ? options.Theme1Id : options.Theme2Id);
                if (!themeToSwitchTo.Equals(GetCurrentTheme()))
                {
                    ApplyTheme(themeToSwitchTo);
                }
            }
            catch (ArgumentNullException e)
            {
                DialogHelper.ShowMessageDialog(
                    Resources.Auto_Theme_Switcher_for_Win10_Dark_Mode_extension_,

                    Resources.Please_go_to_Tools_Options_Auto_Theme_Switcher_for_Win10_and_check_the_settings_ + NewLine
                    + NewLine
                    + Resources.Exception_ + NewLine
                    + e.Message,

                    OLEMSGICON.OLEMSGICON_INFO);
            }
            catch (NotSupportedException e)
            {
                DialogHelper.ShowMessageDialog(
                    Resources.Auto_Theme_Switcher_for_Win10_Dark_Mode_extension_error_,

                    Resources.This_extension_requires_Windows_10_Build_14393_or_above_ + NewLine
                    + NewLine
                    + Resources.Exception_ + NewLine
                    + e.Message,

                    OLEMSGICON.OLEMSGICON_CRITICAL);
            }
            catch (Exception e)
            {
                DialogHelper.ShowMessageDialog(
                    Resources.Auto_Theme_Switcher_for_Win10_Dark_Mode_extension_error_,

                    Resources.Exception_ + NewLine
                    + e.Message,

                    OLEMSGICON.OLEMSGICON_CRITICAL);
            }
        }
        
        #endregion
    }
}
