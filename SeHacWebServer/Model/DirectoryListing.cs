﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeHacWebServer.Model
{
    class DirectoryListing
    {
        /// <summary>
        /// Generates the directory listing  
        /// </summary>
        /// <param name="pathString"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static String Generate(String pathString, string root)
        {
            String html = "<html><head><title>Directory list</title></head><body><ul>";
            
            String[] dirList = Directory.GetDirectories(pathString);
            String[] fileList = Directory.GetFiles(pathString);

            for (int i = 0; i < dirList.Length; i++)
            {
                String dir = @dirList[i];
                html += "<li><a href=\"" + dir.Substring(root.Length).Replace("\\", "/") + "\">" + dir.Substring(pathString.Length).Replace("\\","") + "</a></li>";
            }
            for (int i = 0; i < fileList.Length; i++)
            {
                String file = fileList[i];
                html += "<li><a href=\"" + file.Substring(root.Length).Replace("\\", "/") + "\">" + file.Substring(pathString.Length).Replace("\\", "") + "</a></li>";
            }

            html += "</ul></body></html>";

            return html;
        }
    }
}
