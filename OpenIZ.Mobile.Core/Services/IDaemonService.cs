using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Services
{
    /// <summary>
    /// Daemon service which runs when the application is started
    /// </summary>
    public interface IDaemonService
    {
        /// <summary>
        /// Starts the specified daemon service
        /// </summary>
        bool Start();
        /// <summary>
        /// Stop the service
        /// </summary>
        bool Stop();
        /// <summary>
        /// True when daemon is running
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// Fired when the daemon is starting
        /// </summary>
        event EventHandler Starting;
        /// <summary>
        /// Fired when the daemon is started
        /// </summary>
        event EventHandler Started;
        /// <summary>
        /// Fired when the daemon is stopping
        /// </summary>
        event EventHandler Stopping;
        /// <summary>
        /// Fired when the daemon has stopped
        /// </summary>
        event EventHandler Stopped;
    }
}
