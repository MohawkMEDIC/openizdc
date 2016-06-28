using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// Test class for places
    /// </summary>
    [TestClass]
    public class PlacePersistenceServiceTest : PersistenceTest<Place>
    {

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            (ApplicationContext.Current as TestApplicationContext).UnitTestContext = context;
        }


        /// <summary>
        /// Test insertion of a place
        /// </summary>
        [TestMethod]
        public void TestInsertPlace()
        {
            Place place = new Place()
            {
                ClassConceptKey = EntityClassKeys.ServiceDeliveryLocation,
                Addresses = new List<EntityAddress>()
                {
                    new EntityAddress(AddressUseKeys.PhysicalVisit, "123 Road", "Hamilton", "ON", "CA", "L8K5N2")
                },
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Assigned, "Good Health Clinic")
                },
                Identifiers = new List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>()
                {
                    new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority() { DomainName = "PLCID", Name = "Place IDS" }, "123942349")
                },
                IsMobile = true,
                Lat = -23.39403f,
                Lng = 1.0392f,
                Services = new List<PlaceService>()
                {
                    new PlaceService() { ServiceConcept  = new Concept()
                    {
                        ConceptNames = new List<ConceptName>() { new ConceptName() {  Name = "Immunization", Language = "en" } },
                        ClassKey = ConceptClassKeys.Other,
                        Mnemonic = "IMMUNIZ"
                        }
                    }
                }
            };

            var afterInsert = base.DoTestInsert(place);
            Assert.AreEqual(1, afterInsert.Names.Count);
            Assert.AreEqual(1, afterInsert.Addresses.Count);
            Assert.AreEqual(1, afterInsert.Identifiers.Count);
            Assert.IsTrue(afterInsert.IsMobile);
            Assert.AreEqual(-23, (int)afterInsert.Lat);
            Assert.AreEqual(1, (int)afterInsert.Lng);
            Assert.AreEqual(1, afterInsert.Services.Count);
            Assert.AreEqual("IMMUNIZ", afterInsert.Services[0].ServiceConcept.Mnemonic);
        }
    }
}
