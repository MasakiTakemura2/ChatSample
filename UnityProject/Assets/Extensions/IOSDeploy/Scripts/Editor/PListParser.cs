using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.IOSDeployEditor
{
    public class PListParser
    {
        public PListDict xmlDict;
        private string filePath;

        public PListParser(string fullPath)
        {
            filePath = fullPath;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            XmlReader plistReader = XmlReader.Create(filePath, settings);

            XDocument doc = XDocument.Load(plistReader);
            XElement plist = doc.Element("plist");
            XElement dict = plist.Element("dict");

            xmlDict = new PListDict(dict);
            plistReader.Close();
        }
        
        /// <summary>
        /// Updates the schemes settings.
        /// </summary>
        /// <param name="urlSchemes">URL schemes.</param>
        public void UpdateSchemesSettings(string[] urlSchemes)
        {
            if (xmlDict.ContainsKey ("CFBundleURLTypes")) {
                //Debug.Log ("Find CFBundleURLTypes");
                var currentSchemas = (List<object>)xmlDict ["CFBundleURLTypes"];
                for (int i = 0; i < currentSchemas.Count; i++) {
                    // if it's not a dictionary, go to next index
                    if (currentSchemas [i].GetType () == typeof(PListDict)) {
                        var bundleTypeNode = (PListDict)currentSchemas [i];
                        if (bundleTypeNode.ContainsKey ("CFBundleURLSchemes") && bundleTypeNode ["CFBundleURLSchemes"].GetType () == typeof(List<object>)) {
                            var appIdsFromPListDict = (List<object>)bundleTypeNode ["CFBundleURLSchemes"];
                            appIdsFromPListDict.Clear ();
                            for (int j = 0; j < urlSchemes.Length; j++) {
                                string modifiedID = urlSchemes [j];
                                appIdsFromPListDict.Add ((object)modifiedID);
                            }
                            return;
                        }
                    }
                }

                // Didn't find schema, let's add schema to the list of schemas already present
                var appIds = new List<object> ();
                for (int j = 0; j < urlSchemes.Length; j++) {
                    string modifiedID = urlSchemes [j];
                    appIds.Add ((object)modifiedID);
                }
                var schemaEntry = new PListDict ();
                schemaEntry.Add ("CFBundleURLSchemes", appIds);
                currentSchemas.Add (schemaEntry);
                return;
            } else {
                //Debug.Log ("Didn't find any CFBundleURLTypes");
                var appIds = new List<object> ();
                for (int j = 0; j < urlSchemes.Length; j++) {
                    Debug.Log ("urlScheme : " + urlSchemes [j]);

                    string modifiedID = urlSchemes [j];
                    appIds.Add ((object)modifiedID);
                }
                var schemaEntry = new PListDict ();
                schemaEntry.Add ("CFBundleURLSchemes", appIds);

                var currentSchemas = new List<object> ();
                currentSchemas.Add (schemaEntry);
                xmlDict.Add ("CFBundleURLTypes", currentSchemas);
            }
        }

        public void UpdateRegionSettings(string[] regionSettings)
        {
            string currentKey = "CFBundleDevelopmentRegion";

            if (xmlDict.ContainsKey (currentKey)) {
                xmlDict.Remove (currentKey);
            }

            if (regionSettings.Length == 1) {
                xmlDict.Add (currentKey, (object)regionSettings [0]);
            } else if (0 < regionSettings.Length) {
                var regionList = new List<object> ();
                for (int j = 0; j < regionSettings.Length; j++) {
                    string region = regionSettings [j];
                    regionList.Add ((object)region);
                }
                xmlDict.Add (currentKey, regionList);
            }
        }

        public void UpdateDomainSecuritySettings(string[] domains)
        {
            string currentKey = "NSAppTransportSecurity";

            if (xmlDict.ContainsKey (currentKey)) {
                xmlDict.Remove (currentKey);
            }

            if (domains.Length == 0) {
                var dictSecurity = new PListDict ();
                object setVlaue = "true";
                dictSecurity.Add ("NSAllowsArbitraryLoads", setVlaue);
                xmlDict.Add (currentKey, dictSecurity);
            } else {
                var dictSecurity = new PListDict ();
                var dictDomains = new PListDict ();
                var settings = new PListDict ();
                object setVlaue;

                setVlaue = "true";
                settings.Add ("NSExceptionAllowsInsecureHTTPLoads", setVlaue);
                setVlaue = "false";
                settings.Add ("NSExceptionRequiresForwardSecrecy", setVlaue);
                setVlaue = "true";
                settings.Add ("NSIncludesSubdomains", setVlaue);

                for (int j = 0; j < domains.Length; j++) {
                    string domain = domains [j];
                    dictDomains.Add (domain, settings);
                }

                dictSecurity.Add ("NSExceptionDomains", dictDomains);
                xmlDict.Add (currentKey, dictSecurity);
            }
        }

        public void UpdateAppUsesEncryptionSettings(string[] settings)
        {
            if (settings.Length <= 0) {
                return;
            }

            string setValue = settings [0];
            string currentKey = "ITSAppUsesNonExemptEncryption";

            if (xmlDict.ContainsKey (currentKey)) {
                xmlDict.Remove (currentKey);
            }

            var setting = new object ();
            if (setValue == "true") {
                setting = true;
            } else if (setValue == "false") {
                setting = false;
            }
            xmlDict.Add (currentKey, setting);
        }

        /// <summary>
        /// Writes to file.
        /// </summary>
        public void WriteToFile()
        {
            // Corrected header of the plist
            string publicId = "-//Apple//DTD PLIST 1.0//EN";
            string stringId = "http://www.apple.com/DTDs/PropertyList-1.0.dtd";
            string internalSubset = null;
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XDocumentType docType = new XDocumentType("plist", publicId, stringId, internalSubset);

            xmlDict.Save(filePath, declaration, docType);
        }
    }
}
