/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-11-14
 */
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
			ApplicationContext.Current = new TestApplicationContext();

			((TestApplicationContext)ApplicationContext.Current).UnitTestContext = context;
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