/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-6-28
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
	/// <summary>
	/// Tests the persistence service which is responsible for the persistence of user entities
	/// </summary>
	[TestClass]
	public class UserEntityPersistenceServiceTest : PersistenceTest<UserEntity>
	{
		[ClassInitialize]
		public static void ClassSetup(TestContext context)
		{
			ApplicationContext.Current = new TestApplicationContext();

			((TestApplicationContext)ApplicationContext.Current).UnitTestContext = context;
		}

		/// <summary>
		/// Tests the insert of a user entity for a user
		/// </summary>
		[TestMethod]
		public void TestInsertUserEntity()
		{
			var userEntity = new UserEntity()
			{
				DateOfBirth = DateTime.Now,
				LanguageCommunication = new System.Collections.Generic.List<PersonLanguageCommunication>()
				{
					new PersonLanguageCommunication("en", true),
					new PersonLanguageCommunication("fr", false)
				},
				Identifiers = new System.Collections.Generic.List<OpenIZ.Core.Model.DataTypes.EntityIdentifier>()
				 {
					 new OpenIZ.Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority() { DomainName = "MyDomain", Oid = "1.2.3.4.5.6", Name ="Test Domain" }, "12039232-2302")
				 },
				Names = new System.Collections.Generic.List<EntityName>()
				 {
					 new EntityName(NameUseKeys.OfficialRecord, "Smith", "Bob")
				 },
				SecurityUser = new OpenIZ.Core.Model.Security.SecurityUser()
				{
					Email = "mailto:test@test.com",
					EmailConfirmed = true,
					PhoneNumber = "tel:+1203203920394",
					PhoneNumberConfirmed = true,
					UserName = "smithb",
					SecurityHash = Guid.NewGuid().ToString(),
					PasswordHash = "xxxx"
				}
			};

			// Insert
			var afterInsert = base.DoTestInsert(userEntity);
			Assert.AreEqual(2, afterInsert.LanguageCommunication.Count);
			Assert.AreEqual(1, afterInsert.Identifiers.Count);
			Assert.AreEqual(1, afterInsert.Names.Count);
			Assert.AreEqual("Smith", afterInsert.Names[0].Component[0].Value);
			Assert.IsNotNull(afterInsert.SecurityUser);
			Assert.AreEqual(afterInsert.SecurityUser.UserName, "smithb");
		}
	}
}