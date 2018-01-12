/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-9-1
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System.Collections.Generic;

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
			ApplicationContext.Current = new TestApplicationContext();

			((TestApplicationContext)ApplicationContext.Current).UnitTestContext = context;
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
				IndustryConcept = new Concept()
				{
					Mnemonic = "Healthcare"
				}
			};
			var afterTest = base.DoTestInsert(org);
			Assert.AreEqual(1, afterTest.Names.Count);
			Assert.AreEqual(1, afterTest.Identifiers.Count);
			Assert.AreEqual("Healthcare", org.IndustryConcept.Mnemonic);
			Assert.AreEqual("Good Health Hospital System", org.Names[0].Component[0].Value);
		}
	}
}