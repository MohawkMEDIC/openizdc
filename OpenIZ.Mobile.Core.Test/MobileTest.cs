using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// Represents a simple mobile test which sets up context
    /// </summary>
    public abstract class MobileTest
    {

        /// <summary>
        /// Mobile test
        /// </summary>
        public MobileTest()
        {
            ApplicationContext.Current = new TestApplicationContext();
        }
    }
}