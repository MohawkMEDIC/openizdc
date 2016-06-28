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
    /// Organization persistence test
    /// </summary>
    [TestClass]
    public class OrganizationPersistenceTest : PersistenceTest<Organization>
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            (ApplicationContext.Current as TestApplicationContext).UnitTestContext = context;
        }

        /// <summary>
        /// Test the insertion of an organization
        /// </summary>
        [TestMethod]
        public void TestInsertOrganization()
        {
            Organization org = new Organization()
            {
                Identifiers = new List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>()
                {
                    new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority() { Name = "OrgId", DomainName = "ORGID" }, "12345")
                },
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Assigned, "Good Health Hospital System")
                },
                IndustryConceptKey = UserClassKeys.ApplictionUser
            };
            var afterTest = base.DoTestInsert(org);
            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(1, afterTest.Identifiers.Count);
            Assert.AreEqual(UserClassKeys.ApplictionUser, org.IndustryConceptKey);
            Assert.AreEqual("Good Health Hospital System", org.Names[0].Component[0].Value);
        }

    }
}
