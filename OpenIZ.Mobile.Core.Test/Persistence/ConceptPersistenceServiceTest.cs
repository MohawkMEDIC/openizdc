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
 * Date: 2016-6-14
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Services;
using System.Linq;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
	/// <summary>
	/// Concept persistence test
	/// </summary>
	[TestClass]
	public class ConceptPersistenceServiceTest : PersistenceTest<Concept>
	{
		[ClassInitialize]
		public static void ClassSetup(TestContext context)
		{
			ApplicationContext.Current = new TestApplicationContext();

			((TestApplicationContext)ApplicationContext.Current).UnitTestContext = context;
		}

		/// <summary>
		/// Tests that the concept persistence service can persist a
		/// simple concept which has a display name
		/// </summary>
		[TestMethod]
		public void TestInsertNamedConcept()
		{
			Concept namedConcept = new Concept()
			{
				ClassKey = ConceptClassKeys.Other,
				IsSystemConcept = false,
				Mnemonic = "TESTCODE2"
			};

			// Names
			namedConcept.ConceptNames.Add(new ConceptName()
			{
				Name = "Test Code",
				Language = "en",
				PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
				PhoneticCode = "E"
			});

			// Insert
			var afterTest = base.DoTestInsert(namedConcept);
			Assert.AreEqual("TESTCODE2", afterTest.Mnemonic);
			//            Assert.AreEqual("Other", afterTest.Class.Mnemonic);
			Assert.IsFalse(afterTest.IsSystemConcept);
			Assert.AreEqual(1, afterTest.ConceptNames.Count);
			Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
			Assert.AreEqual("Test Code", afterTest.ConceptNames[0].Name);
			Assert.AreEqual("E", afterTest.ConceptNames[0].PhoneticCode);
		}

		/// <summary>
		/// Tests that the concept persistence service can successfully
		/// insert and retrieve a concept
		/// </summary>
		[TestMethod]
		public void TestInsertSimpleConcept()
		{
			Concept simpleConcept = new Concept()
			{
				ClassKey = ConceptClassKeys.Other,
				IsSystemConcept = true,
				Mnemonic = "TESTCODE1"
			};
			var afterTest = base.DoTestInsert(simpleConcept);
			Assert.AreEqual("TESTCODE1", afterTest.Mnemonic);
			Assert.IsTrue(afterTest.IsSystemConcept);
		}

		/// <summary>
		/// Tests that the concept persistence service can persist a
		/// simple concept which has a display name
		/// </summary>
		[TestMethod]
		public void TestSearchNamedConcept()
		{
			Concept namedConcept = new Concept()
			{
				ClassKey = ConceptClassKeys.Other,
				IsSystemConcept = false,
				Mnemonic = "TESTCODE4"
			};

			// Names
			namedConcept.ConceptNames.Add(new ConceptName()
			{
				Name = "Test Code 1",
				Language = "en",
				PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
				PhoneticCode = "E"
			});
			namedConcept.ConceptNames.Add(new ConceptName()
			{
				Name = "Test Code 1",
				Language = "en",
				PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
				PhoneticCode = "E"
			});

			// Insert
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
			var afterTest = persistenceService.Insert(namedConcept);

			Assert.AreEqual("TESTCODE4", afterTest.Mnemonic);
			Assert.AreEqual("Other", afterTest.Class.Mnemonic);
			Assert.IsFalse(afterTest.IsSystemConcept);
			Assert.AreEqual(2, afterTest.ConceptNames.Count);
			Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
			Assert.IsTrue(afterTest.ConceptNames.Exists(n => n.Name == "Test Code 1"));
			Assert.AreEqual("E", afterTest.ConceptNames[0].PhoneticCode);
			Assert.IsNotNull(afterTest.CreatedBy);

			var originalId = afterTest.VersionKey;

			this.DoTestQuery(o => o.ConceptNames.Any(p => p.Name == "Test Code 1"), afterTest.Key);
		}

		/// <summary>
		/// Tests that the concept persistence service can persist a
		/// simple concept which has a display name
		/// </summary>
		[TestMethod]
		public void TestUpdateNamedConcept()
		{
			Concept namedConcept = new Concept()
			{
				ClassKey = ConceptClassKeys.Other,
				IsSystemConcept = false,
				Mnemonic = "TESTCODE3"
			};

			// Names
			namedConcept.ConceptNames.Add(new ConceptName()
			{
				Name = "Test Code 1",
				Language = "en",
				PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
				PhoneticCode = "E"
			});
			namedConcept.ConceptNames.Add(new ConceptName()
			{
				Name = "Test Code 2",
				Language = "en",
				PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
				PhoneticCode = "E"
			});

			// Insert
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
			var afterTest = persistenceService.Insert(namedConcept);

			Assert.AreEqual("TESTCODE3", afterTest.Mnemonic);
			Assert.AreEqual("Other", afterTest.Class.Mnemonic);
			Assert.IsFalse(afterTest.IsSystemConcept);
			Assert.AreEqual(2, afterTest.ConceptNames.Count);
			Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
			Assert.IsTrue(afterTest.ConceptNames.Exists(n => n.Name == "Test Code 1"));
			Assert.AreEqual("E", afterTest.ConceptNames[0].PhoneticCode);
			Assert.IsNotNull(afterTest.CreatedBy);

			var originalId = afterTest.VersionKey;

			// Step 1: Test an ADD of a name
			afterTest.ConceptNames.Add(new ConceptName()
			{
				Name = "Test Code 3",
				Language = "en",
				PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
				PhoneticCode = "E"
			});
			afterTest.Mnemonic = "TESTCODE3_A";
			afterTest = persistenceService.Update(afterTest);
			Assert.AreEqual(3, afterTest.ConceptNames.Count);
			Assert.AreEqual("TESTCODE3_A", afterTest.Mnemonic);
			//Assert.IsNotNull(afterTest.PreviousVersion);
			Assert.AreEqual(originalId, afterTest.PreviousVersionKey);
			var updateKey = afterTest.VersionKey;

			// Verify 2: Remove a name
			afterTest.ConceptNames.RemoveAt(1);
			afterTest.ConceptNames[0].Language = "fr";
			afterTest = persistenceService.Update(afterTest);
			Assert.AreEqual(2, afterTest.ConceptNames.Count);
			Assert.IsTrue(afterTest.ConceptNames.Exists(n => n.Language == "fr"));
			//Assert.IsNotNull(afterTest.PreviousVersion);
			Assert.AreEqual(updateKey, afterTest.PreviousVersionKey);
			//Assert.IsNotNull(afterTest.PreviousVersion.PreviousVersion);
			//Assert.AreEqual(originalId, afterTest.PreviousVersion.PreviousVersionKey);
		}
	}
}