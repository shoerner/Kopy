using System;
using System.Collections.Generic;
using System.IO;

namespace Kopy
{
    /// <summary>
    /// Consideration has been given to creating a dataset and placing each CMEventObject into it as distinct fields.
    /// As the primary means of conveyance will be a toString appearing as a ConfigManager event log item, this 
    /// idea was benched and a list will be used in its place.
    /// </summary>
    public sealed class LocalLog
    {
        private static List<CMEventObject> LoggedItems;
        private static List<CMEventObject> NonPushedItems;

        static readonly Object _LockObject = new Object();
        private LocalLog()
        {
            LoggedItems = new List<CMEventObject>();
            NonPushedItems = new List<CMEventObject>();
        }
        /// <summary>
        /// Helper method for CMEventObject auto-add
        /// </summary>
        /// <param name="Message">Message to be stored in the log</param>
        /// <param name="Component">Calling method/class</param>
        public void MakeEvent(String Message, String Component)
        {
            NonPushedItems.Add(new CMEventObject(Message, Component));
        }
        /// <summary>
        /// Helper method for CMEventObject auto-add
        /// </summary>
        /// <param name="Message">Message to be stored in the log</param>
        /// <param name="Component">Calling method/class</param>
        /// <param name="Severity">Importance, Suggested Ratings: INFO, WARNING, ERROR, CRITICAL</param>
        public void MakeEvent(String Message, String Component, String Severity)
        {
            NonPushedItems.Add(new CMEventObject(Message, Component, Severity));
        }

        public void PushEvent()
        {
            // Perform the basic file checks
            // Build the file name per user setting
            String filePath;
            if (String.IsNullOrEmpty(Properties.Settings.Default.LogLocation) || !Directory.Exists(Properties.Settings.Default.LogLocation))
                filePath = System.IO.Path.GetTempPath() + "Kopy.log";
            else
                filePath = Properties.Settings.Default.LogLocation + "Kopy.log";


            // We are doing file IO here - a lock is needed to make sure we don't cross the streams
            lock (_LockObject)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        foreach (CMEventObject entry in NonPushedItems)
                        {
                            // Push to stream writer 
                            writer.WriteLine(entry.ToString());

                            // Push to pushed list
                            LoggedItems.Add(entry);
                        }
                    }
                }
                // empty the scratch list
                NonPushedItems.Clear();
            }
        }

        // Begin instance declaration 
        private static LocalLog instance = null;

        public static LocalLog Instance
        {
            get
            {
                if (instance == null)
                    instance = new LocalLog();
                return instance;
            }
        }
    }
}
