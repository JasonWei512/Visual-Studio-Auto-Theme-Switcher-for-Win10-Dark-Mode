using VS_Auto_Theme_Switcher_for_Win10_Dark_Mode.Logic;
using VS_Auto_Theme_Switcher_for_Win10_Dark_Mode.Options;
using Microsoft.VisualStudio.Shell;
using System;

namespace VS_Auto_Theme_Switcher_for_Win10_Dark_Mode.Options
{
    /// <summary>
    /// Provides extension settings.
    /// </summary>
    internal static class Settings
    {
        private static Package thisPackage;

        /// <summary>
        /// Initialize the settings. 
        /// Call this when initializing the extension package and before getting any settings from this class.
        /// </summary>
        /// <param name="package">The extension package.</param>
        internal static void Initialize(Package package)
        {
            thisPackage = package;
        }

        /// <summary>Theme switcher options.</summary>
        /// <exception cref="InvalidOperationException">Occurs when the settings class has not been initialized yet.</exception>
        internal static IThemeSwitcherOptions ThemeSwitcherOptions
        {
            get
            {
                if (thisPackage == null)
                {
                    throw new InvalidOperationException("The settings class has not been initialized yet.");
                }

                var optionsPage = (ThemeSwitcherOptionsDialogPage)thisPackage.GetDialogPage(typeof(ThemeSwitcherOptionsDialogPage));
                optionsPage.LoadSettingsFromStorage();
                return optionsPage;
            }
        }
    }
}
