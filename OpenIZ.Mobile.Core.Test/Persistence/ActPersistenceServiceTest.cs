using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// ACT persistence 
    /// </summary>
    [TestClass]
    public class ActPersistenceServiceTest : PersistenceTest<Act>
    {

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            (ApplicationContext.Current as TestApplicationContext).UnitTestContext = context;
        }

        /// <summary>
        /// Simple test of an anonymous act
        /// </summary>
        [TestMethod]
        public void TestInsertSimpleAct()
        {
            Act underTest = new Act()
            {
                ActTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)),
                CreationTime = DateTime.Now,
                IsNegated = true,
                MoodConceptKey = ActMoodKeys.Eventoccurrence,
                ReasonConcept = new Concept()
                {
                    Mnemonic = "Testing"
                },
                TypeConcept = new Concept()
                {
                    Mnemonic = "TestCase"
                },
                ClassConceptKey = ActClassKeys.Act
            };
            var afterTest = base.DoTestInsert(underTest);
            Assert.AreEqual(underTest.ActTime, afterTest.ActTime);
            Assert.AreEqual(ActMoodKeys.Eventoccurrence, afterTest.MoodConceptKey);
            Assert.AreEqual("Eventoccurrence", afterTest.MoodConcept.Mnemonic);
            Assert.AreEqual("Act", afterTest.ClassConcept.Mnemonic);
            Assert.AreEqual("Testing", afterTest.ReasonConcept.Mnemonic);
            Assert.AreEqual("TestCase", afterTest.TypeConcept.Mnemonic);
            Assert.AreEqual(StatusKeys.New, afterTest.StatusConcept.Key);
            Assert.IsNotNull(afterTest.CreationTime);
        }

        /// <summary>
        /// Relate two acts together
        /// </summary>
        [TestMethod]
        public void TestRelateActs()
        {

            Act underTest = new Act()
            {
                ClassConceptKey = ActClassKeys.Encounter,
                StartTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0)),
                StopTime = DateTime.Now,
                IsNegated = false,
                MoodConceptKey = ActMoodKeys.Eventoccurrence,
                StatusConceptKey = StatusKeys.Active,
                TypeConcept = new Concept() { Mnemonic = "IMMUNIZ" },
                Relationships = new List<ActRelationship>()
                {
                    new ActRelationship()
                    {
                        TargetAct = new Act()
                        {
                            ClassConceptKey = ActClassKeys.SubstanceAdministration,
                            ActTime = DateTime.Now,
                            IsNegated = false,
                            MoodConceptKey = ActMoodKeys.Eventoccurrence,
                            ReasonConcept = new Concept()
                            {
                                Mnemonic = "PerProtocol"
                            },
                            StatusConceptKey = StatusKeys.Active,
                            TypeConcept = new Concept() { Mnemonic = "Immunization" }
                        },
                        RelationshipTypeKey = ActRelationshipTypeKeys.HasComponent
                    }
                }
            };
            var afterTest = base.DoTestInsert(underTest);
            Assert.AreEqual(ActClassKeys.Encounter, afterTest.ClassConcept.Key);
            Assert.AreEqual(false, afterTest.IsNegated);
            Assert.AreEqual("IMMUNIZ", afterTest.TypeConcept.Mnemonic);
            Assert.AreEqual(1, afterTest.Relationships.Count);
            Assert.AreEqual(ActClassKeys.SubstanceAdministration, afterTest.Relationships[0].TargetAct.ClassConcept.Key);
            Assert.AreEqual(ActRelationshipTypeKeys.HasComponent, afterTest.Relationships[0].RelationshipType.Key);
            Assert.AreEqual("PerProtocol", afterTest.Relationships[0].TargetAct.ReasonConcept.Mnemonic);
            Assert.IsNotNull(afterTest.StartTime);
            Assert.IsNotNull(afterTest.StopTime);
        }

        /// <summary>
        /// Test insertion of an act with participation relationships
        /// </summary>
        [TestMethod]
        public void TestActWithParticipations()
        {
            Act underTest = new Act()
            {
                ClassConceptKey = ActClassKeys.Encounter,
                StartTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0)),
                StopTime = DateTime.Now,
                IsNegated = true,
                MoodConceptKey = ActMoodKeys.Eventoccurrence,
                StatusConceptKey = StatusKeys.Active,
                TypeConcept = new Concept() { Mnemonic = "IMMUNIZ" },
                Participations = new List<ActParticipation>()
                {
                    new ActParticipation()
                    {
                        ParticipationRoleKey = ActParticipationKey.RecordTarget,
                        PlayerEntity = new Patient()
                        {
                            Names = new List<EntityName>()
                            {
                                new EntityName(NameUseKeys.OfficialRecord, "Smith", "John", "B.")
                            },
                            GenderConcept = new Concept() { Mnemonic = "Male" }
                        }
                    }
                }
            };
            var afterTest = base.DoTestInsert(underTest);
            Assert.AreEqual(ActClassKeys.Encounter, afterTest.ClassConcept.Key);
            Assert.AreEqual(true, afterTest.IsNegated);
            Assert.AreEqual("IMMUNIZ", afterTest.TypeConcept.Mnemonic);
            Assert.AreEqual(1, afterTest.Participations.Count);
            Assert.AreEqual(ActParticipationKey.RecordTarget, afterTest.Participations[0].ParticipationRole.Key);
            Assert.AreEqual(NameUseKeys.OfficialRecord, afterTest.Participations[0].PlayerEntity.Names[0].NameUseKey);
            Assert.AreEqual("Smith", afterTest.Participations[0].PlayerEntity.Names[0].Component[0].Value);
            Assert.IsNotNull(afterTest.StartTime);
            Assert.IsNotNull(afterTest.StopTime);
        }

        /// <summary>
        /// Test updating an ACT
        /// </summary>
        [TestMethod]
        public void TestUpdateAct()
        {
            Act underTest = new Act()
            {
                ActTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)),
                CreationTime = DateTime.Now,
                IsNegated = true,
                MoodConceptKey = ActMoodKeys.Eventoccurrence,
                ReasonConcept = new Concept()
                {
                    Mnemonic = "Testing"
                },
                TypeConcept = new Concept()
                {
                    Mnemonic = "TestCase"
                },
                ClassConceptKey = ActClassKeys.Act
            };
            var afterTest = base.DoTestInsert(underTest);
            Assert.AreEqual(underTest.ActTime, afterTest.ActTime);
            Assert.AreEqual(ActMoodKeys.Eventoccurrence, afterTest.MoodConceptKey);
            Assert.AreEqual("Eventoccurrence", afterTest.MoodConcept.Mnemonic);
            Assert.AreEqual("Act", afterTest.ClassConcept.Mnemonic);
            Assert.AreEqual("Testing", afterTest.ReasonConcept.Mnemonic);
            Assert.AreEqual("TestCase", afterTest.TypeConcept.Mnemonic);
            Assert.AreEqual(StatusKeys.New, afterTest.StatusConcept.Key);
            Assert.IsNotNull(afterTest.CreationTime);

            // Now update the act
            var oldVerison = afterTest.VersionKey;
            var oldCt = afterTest.CreationTime;
            Thread.Sleep(1000);
            var afterUpdate = base.DoTestUpdate(afterTest, "IsNegated");
            Assert.AreNotEqual(underTest.IsNegated, afterUpdate.IsNegated);
            Assert.AreNotEqual(oldVerison, afterUpdate.VersionKey);
            Assert.AreEqual(oldVerison, afterUpdate.PreviousVersionKey);
            Assert.AreNotEqual(oldCt, afterUpdate.CreationTime);
        }

        [TestMethod]
        public void TestObsoleteAct()
        {

        }

        [TestMethod]
        public void TestActExtensions()
        {

        }

        [TestMethod]
        public void TestActNotes()
        {

        }

        [TestMethod]
        public void TestInsertIdentifiedAct()
        {

        }

        [TestMethod]
        public void TestQueryActByClassMood()
        {

        }

        [TestMethod]
        public void TestQueryActByParticipant()
        {

        }

        [TestMethod]
        public void TestQueryActByIdentified()
        {

        }
    }
}
