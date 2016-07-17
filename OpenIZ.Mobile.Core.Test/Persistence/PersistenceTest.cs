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
        public TModel DoTestUpdate(TModel objectUnderTest, String propertyToChange)
        {

            // Auth context

            // Store user
            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            // Update the user
            var objectAfterInsert = persistenceService.Insert(objectUnderTest);

            // Update
            var propertyInfo = typeof(TModel).GetProperty(propertyToChange);
            Object oldValue = propertyInfo.GetValue(objectUnderTest);

            if (propertyInfo.PropertyType == typeof(String))
                propertyInfo.SetValue(objectAfterInsert, "NEW_VALUE");
            else if (propertyInfo.PropertyType == typeof(Nullable<DateTimeOffset>) ||
                propertyInfo.PropertyType == typeof(DateTimeOffset))
                propertyInfo.SetValue(objectAfterInsert, DateTimeOffset.MaxValue);
            else if (propertyInfo.PropertyType == typeof(Boolean) ||
                propertyInfo.PropertyType == typeof(Nullable<Boolean>))
                propertyInfo.SetValue(objectAfterInsert, true);

            var objectAfterUpdate = persistenceService.Update(objectAfterInsert);
            Assert.AreEqual(objectAfterInsert.Key, objectAfterUpdate.Key);
            objectAfterUpdate = persistenceService.Get(objectAfterUpdate.Key.Value);
            // Update attributes should be set
            Assert.AreNotEqual(oldValue, propertyInfo.GetValue(objectAfterUpdate));
            Assert.AreEqual(objectAfterInsert.Key, objectAfterUpdate.Key);

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
