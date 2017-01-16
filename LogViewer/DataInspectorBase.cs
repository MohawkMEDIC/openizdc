namespace LogViewer
{
    /// <summary>
    /// Data inspector
    /// </summary>
    public abstract class DataInspectorBase
    {

        /// <summary>
        /// Get the name of the inspector
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Inspect
        /// </summary>
        public abstract string Inspect(string source);

        /// <summary>
        /// Represent as string
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }
    }
}