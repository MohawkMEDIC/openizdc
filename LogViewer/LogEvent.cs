using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogViewer
{
    /// <summary>
    /// Represents a log event
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// Gets or sets the log source
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// Get or sets the level
        /// </summary>
        public EventLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the thread
        /// </summary>
        public String Thread { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Load gzipped stream
        /// </summary>
        public static List<LogEvent> LoadGz(String filename)
        {
            using (var strm = new GZipStream(File.OpenRead(filename), CompressionMode.Decompress))
            using (var sw = new StreamReader(strm))
                return LogEvent.Load(sw);
        }

        /// <summary>
        /// Load plain text
        /// </summary>
        public static List<LogEvent> Load(String filename)
        {
            using (var sw = File.OpenText(filename))
                return LogEvent.Load(sw);
        }

        /// <summary>
        /// Load the specified file
        /// </summary>
        public static List<LogEvent> Load(StreamReader stream)
        {
            Regex v1Regex = new Regex(@"^(.*)?(\s)?[\s][\[\<](.*?)[\]\>]\s\[(.*?)\]\s?:(.*)$"),
             v2Regex = new Regex(@"^(.*)?@(.*)?\s[\[\<](.*)?[\>\]]\s\[(.*)?\]\:\s(.*)$");

            List<LogEvent> retVal = new List<LogEvent>();
            LogEvent current = null;

            while (!stream.EndOfStream)
            { 
                var line = stream.ReadLine();
                var match = v2Regex.Match(line);
                if (!match.Success)
                    match = v1Regex.Match(line);
                if (match.Success)
                {
                    if (current != null) retVal.Add(current);
                    current = new LogEvent()
                    {
                        Source = match.Groups[1].Value,
                        Thread = match.Groups[2].Value,
                        Level = (EventLevel)Enum.Parse(typeof(EventLevel), match.Groups[3].Value),
                        Date = DateTime.Parse(match.Groups[4].Value),
                        Message = match.Groups[5].Value
                    };
                }
                else if(current != null)
                    current.Message += "\r\n" + line;
            }
            if(current != null)
                retVal.Add(current);

            return retVal;
        }
    }
}
