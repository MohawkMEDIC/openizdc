using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Entities;
using System.Linq;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Mobile.Core.Test.ViewModel
{
    /// <summary>
    /// This test ensures that the new view model serializer only serializes the specified properties
    /// in the view model context
    /// </summary>
    [TestClass]
    public class ViewModelDelayLoadTests : MobileTest
    {

        // View model
        private ViewModelDescription m_viewModel;

        /// <summary>
        /// View model delay load tests
        /// </summary>
        public ViewModelDelayLoadTests()
        {
            this.m_viewModel = ViewModelDescription.Load(typeof(ViewModelDelayLoadTests).Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Test.Resources.ViewModel.xml"));
        }

        /// <summary>
        /// Class setup
        /// </summary>
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            ApplicationContext.Current = new TestApplicationContext();
            ((TestApplicationContext)ApplicationContext.Current).UnitTestContext = context;
        }

        /// <summary>
        /// Tests that the view model serializer delay loads an appropriate amount of data
        /// </summary>
        [TestMethod]
        public void TestDelayLoadPlaceParentOnly()
        {

            var idp = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            int tr = 0;
            var place = idp.Query(o => o.Names.Any(n => n.Component.Any(c => c.Value == "Imbaseni")) && o.ClassConceptKey == EntityClassKeys.Place, 0, 1, out tr).FirstOrDefault();
            Assert.IsNotNull(place);

            // Now serialize
            var json = JsonViewModelSerializer.Serialize(place, this.m_viewModel);
            Assert.IsTrue(json.Contains("Arusha District"));

        }
    }
}
