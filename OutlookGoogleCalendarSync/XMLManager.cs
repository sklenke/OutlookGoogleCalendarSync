﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using log4net;

namespace OutlookGoogleCalendarSync
{
    /// <summary>
    /// Exports or imports any object to/from XML.
    /// </summary>
    public class XMLManager {
        private static readonly ILog log = LogManager.GetLogger(typeof(XMLManager));

        public XMLManager() {
        }
        
        /// <summary>
        /// Exports any object given in "obj" to an xml file given in "filename"
        /// </summary>
        /// <param name="obj">The object that is to be serialized/exported to XML.</param>
        /// <param name="filename">The filename of the xml file to be written.</param>
        public static void Export(Object obj, string filename) {
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            new DataContractSerializer(obj.GetType()).WriteObject(writer, obj);
            writer.Close();
        }
        
        /// <summary>
        /// Imports from XML and returns the resulting object of type T.
        /// </summary>
        /// <param name="filename">The XML file from which to import.</param>
        /// <returns></returns>
        public static T Import<T>(string filename) {
            FileStream fs = new FileStream(filename, FileMode.Open);
            T result = default(T);
            try {
                result = (T)new DataContractSerializer(typeof(T)).ReadObject(fs);
            } catch {
                MainForm.Instance.tabApp.SelectedTab = MainForm.Instance.tabPage_Settings;
            }
            fs.Close();
            return result;
        }

        public static void ExportElement(string nodeName, object nodeValue, string filename) {
            XDocument xml = XDocument.Load(filename);
            XNamespace ns = "http://schemas.datacontract.org/2004/07/OutlookGoogleCalendarSync";
            XElement settingsXE = xml.Descendants(ns + "Settings").FirstOrDefault();
            try {
                XElement xe = settingsXE.Elements(ns + nodeName).First();
                xe.SetValue(nodeValue);
            } catch (Exception ex) {
                if (ex.Message == "Sequence contains no elements") {
                    log.Debug("Setting " + nodeName + " added to settings.xml");
                    settingsXE.Add(new XElement(ns + nodeName, nodeValue));
                } else {
                    log.Error("Failed to export setting " + nodeName + "=" + nodeValue + " to settings.xml file.");
                }
            }
            xml.Save(filename);
            log.Debug("Setting '"+ nodeName +"' updated to '"+ nodeValue +"'");
        }
    }
}
