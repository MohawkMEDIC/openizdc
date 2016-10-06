using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Mobile.Core.Services;
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
            var oldNegated = underTest.IsNegated;
            Thread.Sleep(1000);
            var afterUpdate = base.DoTestUpdate(afterTest, "IsNegated");
            Assert.AreNotEqual(oldNegated, afterUpdate.IsNegated);
            Assert.AreNotEqual(oldVerison, afterUpdate.VersionKey);
            Assert.AreEqual(oldVerison, afterUpdate.PreviousVersionKey);
            Assert.AreNotEqual(oldCt, afterUpdate.CreationTime);
        }

        /// <summary>
        /// Tests the obsoletion of an act
        /// </summary>
        [TestMethod]
        public void TestObsoleteAct()
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
                ClassConceptKey = ActClassKeys.Act,
                StatusConceptKey = StatusKeys.Active
            };

            // Insert the object 
            var dataPersister = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();
            dataPersister.Insert(underTest);
            var afterInsert = dataPersister.Get(underTest.Key.Value);
            Assert.AreEqual(StatusKeys.Active, afterInsert.StatusConcept.Key);

            // Obsolete the object
            dataPersister.Obsolete(afterInsert);
            var afterObsolete = dataPersister.Get(afterInsert.Key.Value);
            Assert.AreEqual(StatusKeys.Obsolete, afterObsolete.StatusConcept.Key);
            Assert.IsNotNull(afterObsolete.PreviousVersion);
        }

        /// <summary>
        /// Test inserting an act with extensions
        /// </summary>
        [TestMethod]
        public void TestActExtensions()
        {
            Act underTest = new Act()
            {
                ActTime = DateTime.Now,
                ClassConceptKey = ActClassKeys.CareProvision,
                Extensions = new List<ActExtension>()
                {
                    new ActExtension(ActClassKeys.CareProvision, new byte[] { 22, 23, 24 })
                },
                MoodConceptKey = ActMoodKeys.Intent
            };
            var afterInsert = base.DoTestInsert(underTest);
            Assert.AreEqual(ActClassKeys.CareProvision, afterInsert.ClassConcept.Key);
            Assert.AreEqual(1, underTest.Extensions.Count);
            Assert.AreEqual(ActClassKeys.CareProvision, underTest.Extensions[0].ExtensionTypeKey);
            Assert.AreEqual(23, underTest.Extensions[0].ExtensionValue[1]);
        }

        /// <summary>
        /// Test recording of notes attached to an act
        /// </summary>
        [TestMethod]
        public void TestActNotes()
        {
            Act underTest = new Act()
            {
                ActTime = DateTime.Now,
                ClassConceptKey = ActClassKeys.Observation,
                Notes = new List<ActNote>()
                {
                    new ActNote(Guid.Parse("C96859F0-043C-4480-8DAB-F69D6E86696C"), "This is a test note")
                },
                MoodConceptKey = ActMoodKeys.Intent
            };
            var afterInsert = base.DoTestInsert(underTest);
            Assert.AreEqual(ActClassKeys.Observation, afterInsert.ClassConcept.Key);
            Assert.AreEqual(1, underTest.Notes.Count);
            Assert.AreEqual("c96859f0-043c-4480-8dab-f69d6e86696c", underTest.Notes[0].AuthorKey.ToString());
            Assert.AreEqual("This is a test note", underTest.Notes[0].Text);

        }

        /// <summary>
        /// Test insertion of an identified act
        /// </summary>
        [TestMethod]
        public void TestInsertIdentifiedAct()
        {
            Act underTest = new Act()
            {
                ActTime = DateTime.Now,
                ClassConceptKey = ActClassKeys.Observation,
                MoodConceptKey = ActMoodKeys.Intent,
                Identifiers = new List<ActIdentifier>()
                {
                    new ActIdentifier(new AssigningAuthority("TIIS", "TIIS", "1.2.3.4.5.5.64.4343"), "10293")
                }
            };
            var afterInsert = base.DoTestInsert(underTest);
            afterInsert.SetDelayLoad(true);
            Assert.AreEqual(ActClassKeys.Observation, afterInsert.ClassConcept.Key);
            Assert.AreEqual(1, underTest.Identifiers.Count);
            Assert.AreEqual("10293", underTest.Identifiers[0].Value);
            Assert.AreEqual("TIIS", underTest.Identifiers[0].Authority.DomainName);

        }

        /// <summary>
        /// Test query by class and mood code
        /// </summary>
        [TestMethod]
        public void TestQueryActByClassMood()
        {
            Act underTest = new Act()
            {
                ActTime = DateTime.Now,
                ClassConceptKey = ActClassKeys.Inform,
                MoodConceptKey = ActMoodKeys.Intent,
                Identifiers = new List<ActIdentifier>()
                {
                    new ActIdentifier(new AssigningAuthority("TIIS", "TIIS", "1.2.3.4.5.5.64.4343"), "10293")
                }
            };

            var afterTest = base.DoTestInsert(underTest);
            String cls = underTest.ClassConcept.Mnemonic,
                mood = underTest.MoodConcept.Mnemonic;
            base.DoTestQuery(o => o.ClassConceptKey == ActClassKeys.Inform && o.MoodConceptKey == ActMoodKeys.Intent, afterTest.Key);
            base.DoTestQuery(o => o.ClassConcept.Mnemonic == cls && o.MoodConcept.Mnemonic == mood, afterTest.Key);

        }

        /// <summary>
        /// Query by a participant's identifier
        /// </summary>
        [TestMethod]
        public void TestQueryActByParticipant()
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
            String encounterText = underTest.ClassConcept.Mnemonic,
                rctText = afterTest.Participations[0].ParticipationRole.Mnemonic;
            Guid playerEntity = underTest.Participations[0].PlayerEntity.Key.Value;

            // Query: Give me all acts with a record target
            base.DoTestQuery(o => o.Participations.Any(p => p.ParticipationRole.Mnemonic == rctText), underTest.Key);
            // Query: Give me all acts in which Patient XXXX is a participant
            base.DoTestQuery(o => o.Participations.Any(p => p.PlayerEntity.Key == playerEntity), underTest.Key);
            // Query: Give me all the acts in which patient XX is the record target
            base.DoTestQuery(o => o.Participations.Where(guard => guard.ParticipationRole.Mnemonic == "RecordTarget").Any(p => p.PlayerEntity.Key == playerEntity), underTest.Key);
        }

        /// <summary>
        /// Query Act by identifier
        /// </summary>
        [TestMethod]
        public void TestQueryActByIdentifier()
        {

            var underTest = new Act() {
                ActTime = DateTime.Now,
                ClassConceptKey = ActClassKeys.Observation,
                MoodConceptKey = ActMoodKeys.Intent,
                Identifiers = new List<ActIdentifier>()
                {
                    new ActIdentifier(new AssigningAuthority("TIIS", "TIIS", "1.2.3.4.5.5.64.4343"), "10294")
                }
            };

            base.DoTestInsert(underTest);
            // Query: Give me all acts created by TIIS
            base.DoTestQuery(o => o.Identifiers.Any(a => a.Authority.DomainName == "TIIS"), underTest.Key);
            // Query: Give me exactly act 10294 created by TIIS
            base.DoTestQuery(o => o.Identifiers.Where(guard => guard.Authority.DomainName == "TIIS").Any(i => i.Value == "10294"), underTest.Key);
        }
    }
}
