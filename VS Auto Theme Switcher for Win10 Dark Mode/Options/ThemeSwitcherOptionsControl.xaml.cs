using System;
using System.Windows.Controls;

namespace VsAutoThemeSwitcherForWin10DarkMode.Options
{
    /// <summary>Interaction logic for ThemeSwitcherOptionsControl.xaml</summary>
    public partial class ThemeSwitcherOptionsControl : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instnce of the <see cref="ThemeSwitcherOptionsControl" /> class.</summary>
        /// <param name="optionsPage">The options page that should be used by the control.</param>
        /// <exception cref="ArgumentNullException">Occurs if <paramref name="optionsPage" /> is null.</exception>
        public ThemeSwitcherOptionsControl(ThemeSwitcherOptionsDialogPage optionsPage)
        {
            if (optionsPage == null)
            {
                throw new ArgumentNullException(nameof(optionsPage));
            }

            this.DataContext = optionsPage;
            this.InitializeComponent();
        }

        #endregion
    }
}
