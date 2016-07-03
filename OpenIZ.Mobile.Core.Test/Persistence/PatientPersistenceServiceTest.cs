using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// Test class for patient persistence
    /// </summary>
    [TestClass]
    public class PatientPersistenceServiceTest : PersistenceTest<Patient>
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            (ApplicationContext.Current as TestApplicationContext).UnitTestContext = context;
        }

        /// <summary>
        /// Test the persistence of a person
        /// </summary>
        [TestMethod]
        public void TestPersistPatient()
        {

            Patient p = new Patient()
            {
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Johnson", "William")
                },
                Addresses = new List<EntityAddress>()
                {
                    new EntityAddress(AddressUseKeys.HomeAddress, "123 Main Street West", "Hamilton", "ON", "CA", "L8K5N2")
                },
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(new AssigningAuthority() { Name = "OHIPCARD", DomainName = "OHIPCARD", Oid = "1.2.3.4.5.6" }, "12343120423")
                },
                Telecoms = new List<EntityTelecomAddress>()
                {
                    new EntityTelecomAddress(AddressUseKeys.WorkPlace, "mailto:will@johnson.com")
                },
                Tags = new List<EntityTag>()
                {
                    new EntityTag("hasBirthCertificate", "true")
                },
                Notes = new List<EntityNote>()
                {
                    new EntityNote(Guid.NewGuid(), "William is a test patient")
                },
                GenderConceptKey = Guid.Parse("F4E3A6BB-612E-46B2-9F77-FF844D971198"),
                DateOfBirth = new DateTime(1984, 03, 22),
                MultipleBirthOrder = 2,
                DeceasedDate = new DateTime(2016,05,02),
                DeceasedDatePrecision = DatePrecision.Hour,
                DateOfBirthPrecision = DatePrecision.Day
            };

            var afterInsert = base.DoTestInsert(p);
            Assert.AreEqual(DatePrecision.Day, afterInsert.DateOfBirthPrecision);
            Assert.AreEqual(DatePrecision.Hour, afterInsert.DeceasedDatePrecision);
            Assert.AreEqual(new DateTime(1984, 03, 22), afterInsert.DateOfBirth);
            Assert.AreEqual(new DateTime(2016, 05, 02), afterInsert.DeceasedDate);
            Assert.AreEqual("Male", afterInsert.GenderConcept.Mnemonic);
            Assert.AreEqual(2, afterInsert.MultipleBirthOrder);
            Assert.AreEqual(1, p.Names.Count);
            Assert.AreEqual(1, p.Addresses.Count);
            Assert.AreEqual(1, p.Identifiers.Count);
            Assert.AreEqual(1, p.Telecoms.Count);
            Assert.AreEqual(1, p.Tags.Count);
            Assert.AreEqual(1, p.Notes.Count);
            Assert.AreEqual(EntityClassKeys.Patient, p.ClassConceptKey);
            Assert.AreEqual(DeterminerKeys.Specific, p.DeterminerConceptKey);
            Assert.AreEqual(StatusKeys.Active, p.StatusConceptKey);
        }
    }
}
