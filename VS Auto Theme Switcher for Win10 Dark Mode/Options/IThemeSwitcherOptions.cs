namespace VS_Auto_Theme_Switcher_for_Win10_Dark_Mode.Options
{
    /// <summary>Provides acces to all options for the extension.</summary>
    internal interface IThemeSwitcherOptions
    {
        #region Public Properties

        /// <summary>Gets the id of the first theme.</summary>
        string Theme1Id { get; }

        /// <summary>Gets the id of the second theme.</summary>
        string Theme2Id { get; }

        #endregion
    }
}
