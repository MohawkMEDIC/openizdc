using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Services;
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
                TypeConcept = new Concept()
                {
                    Key = Guid.NewGuid(), // Some random concept for the "Type",
                    Mnemonic = "TEST_CHAIN_INSERT",
                    CreatedByKey = Guid.Empty,
                    IsSystemConcept = false,
                    StatusConceptKey = StatusKeys.Active
                }
            };
            strawberry.Names.Add(new EntityName(NameUseKeys.Assigned, "Strawberries"));

            var afterTest = base.DoTestInsert(strawberry);
            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Described, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Food, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "Strawberries")));
        }

        /// <summary>
        /// Test the insert of an identified entity
        /// </summary>
        [TestMethod]
        public void TestInsertIdentifiedEntity()
        {
            var mid = String.Format("urn:uuid:{0}", Guid.NewGuid());
            Entity organization = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Organization,
                StatusConceptKey = StatusKeys.Active,
                TypeConcept = new Concept()
                {
                    Key = Guid.NewGuid(),
                    Mnemonic = "Public Institution"
                },
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier()
                    {
                        Authority = new AssigningAuthority()
                        {
                            Name = "InfomanIdentifier",
                            Oid = "1.2.3.4.6.7.8.9",
                            Key = Guid.NewGuid(),
                            DomainName = "OINFMAN"
                        },
                        Value = mid
                    }
                },
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "HAMILTON HEALTH SCIENCES"),
                    new EntityName(NameUseKeys.Search, "HHS")
                },
                Addresses = new List<EntityAddress>()
                {
                    new EntityAddress(AddressUseKeys.Direct, "123 Main Street West", "Hamilton", "ON", "CA", "L8K5N2")
                }
            };

            var afterTest = base.DoTestInsert(organization);
            Assert.AreEqual(2, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Specific, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Organization, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "HHS")));
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "HAMILTON HEALTH SCIENCES")));
            Assert.AreEqual(1, afterTest.Addresses.Count);
            Assert.AreEqual(5, afterTest.Addresses[0].Component.Count);
            Assert.AreEqual(AddressUseKeys.Direct, afterTest.Addresses[0].AddressUseKey);
            Assert.AreEqual(1, afterTest.Identifiers.Count);
            Assert.AreEqual("1.2.3.4.6.7.8.9", afterTest.Identifiers[0].Authority.Oid);
            Assert.AreEqual("OINFMAN", afterTest.Identifiers[0].Authority.DomainName);
            Assert.AreEqual(mid, afterTest.Identifiers[0].Value);

            // Test lookup by identifier
            var lookupId = base.DoTestQuery(o => o.Identifiers.Any(i => i.Value == mid && i.Authority.Oid == "1.2.3.4.6.7.8.9"), afterTest.Key);



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

        /// <summary>
        /// Obsolete an entity
        /// </summary>
        [TestMethod]
        public void TestObsoleteEntity()
        {
            Entity toBeKilled = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Food,
                StatusConceptKey = StatusKeys.Active,
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            toBeKilled.Names.Add(new EntityName(NameUseKeys.Assigned, "Kill Me!"));

            var afterTest = base.DoTestInsert(toBeKilled);
            var id = afterTest.Key;
            Assert.AreEqual(StatusKeys.Active, afterTest.StatusConcept.Key);

            // Obsolete
            var idp = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();
            var afterObsolete = idp.Obsolete(afterTest);
            afterObsolete = idp.Get(id);

            // Assert
            Assert.IsNotNull(afterObsolete.ObsoletionTime);
            Assert.IsNotNull(afterObsolete.ObsoletedBy);
            Assert.AreEqual(StatusKeys.Obsolete, afterObsolete.StatusConcept.Key);


        }

        /// <summary>
        /// Test query of data by name
        /// </summary>
        [TestMethod]
        public void TestQueryByName()
        {
            Entity toBeQueried = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Described,
                ClassConceptKey = EntityClassKeys.Place,
                StatusConceptKey = StatusKeys.Active,
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            toBeQueried.Names.Add(new EntityName(NameUseKeys.Assigned, "Some Clinic"));

            var afterTest = base.DoTestInsert(toBeQueried);
            var id = afterTest.Key;
            Assert.AreEqual(StatusKeys.Active, afterTest.StatusConcept.Key);

            // Query 
            var query = base.DoTestQuery(o => o.CreationTime > DateTime.MinValue && o.ClassConcept.Key == EntityClassKeys.Place && o.Names.Any(n => n.NameUse.Key == NameUseKeys.Assigned && n.Component.Any(c => c.Value == "Some Clinic")), id);
            Assert.AreEqual(1, query.Count());

            // No results
            var idp = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();
            query = idp.Query(o => o.CreationTime < DateTime.MinValue && o.ClassConcept.Key == EntityClassKeys.Place && o.Names.Any(n => n.NameUse.Key == NameUseKeys.Assigned && n.Component.Any(c => c.Value == "Some Clinic")));
            Assert.AreEqual(0, query.Count());

            // One result (like)
            query = idp.Query(o => o.Names.Any(n=>n.Component.Any(c => c.Value.Contains("Clinic"))));
            Assert.AreEqual(1, query.Count());

        }

        [TestMethod]
        public void TestRelateTwoEntities()
        {
            Entity e1 = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Organization,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "PARENT")
                }
            }, e2 = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Place,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "CHILD")
                }
            };

            // Associate: PARENT > CHILD
            e1.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.OwnedEntity, e2));

            // Persist
            var afterInsert = base.DoTestInsert(e1);
            Assert.AreEqual(EntityClassKeys.Organization, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Relationships.Count);
            Assert.AreEqual(EntityRelationshipTypeKeys.OwnedEntity, afterInsert.Relationships[0].RelationshipTypeKey);
            Assert.AreEqual(EntityClassKeys.Place, afterInsert.Relationships[0].TargetEntity.ClassConceptKey);

        }

        /// <summary>
        /// Test persisting an entity with telecom address
        /// </summary>
        [TestMethod]
        public void TestEntityWithTelecom()
        {
            Entity e1 = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Organization,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "GOOD HEALTH HOSPITAL SYSTEM")
                }
            };
            e1.Telecoms.Add(new EntityTelecomAddress(AddressUseKeys.WorkPlace, "mailto:bob@bob.com"));
            
            // Persist
            var afterInsert = base.DoTestInsert(e1);
            Assert.AreEqual(EntityClassKeys.Organization, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Telecoms.Count);
            Assert.AreEqual(AddressUseKeys.WorkPlace, afterInsert.Telecoms[0].AddressUseKey);
            Assert.AreEqual("mailto:bob@bob.com", afterInsert.Telecoms[0].Value);

        }

        /// <summary>
        /// Tests an entity with an extended attribute
        /// </summary>
        [TestMethod]
        public void TestEntityExtensions()
        {
            Entity person = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Person,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Smith", "Baby")
                }
            };
            person.Extensions.Add(new EntityExtension()
            {
                ExtensionType = new ExtensionType()
                {
                    Name = "http://openiz.org/oiz/birthcertificate",
                    ExtensionHandler = typeof(EntityPersistenceServiceTest)
                },
                ExtensionValue = new byte[] { 1 }
            });

            var afterInsert = base.DoTestInsert(person);
            Assert.AreEqual(EntityClassKeys.Person, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Extensions.Count);
            Assert.AreEqual(typeof(EntityPersistenceServiceTest), person.Extensions[0].ExtensionType.ExtensionHandler);
            Assert.IsTrue(BitConverter.ToBoolean(person.Extensions[0].ExtensionValue, 0));
        }
    }
}
