using Contensive.Addons.Dashboard.Controllers;
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Contensive.Addons.Dashboard.Models {
    public class ConfigModel {
        public ConfigWrapper defaultWrapper;
        public Dictionary<string, ConfigNodeModel> nodeList;
        // 
        // ====================================================================================================
        /// <summary>
        /// Create a config model of the user. If not found, one is created.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ConfigModel create(CPBaseClass cp, int userId) {
            try {
                var result = load(cp, userId);
                if (result is null) {
                    // 
                    // -- try legacy config
                    result = createFromLegacyXmlData(cp, userId);
                    if (result is null) {
                        // 
                        // -- load default
                        result = load(cp, 0);
                        if (result is null) {
                            // 
                            // -- load legacy default and create current default
                            result = createFromLegacyXmlData(cp, 0);
                            if (result is null) {
                                result = cp.JSON.Deserialize<ConfigModel>(getDefaultConfigJson());
                            }
                            // 
                            // -- create default
                            result.save(cp, 0);
                        }
                    }
                    result.save(cp, userId);
                }
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// load config for a user. Returns null if config file not found
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static ConfigModel load(CPBaseClass cp, int userId) {
            ConfigModel result = null;
            string userConfigFilename = @"dashboard\dashconfig." + userId + ".json";
            string jsonConfigText = cp.CdnFiles.Read(userConfigFilename);
            if (!string.IsNullOrWhiteSpace(jsonConfigText)) {
                result = cp.JSON.Deserialize<ConfigModel>(jsonConfigText);
            }
            return result;
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// save config for a specified user. Needed to save for the default user=0 
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="userId"></param>
        public void save(CPBaseClass cp, int userId) {
            cp.CdnFiles.Save(@"dashboard\dashconfig." + userId + ".json", cp.JSON.Serialize(this));
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// save config for the current user
        /// </summary>
        /// <param name="cp"></param>
        public void save(CPBaseClass cp) {
            save(cp, cp.User.Id);
        }
        // 
        // ====================================================================================================
        // 
        public static ConfigModel createFromLegacyXmlData(CPBaseClass cp, int userId) {
            ConfigModel result = null;
            try {
                string UserConfigFilename = userId <= 0 ? @"dashboard\dashconfig.xml" : @"dashboard\dashconfig." + userId + ".xml";
                string xmlConfigText = cp.CdnFiles.Read(UserConfigFilename);
                if (!string.IsNullOrWhiteSpace(xmlConfigText)) {
                    var xmlConfig = new XmlDocument();
                    xmlConfig.LoadXml(xmlConfigText);
                    if (xmlConfig.HasChildNodes) {
                        result = new ConfigModel() { nodeList = new Dictionary<string, ConfigNodeModel>() };
                        // 
                        // -- review values, remove deleted nodes and get settings
                        int nodeKeySuffix = 0;
                        foreach (XmlElement node in xmlConfig.DocumentElement.ChildNodes) {
                            switch (node.Name.ToLower() ?? "") {
                                case "defaultwrapper": {
                                        result.defaultWrapper = new ConfigWrapper() { guid = DashboardIconController.getAttribute(cp, node, "guid") };
                                        break;
                                    }
                                case "node": {
                                        var addonArgList = new List<NameValueModel>();
                                        string optionString = DashboardIconController.getAttribute(cp, node, "optionstring");
                                        if (!string.IsNullOrWhiteSpace(optionString)) {
                                            foreach (string namevalue in optionString.Split('&')) {
                                                if (namevalue.IndexOf('=') > 0) {
                                                    string[] pair = namevalue.Split('=');
                                                    addonArgList.Add(new NameValueModel() {
                                                        name = pair[0],
                                                        value = pair[1]
                                                    });
                                                }
                                            }
                                        }
                                        string nodeKey = "node" + nodeKeySuffix;
                                        ConfigNodeState nodeState;
                                        Enum.TryParse(DashboardIconController.getAttribute(cp, node, "state"), out nodeState);
                                        result.nodeList.Add(nodeKey, new ConfigNodeModel() {
                                            key = nodeKey,
                                            addonArgList = addonArgList,
                                            addonGUID = DashboardIconController.getAttribute(cp, node, "addonGUID"),
                                            contentGUID = DashboardIconController.getAttribute(cp, node, "contentGUID"),
                                            contentName = DashboardIconController.getAttribute(cp, node, "contentName"),
                                            sizex = cp.Utils.EncodeInteger(DashboardIconController.getAttribute(cp, node, "sizex")),
                                            sizey = cp.Utils.EncodeInteger(DashboardIconController.getAttribute(cp, node, "sizey")),
                                            state = nodeState,
                                            title = DashboardIconController.getAttribute(cp, node, "title"),
                                            x = cp.Utils.EncodeInteger(DashboardIconController.getAttribute(cp, node, "x")),
                                            y = cp.Utils.EncodeInteger(DashboardIconController.getAttribute(cp, node, "y"))
                                        });
                                        break;
                                    }
                            }
                            nodeKeySuffix += 1;
                        }
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        //
        // ====================================================================================================
        /// <summary>
        /// Read the default config JSON from the embedded resource
        /// </summary>
        private static string getDefaultConfigJson() {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Contensive.Addons.Dashboard.Resources.defaultConfigJson.txt")) {
                if (stream == null) { return "{}"; }
                using (var reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    //
    public class ConfigWrapper {
        public string guid;
    }
    // 
    public class NameValueModel {
        public string name;
        public string value;
    }
    // 
    public enum ConfigNodeState {
        closed = 1,
        open = 2
    }
    // 
    public class ConfigNodeModel {
        /// <summary>
        /// The htmlId of the node. 
        /// Also the key into the dictionary of these objects.
        /// Also the prefix for other html structions: 
        /// </summary>
        public string key;
        /// <summary>
        /// optional, if provided this icon will link to the content list page
        /// </summary>
        public string contentName;
        public string contentGUID;
        /// <summary>
        /// opional, if provided this icon will link to the execution of this addon
        /// </summary>
        public string addonGUID;
        /// <summary>
        /// title for this node
        /// </summary>
        public string title;
        /// <summary>
        /// open, closed, etc - convert to an enumeration
        /// </summary>
        public ConfigNodeState state;
        /// <summary>
        /// the width
        /// </summary>
        public int sizex;
        /// <summary>
        /// the height
        /// </summary>
        public int sizey;
        /// <summary>
        /// if node is for an addon, set these request properties first
        /// </summary>
        public List<NameValueModel> addonArgList;
        /// <summary>
        /// The style top for the object
        /// </summary>
        public int x;
        /// <summary>
        /// the style left for the object
        /// </summary>
        public int y;
        /// <summary>
        /// The html style z index for this object
        /// </summary>
        public int z;
        /// <summary>
        /// If not empty, this link will be used for the dashboard icon. Use for navigator entries setup as links.
        /// </summary>
        public string link;
        /// <summary>
        /// currently not used. The id of the wrapper object
        /// </summary>
        public int wrapperId;
    }
}