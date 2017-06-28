namespace OpenIZ.Mobile.Core.Tickler
{
    /// <summary>
    /// Represents the type of tickle
    /// </summary>
    public enum TickleType
    {
        /// <summary>
        /// Represents an informational tickle, which can be dismissed by the user
        /// </summary>
        Information = 1,
        /// <summary>
        /// Represents a danger tickle
        /// </summary>
        Danger = 2,
        /// <summary>
        /// Toast
        /// </summary>
        Toast = 4,
        /// <summary>
        /// <summary>
        /// Represents a task the user must perform before the tickle can be dismissed
        /// </summary>
        Task = 8,
        /// Represents a tickle related to security
        /// </summary>
        Security = 16,
        SecurityTask = Task | Security,
        SecurityError = Danger | Security,
        SecurityInformation = Information | Security
        
    }
}