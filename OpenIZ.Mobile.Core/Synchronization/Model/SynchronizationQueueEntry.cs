using System;
using SQLite;
using OpenIZ.Core.Model;
using System.Collections.Generic;
using System.Reflection;

namespace OpenIZ.Mobile.Core.Synchronization.Model
{
	/// <summary>
	/// Synchronization operation type.
	/// </summary>
	public enum DataOperationType 
	{
		/// <summary>
		/// The operation represents an inbound entry (sync)
		/// </summary>
		Sync = 0,
		/// <summary>
		/// Operation represents an insert (create) only if not existing
		/// </summary>
		Insert = 1,
		/// <summary>
		/// Operation represents an update
		/// </summary>
		Update = 2,
		/// <summary>
		/// Operation represents an obsolete
		/// </summary>
		Obsolete = 3
	}

	/// <summary>
	/// The message queue represents outbound or inbound data requests found by the sync service
	/// </summary>
	public abstract class SynchronizationQueueEntry  
	{

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[PrimaryKey, AutoIncrement, Column("id")]
		public int Id {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the operation.
		/// </summary>
		/// <value>The operation.</value>
		[Column("operation"), NotNull]
		public DataOperationType Operation {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the model type
		/// </summary>
		/// <value>The type.</value>
		[Column("type")]
		public String Type {
			get;
			set;
		}

		/// <summary>
		/// Creation time of the queue item
		/// </summary>
		/// <value>The creation time.</value>
		[Column("creation_time")]
		public DateTime CreationTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the serialized data which is to be sent to the service (XML)
		/// </summary>
		/// <value>The data.</value>
		[Column("data")]
		public byte[] Data {
			get;
			set;
		}

	}

	/// <summary>
	/// Outbound synchronization queue entry.
	/// </summary>
	[Table("outbound_queue")]
	public class OutboundQueueEntry : SynchronizationQueueEntry
	{

        /// <summary>
        /// Indicates the fail count
        /// </summary>
        public int RetryCount { get; set; }
    }

	/// <summary>
	/// Dead letter queue entry - Dead letters are queue items that could not be synchronized for some reason.
	/// </summary>
	[Table("deadletter_queue")]
	public class DeadLetterQueueEntry : SynchronizationQueueEntry
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Synchronization.Model.DeadLetterQueueEntry"/> class.
		/// </summary>
		public DeadLetterQueueEntry ()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Synchronization.Model.DeadLetterQueueEntry"/> class.
		/// </summary>
		/// <param name="fromEntry">From entry.</param>
		public DeadLetterQueueEntry (SynchronizationQueueEntry fromEntry)
		{
			if (fromEntry == null)
				throw new ArgumentNullException (nameof (fromEntry));
			
			this.OriginalQueue = fromEntry.GetType().GetTypeInfo ().GetCustomAttribute<TableAttribute> ().Name;
			this.Data = fromEntry.Data;
			this.CreationTime = DateTime.Now;
			this.Type = fromEntry.Type;
		}

		/// <summary>
		/// The original queue name to which the dead letter item belonged. This can be used for retry enqueuing 
		/// </summary>
		/// <value>The original queue.</value>
		[Column("original_queue")]
		public string OriginalQueue {
			get;
			set;
		}
	}

	/// <summary>
	/// Inbound queue represents an object which was received from the server that needs to be inserted into the OpenIZ mobile database
	/// </summary>
	[Table("inbound_queue")]
	public class InboundQueueEntry : SynchronizationQueueEntry 
	{
	}

    /// <summary>
    /// Queue which is used to store administrative events on the user
    /// </summary>
    [Table("admin_queue")]
    public class OutboundAdminQueueEntry : SynchronizationQueueEntry
    {
    }
}

