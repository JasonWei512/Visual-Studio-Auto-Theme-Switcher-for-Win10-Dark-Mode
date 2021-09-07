using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace VsAutoThemeSwitcherForWin10DarkMode.Helpers
{
    /// <summary>
    /// A helper class to show dialogs in Visual Studio.
    /// </summary>
    public static class DialogHelper
    {
        /// <summary>
        /// Show a message dialog.
        /// </summary>
        /// <param name="title">The title of the message dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="icon">The icon to show on the message dialog.</param>
        public static void ShowMessageDialog(string title, string message, OLEMSGICON icon)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            VsShellUtilities.ShowMessageBox(
                ServiceProvider.GlobalProvider,
                message,
                title,
                icon,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
