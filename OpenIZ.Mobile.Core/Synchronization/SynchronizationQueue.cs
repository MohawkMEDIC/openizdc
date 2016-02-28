using System;
using OpenIZ.Mobile.Core.Synchronization.Model;
using System.Collections.Generic;
using System.Reflection;
using SQLite;
using System.Threading.Tasks;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model;
using System.IO;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Synchronization
{
	/// <summary>
	/// Represents a generic synchronization queue
	/// </summary>
	public class SynchronizationQueue<TQueueEntry> where TQueueEntry : SynchronizationQueueEntry, new()
	{

		// Get the tracer
		private Tracer m_tracer = Tracer.GetTracer(typeof(SynchronizationQueue<TQueueEntry>));

		// Object sync
		private Object m_syncObject = new object ();

		// The queue instance
		private static SynchronizationQueue<TQueueEntry> s_instance;

		/// <summary>
		/// Singleton
		/// </summary>
		/// <value>The current.</value>
		public static SynchronizationQueue<TQueueEntry> Current
		{
			get {
				if (s_instance == null)
					s_instance = new SynchronizationQueue<TQueueEntry> ();
				return s_instance;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Synchronization.SynchronizationQueue`1"/> class.
		/// </summary>
		private SynchronizationQueue ()
		{
		}

		/// <summary>
		/// Create a connection
		/// </summary>
		/// <returns>The connection.</returns>
		private SQLiteConnection CreateConnection()
		{
			return new SQLiteConnection (ApplicationContext.Current.Configuration.GetConnectionString (
				ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection> ().MessageQueueConnectionStringName
			).Value);
		}



		/// <summary>
		/// Enqueue the specified entry data
		/// </summary>
		public TQueueEntry Enqueue(IdentifiedData data)
		{
			// Serialize object
			XmlSerializer xsz = new XmlSerializer (data.GetType ());
			using (MemoryStream ms = new MemoryStream ()) {
				xsz.Serialize (ms, data);
				ms.Flush ();

				// Create queue entry
				TQueueEntry queueEntry = new TQueueEntry () {
					Data = ms.ToArray (),
					CreationTime = DateTime.Now,
					Type = data.GetType ().FullName
				};

				// Enqueue the object
				this.EnqueueRaw (queueEntry);

				return queueEntry;
			}
		}

		/// <summary>
		/// Enqueue the specified entry
		/// </summary>
		/// <param name="entry">Entry.</param>
		public void EnqueueRaw(TQueueEntry entry)
		{
			using (SQLiteConnection conn = this.CreateConnection()) {
				try
				{
					conn.BeginTransaction ();

					// Persist the queue entry
					this.m_tracer.TraceInfo("Enqueue {0} successful. Queue item {1}", entry, conn.Insert(entry));
					conn.Commit();

				}
				catch(Exception e) {
					this.m_tracer.TraceError ("Error enqueueing object {0} : {1}", entry, e);
					conn.Rollback ();
					throw;
				}

			}
		}

		/// <summary>
		/// Deserialize the object
		/// </summary>
		private IdentifiedData DeserializeObject(TQueueEntry entry)
		{
			using(MemoryStream ms = new MemoryStream(entry.Data))
			{
				this.m_tracer.TraceVerbose ("Will deserialize entry {0} of type {1}", entry.Id, entry.Type);
				XmlSerializer serializer = new XmlSerializer (Type.GetType (entry.Type));
				return serializer.Deserialize (ms) as IdentifiedData;
			}
		}

		/// <summary>
		/// Peeks at the next item in the stack
		/// </summary>
		public IdentifiedData Peek()
		{
			return this.DeserializeObject (this.PeekRaw ());
		}

		/// <summary>
		/// Pop an item from the queue.
		/// </summary>
		public IdentifiedData Pop() {
			return this.DeserializeObject (this.PopRaw ());
		}

		/// <summary>
		/// Pops the item off the stack
		/// </summary>
		public TQueueEntry PopRaw()
		{
			using (SQLiteConnection conn = this.CreateConnection ()) {
				try
				{
					conn.BeginTransaction();

					// Fetch the object
					var queueItem = conn.Table<TQueueEntry>().OrderBy(i=>i.Id).First();

					// Delete the object
					conn.Delete(queueItem);

					conn.Commit();
					return queueItem;
				}
				catch(Exception e) {
					this.m_tracer.TraceError ("Error popping object off queue : {0}", e);
					conn.Rollback ();
					throw;
				}
			}
		}

		/// <summary>
		/// Peeks a raw row entry from the database.
		/// </summary>
		/// <returns>The raw.</returns>
		public TQueueEntry PeekRaw(){
			using (SQLiteConnection conn = this.CreateConnection ()) {
				try
				{
					return conn.Table<TQueueEntry> ().OrderBy (i => i.Id).FirstOrDefault ();
				}
				catch(Exception e) {
					this.m_tracer.TraceError ("Error peeking object: {0}", e);
					throw;
				}
			}
		}

	}
}

