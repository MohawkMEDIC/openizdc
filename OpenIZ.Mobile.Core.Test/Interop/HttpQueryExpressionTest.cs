using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Mobile.Core.Http;
using OpenIZ.Mobile.Core.Interop.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Test.Interop
{
    /// <summary>
    /// Tests for the HTTP expression writer
    /// </summary>
    [TestClass]
    public class HttpQueryExpressionTest
    {

        /// <summary>
        /// Test query by key
        /// </summary>
        [TestMethod]
        public void TestWriteLookupByKey()
        {
            Guid id = Guid.Empty;
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.Key == id);
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("id=00000000-0000-0000-0000-000000000000", expression);
        }

        /// <summary>
        /// Test query by key
        /// </summary>
        [TestMethod]
        public void TestWriteLookupAnd()
        {
            Guid id = Guid.Empty;
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.Key == id && o.GenderConcept.Mnemonic == "Male");
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("id=00000000-0000-0000-0000-000000000000&genderConcept.mnemonic=Male", expression);
        }

        /// <summary>
        /// Test query by or
        /// </summary>
        [TestMethod]
        public void TestWriteLookupOr()
        {
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.GenderConcept.Mnemonic == "Male" || o.GenderConcept.Mnemonic == "Female");
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("genderConcept.mnemonic=Male&genderConcept.mnemonic=Female", expression);
        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupGreaterThanEqual()
        {
            DateTime dt = DateTime.MinValue;
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth >= dt);
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3E%3D0001-01-01T00%3A00%3A00.0000000", expression);

        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupGreaterThan()
        {
            DateTime dt = DateTime.MinValue;
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth > dt);
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3E0001-01-01T00%3A00%3A00.0000000", expression);

        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupLessThanEqual()
        {
            DateTime dt = DateTime.MinValue;
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth <= dt);
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3C%3D0001-01-01T00%3A00%3A00.0000000", expression);

        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupLessThan()
        {
            DateTime dt = DateTime.MinValue;
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.DateOfBirth < dt);
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("dateOfBirth=%3C0001-01-01T00%3A00%3A00.0000000", expression);

        }

        /// <summary>
        /// Test write of lookup greater than equal to
        /// </summary>
        [TestMethod]
        public void TestWriteLookupNotEqual()
        {
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.GenderConcept.Mnemonic != "Male");
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("genderConcept.mnemonic=%21Male", expression);
        }

        /// <summary>
        /// Test write of lookup approximately
        /// </summary>
        [TestMethod]
        public void TestWriteLookupApproximately()
        {
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.GenderConcept.Mnemonic.Contains("M"));
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("genderConcept.mnemonic=~M", expression);
        }

        /// <summary>
        /// Test write of Any correctly
        /// </summary>
        [TestMethod]
        public void TestWriteLookupAny()
        {
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.Names.Any(p=>p.NameUse.Mnemonic == "Legal"));
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("name.use.mnemonic=Legal", expression);
        }

        /// <summary>
        /// Test write of Any correctly
        /// </summary>
        [TestMethod]
        public void TestWriteLookupAnyAnd()
        {
            var query = HttpQueryExpressionBuilder.BuildQuery<Patient>(o => o.Names.Any(p => p.NameUse.Mnemonic == "Legal" && p.Component.Any(n=>n.Value == "Smith")));
            var expression = RestClientBase.CreateQueryString(query.ToArray());
            Assert.AreEqual("name.use.mnemonic=Legal&name.component.value=Smith", expression);
        }
    }
}
