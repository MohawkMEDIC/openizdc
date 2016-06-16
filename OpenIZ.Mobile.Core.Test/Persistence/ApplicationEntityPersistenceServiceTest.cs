using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Security;
using OpenIZ.Mobile.Core.Services;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// Tests the application user entity persistence service
    /// </summary>
    [TestClass]
    public class ApplicationEntityPersistenceServiceTest : PersistenceTest<ApplicationEntity>
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            (ApplicationContext.Current as TestApplicationContext).UnitTestContext = context;
        }

        /// <summary>
        /// Test the insertion of the application entity into the database
        /// </summary>
        [TestMethod]
        public void TestInsertApplicationEntity()
        {
            SecurityApplication securityApp = new SecurityApplication()
            {
                ApplicationSecret = "I AM A SECRET!",
                Name = "Test Application"
            };

            var iapps = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityApplication>>();
            var securityApplication = iapps.Insert(securityApp);

            // Create the application entity
            var appEntity = new ApplicationEntity()
            {
                SoftwareName = "Test Software v1",
                VersionName = "Bluenose",
                VendorName = "Some Software Company Inc.",
                SecurityApplication = securityApp
            };

            var afterInsert = base.DoTestInsert(appEntity);
            Assert.AreEqual("Test Software v1", afterInsert.SoftwareName);
            Assert.AreEqual("Bluenose", afterInsert.VersionName);
            Assert.AreEqual("Some Software Company Inc.", afterInsert.VendorName);
            Assert.IsNotNull(afterInsert.SecurityApplication);
            Assert.AreEqual("Test Application", afterInsert.SecurityApplication.Name);

        }
    }
}
