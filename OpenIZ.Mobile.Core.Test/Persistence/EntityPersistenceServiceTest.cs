using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// Test persistence of entities
    /// </summary>
    [TestClass]
    public class EntityPersistenceServiceTest : PersistenceTest<Entity>
    {

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            (ApplicationContext.Current as TestApplicationContext).UnitTestContext = context;
        }

        /// <summary>
        /// Test the insertion of an anonymous entity
        /// </summary>
        [TestMethod]
        public void TestInsertAnonymousEntity()
        {
            Entity strawberry = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Described,
                ClassConceptKey = EntityClassKeys.Food,
                StatusConceptKey = StatusKeys.Active,
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            strawberry.Names.Add(new EntityName(NameUseKeys.Assigned, "Strawberries"));

            var afterTest = base.DoTestInsert(strawberry);
            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Described, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Food, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "Strawberries")));
        }

        /// <summary>
        /// Test the insertion of an anonymous entity
        /// </summary>
        [TestMethod]
        public void TestInsertAnonymousInstanceEntity()
        {
            Entity strawberry = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Food,
                StatusConceptKey = StatusKeys.Active,
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            var st = BitConverter.ToString(EntityClassKeys.Patient.ToByteArray()).Replace("-", "") ;
            strawberry.Names.Add(new EntityName(NameUseKeys.Assigned, "Strawberries"));
            strawberry.Addresses.Add(new EntityAddress(AddressUseKeys.Direct, "123 Main Street West", "Hamilton", "ON", "Canada", "L8K5N2"));

            var afterTest = base.DoTestInsert(strawberry);
            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Specific, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Food, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "Strawberries")));
            Assert.AreEqual(1, afterTest.Addresses.Count);
            Assert.AreEqual(5, afterTest.Addresses[0].Component.Count);
            Assert.AreEqual(AddressUseKeys.Direct, afterTest.Addresses[0].AddressUseKey);
            Assert.AreEqual(NameUseKeys.Assigned, afterTest.Names[0].NameUseKey);
        }
    }
}
