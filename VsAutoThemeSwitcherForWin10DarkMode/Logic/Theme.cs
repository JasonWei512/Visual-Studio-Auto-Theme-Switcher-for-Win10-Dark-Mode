using System.Diagnostics;

namespace VsAutoThemeSwitcherForWin10DarkMode.Logic
{
    /// <summary>Represents a Visual Studio theme.</summary>
    [DebuggerDisplay("{DisplayName}")]
    public class Theme
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Theme" /> class.</summary>
        /// <param name="id">The id of the theme.</param>
        /// <param name="displayName">The display name of the theme.</param>
        public Theme(string id, string displayName)
        {
            this.Id = id;
            this.DisplayName = displayName;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the id of the theme.</summary>
        public string Id { get; }

        /// <summary>Gets the display name of the theme.</summary>
        public string DisplayName { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Determines whether the specified <see cref="Theme" /> object is
        /// equal to the current object.</summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object;
        /// otherwise, false.</returns>
        public bool Equals(Theme other)
        {
            bool equals = false;

            if (other != null)
            {
                equals = this.Id.Equals(other.Id);
                equals &= this.DisplayName.Equals(other.DisplayName);
            }

            return equals;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals(obj as Theme);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 13;

                hashCode = (hashCode * 397) ^ this.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ this.DisplayName.GetHashCode();

                return hashCode;
            }
        }

        #endregion
    }
}
