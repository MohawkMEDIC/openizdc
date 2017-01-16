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
 * Date: 2016-10-25
 */
using System;
using OpenIZ.Mobile.Core.Synchronization.Model;

namespace OpenIZ.Mobile.Core.Exceptions
{
	/// <summary>
	/// Local persistence exception.
	/// </summary>
	public class LocalPersistenceException : Exception
	{

        // Data object
        private Object m_data;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.LocalPersistenceException"/> class.
		/// </summary>
		/// <param name="operation">Operation.</param>
		/// <param name="data">Data.</param>
		public LocalPersistenceException (DataOperationType operation, Object data) : this(operation, data, null)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Exceptions.LocalPersistenceException"/> class.
		/// </summary>
		/// <param name="operation">Operation.</param>
		/// <param name="data">Data.</param>
		/// <param name="causedBy">Caused by.</param>
		public LocalPersistenceException (DataOperationType operation, Object data, Exception causedBy) : base(null, causedBy)
		{
			this.DataObject = data;
			this.Operation = operation;
		}

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>The error message that explains the reason for the exception, or an empty string ("").</returns>
		/// <filterpriority>1</filterpriority>
		/// <value>The message.</value>
		public override string Message {
			get {
				return String.Format ("{0} {1}", this.Operation, this.DataObject);
			}
		}

		/// <summary>
		/// Gets or sets the operation.
		/// </summary>
		/// <value>The operation.</value>
		public DataOperationType Operation {
			get { return (DataOperationType)this.Data ["operation"]; }
			private set { this.Data.Add ("operation", value); }
		}

		/// <summary>
		/// Gets or sets the data object.
		/// </summary>
		/// <value>The data object.</value>
		public Object DataObject {
			get { return this.m_data; }
			private set { this.m_data = value; }
		}
	}
}

