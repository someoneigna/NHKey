using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NHkey.Data
{
    /// <summary>
    /// Provides a log for unhandled errors and exceptions.
    /// </summary>
    public class Logger : IDisposable
    {
        private static string defaultPath = "App.log";
        private string logPath;
        private static int retries = 0;
        public StreamWriter Log { get; protected set; }

        public bool CanWrite { get; protected set; }


        public Logger()
        {
            Open(defaultPath);
        }

        public Logger(string path)
        {
            logPath = path;
            Open(path);
        }

        private void Open(string path)
        {
            try
            {
                Log = new StreamWriter(File.OpenWrite(path));
            }
            catch(PathTooLongException pathLong)
            {
                OpenDefaultPath();
                Append("Log", "Could not open " + path + " got an exception: " + pathLong.Message);
            }
            catch(UnauthorizedAccessException access)
            {
                OpenDefaultPath();
                Append("Log", "Could not open " + path + " got an exception: " + access.Message);
            }
            catch (IOException)
            {
                logPath += string.Format(".{0}", retries + 1);
                Open(logPath);
                return;
            }
            finally
            {
                if (Log != null)
                {
                    CanWrite = Log.BaseStream.CanWrite;
                }
            }

        }

        /// <summary>
        /// Append to log the message with source as header ("source" : "msg")
        /// </summary>
        /// <param name="source">The place where message comes.</param>
        /// <param name="msg">A message describing the situation.</param>
        public void Append(string source, string msg)
        {
            const int maxLineLength = 80;
            List<string> tokens = new List<string>();

            if (msg.Length > maxLineLength)
            {
                int start = 0;
                while (start + maxLineLength < msg.Length)
                {
                    tokens.Add(msg.Substring(start, maxLineLength) + "\n");
                    start += maxLineLength + 1;
                }
            }

            if (CanWrite)
            {
                Log.WriteLine("[" + source + "]: " + msg + ((tokens.Count > 0) ? "\n" : ""));
            }
            else
            {
                throw new InvalidOperationException("Failed to write on log, stream is in invalid state.");
            }
        }

        private void OpenDefaultPath()
        {
            Open(defaultPath);
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Close();
                Disposed = true;
            }
        }

        private void Close()
        {
            Log.Flush();
            Log.Close();
        }

        public bool Disposed { get; protected set; }
    }
}
