using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using VsAutoThemeSwitcherForWin10DarkMode.Helpers;
using VsAutoThemeSwitcherForWin10DarkMode.Logic;
using VsAutoThemeSwitcherForWin10DarkMode.Options;
using VsAutoThemeSwitcherForWin10DarkMode.Properties;
using Task = System.Threading.Tasks.Task;

namespace VsAutoThemeSwitcherForWin10DarkMode
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(ThemeSwitcherOptionsDialogPage), "Auto Theme Switcher for Win10", "General", 100, 101, false)]
    public sealed class VsAutoThemeSwitcherForWin10DarkModePackage : AsyncPackage
    {
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            Settings.Initialize(this);

            if (ThemeManager.OsSupportsDarkTheme)
            {
                await Task.Delay(100);    // Wait for 0.1s or the extension will crash
                ThemeManager.ApplyCurrentOsTheme();

                RegistryMonitor registryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                registryMonitor.RegChangeNotifyFilter = RegChangeNotifyFilter.Value;
                registryMonitor.RegChanged += RegistryMonitor_RegChanged;
                registryMonitor.Start();
            }
            else
            {
                DialogHelper.ShowMessageDialog(
                    Resources.Auto_Theme_Switcher_for_Win10_Dark_Mode_extension_error_,
                    Resources.This_extension_requires_Windows_10_Build_14393_or_above_,
                    OLEMSGICON.OLEMSGICON_CRITICAL);
            }
        }

        private async void RegistryMonitor_RegChanged(object sender, EventArgs e)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            ThemeManager.ApplyCurrentOsTheme();
        }

        #endregion
    }
}
