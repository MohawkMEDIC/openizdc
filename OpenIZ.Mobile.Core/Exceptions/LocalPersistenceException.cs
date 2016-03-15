using System;
using OpenIZ.Mobile.Core.Synchronization.Model;

namespace OpenIZ.Mobile.Core.Exceptions
{
	/// <summary>
	/// Local persistence exception.
	/// </summary>
	public class LocalPersistenceException : Exception
	{

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
			get { return this.Data ["object"]; }
			private set { this.Data.Add ("object", value); }
		}
	}
}

