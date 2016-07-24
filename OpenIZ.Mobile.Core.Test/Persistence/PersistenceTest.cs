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
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Test.Persistence
{
    /// <summary>
    /// Persistence test
    /// </summary>
    public class PersistenceTest<TModel> : MobileTest where TModel : IdentifiedData
    {

        /// <summary>
        /// Test the insertion of a valid security user
        /// </summary>
        public TModel DoTestInsert(TModel objectUnderTest)
        {


            // Store user
            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            var objectAfterTest = persistenceService.Insert(objectUnderTest);
            // Key should be set
            Assert.AreNotEqual(Guid.Empty, objectAfterTest.Key);

            // Verify
            objectAfterTest = persistenceService.Get(objectAfterTest.Key.Value);
            if (objectAfterTest is BaseEntityData)
                Assert.AreNotEqual(default(DateTimeOffset), (objectAfterTest as BaseEntityData).CreationTime);

            return objectAfterTest;
        }

        /// <summary>
        /// Do a test step for an update
        /// </summary>
        public TModel DoTestInsertUpdate(TModel objectUnderTest, String propertyToChange)
        {

            // Auth context

            // Store user
            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            // Update the user
            var objectAfterInsert = persistenceService.Insert(objectUnderTest);

            // Update
            return this.DoTestUpdate(objectAfterInsert, propertyToChange);
        }

        /// <summary>
        /// Test update only
        /// </summary>
        /// <param name="objectUnderTest"></param>
        /// <param name="propertyToChange"></param>
        /// <returns></returns>
        public TModel DoTestUpdate(TModel objectUnderTest, String propertyToChange)
        {

            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            var propertyInfo = typeof(TModel).GetProperty(propertyToChange);
            Object oldValue = propertyInfo.GetValue(objectUnderTest);

            if (propertyInfo.PropertyType == typeof(String))
                propertyInfo.SetValue(objectUnderTest, "NEW_VALUE");
            else if (propertyInfo.PropertyType == typeof(Nullable<DateTimeOffset>) ||
                propertyInfo.PropertyType == typeof(DateTimeOffset))
                propertyInfo.SetValue(objectUnderTest, DateTimeOffset.MaxValue);
            else if (propertyInfo.PropertyType == typeof(Boolean) ||
                propertyInfo.PropertyType == typeof(Nullable<Boolean>))
                propertyInfo.SetValue(objectUnderTest, !(bool)propertyInfo.GetValue(objectUnderTest));

            var objectAfterUpdate = persistenceService.Update(objectUnderTest);
            Assert.AreEqual(objectUnderTest.Key, objectAfterUpdate.Key);
            objectAfterUpdate = persistenceService.Get(objectAfterUpdate.Key.Value);
            // Update attributes should be set
            Assert.AreNotEqual(oldValue, propertyInfo.GetValue(objectAfterUpdate));
            Assert.AreEqual(objectUnderTest.Key, objectAfterUpdate.Key);

            return objectAfterUpdate;
        }

        /// <summary>
        /// Perform a query
        /// </summary>
        public IEnumerable<TModel> DoTestQuery(Expression<Func<TModel, bool>> predicate, Guid? knownResultKey)
        {

            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            // Perform query
            var results = persistenceService.Query(predicate);

            // Look for the known key
            Assert.IsTrue(results.Any(p => p.Key == knownResultKey), "Result doesn't contain known key");

            return results;
        }

    }
}
