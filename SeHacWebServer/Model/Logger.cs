using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeHacWebServer
{
    class Logger
    {
        private static int current = -1;
        private Thread m_doStuff;
        private static String[] m_loggerQueue = new String[3];
        private static Semaphore m_loggerSemaphore = new Semaphore(3,3);
        private string root = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

        public Logger()
        {
            m_doStuff = new Thread(doLogging);
            m_doStuff.Start();
        }

        public static void addLog(string logline)
        {
            //
            m_loggerSemaphore.WaitOne();
            lock(m_loggerQueue)
            {
                m_loggerQueue[++current] = logline;
            }
        }

        /// <summary>
        /// logging worker thread
        /// </summary>
        public void doLogging()
        {
            while (true)
            {
                string logline = null;

                lock (m_loggerQueue)
                {
                    logline = m_loggerQueue[0];
                    if (logline != null)
                    {
                        m_loggerQueue = swap();
                        m_loggerSemaphore.Release();
                        //writelineAsync omdat er meerdere threads tegelijk loggen mss nog veranderen
                        //TEST URL
                        using (StreamWriter sw = File.AppendText(root + @"\Logfiles\ControlServer.log.txt"))
                        {
                            sw.WriteLineAsync(logline);
                        }
                    }
                }
            }
        }

        public String[] swap()
        {
            var temp = new String[m_loggerQueue.Length];
            Array.Copy(m_loggerQueue, 1, temp, 0, m_loggerQueue.Length - 1);
            current--;
            return temp;
        }
    }
}
