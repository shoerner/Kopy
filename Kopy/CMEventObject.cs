using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopy
{
    class CMEventObject
    {
        public String Message { get; set; }
        public String Severity { get; set; }
        public String Component { get; set; }
        public DateTime CalledTime { get; set; }
        public int CallingThread { get; set; }
        
        public const String LogOpen = @"<![LOG[";
        public const String LogClose = @"]LOG]!>";
        public const String FullTimeBias = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString();
        public const String TimeBias = FullTimeBias.Substring(0, FullTimeBias.IndexOf(':'));
        
        /// <summary>
        /// Creates a basic event to be filled later
        /// </summary>
        public CMEventObject()
        {
            Message = String.Empty;
            Severity = "INFO";
            Component = "GenericApplication";
            CalledTime = DateTime.Now;
            CallingThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        /// <summary>
        /// Creates a basic event with a message and a calling component
        /// </summary>
        /// <param name="Message">Message to be stored in the log</param>
        /// <param name="Component">Calling method/class</param>
        public CMEventObject(String Message, String Component)
        {
            this.Message = Message;
            Severity = "INFO";
            this.Component = Component;
            CalledTime = DateTime.Now;
            CallingThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        /// <summary>
        /// Creates an event with a message, calling component, and severity level
        /// </summary>
        /// <param name="Message">Message to be stored in the log</param>
        /// <param name="Component">Calling method/class</param>
        /// <param name="Severity">Importance, Suggested Ratings: INFO, WARNING, ERROR, CRITICAL</param>
        public CMEventObject(String Message, String Component, String Severity)
        {
            this.Message = Message;
            this.Severity = Severity.ToUpper();
            this.Component = Component;
            CalledTime = DateTime.Now;
            CallingThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        /// <summary>
        /// Formats output for single line viewing log
        /// </summary>
        /// <returns>Simplified toString for object</returns>
        public String toLineFormat()
        {
            return (CalledTime.ToString("MM/dd/yyyy hh:mm:ss.fff") + " : " + Component + " : " + Severity + " : " + Message);
        }
        public override String ToString()
        {
            // Build the Date String
            String date = CalledTime.ToString("MM/dd/yyyy");
            String time = CalledTime.ToString("hh:mm:ss.fff");
            // Build the output string
            String output = LogOpen + Message + LogClose + "<time=\"" + time + TimeBias + "\" date=\"" + date +
                "\" component=\"" + Component + "\" context=\"\" type=\"" + Severity.ToUpper() + "\" thread=\"" + CallingThread +
                "\" file=\"\">\n";

            // If we did it right, it should look something like this:
            // "<![LOG[Message]LOG]!><time="[Time]±[Bias]" date="[Date]" component="[Component]" context="" type="[Severity]" thread="[Thread]" file="">"

            return output;
        }
    }
}
