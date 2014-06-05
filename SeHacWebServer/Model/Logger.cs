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
        private Thread m_doStuff;
        private static Queue<String> m_loggerQueue= new Queue<String>();
        private static Semaphore m_loggerSemaphore = new Semaphore(3,3);
        ///////

        public Logger()
        {
            m_doStuff = new Thread(doLogging);
        }

        public static void addLog(string logline)
        {
            //
            m_loggerSemaphore.WaitOne();
            //threadsafe sup
            lock(m_loggerQueue)
            {
                m_loggerQueue.Enqueue(logline);
            }
        }

        /// <summary>
        /// logging worker thread
        /// </summary>
        //TODO: EventWaitHandle?
        public void doLogging()
        {
            
            //TODO:moet een keer stoppen....bijv bij afsluiten van socket
            while (true)
            {
                string logline = null;

                lock (m_loggerQueue)
                {
                    logline = m_loggerQueue.Dequeue();
                    m_loggerSemaphore.Release();
                    if (logline != null)
                    {
                        //writelineAsync omdat er meerdere threads tegelijk loggen mss nog veranderen
                        using (StreamWriter sw = File.AppendText(".log.txt"))
                        {
                            sw.WriteLineAsync(logline);
                        }
                    }
                }
            }
        }
    }
}
