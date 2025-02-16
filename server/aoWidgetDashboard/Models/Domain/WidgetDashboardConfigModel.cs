//using Contensive.BaseClasses;
//using Microsoft.VisualBasic;
//using System;
//using System.Collections.Generic;
//using System.Xml;

//namespace Models {
//    public class WidgetDashboardConfigModel {
//        public ConfigWrapper defaultWrapper { get; set; }
//        public List<WidgetDashboardConfigModel_node> nodeList { get; set; }
//        // 
//        // ====================================================================================================
//        /// <summary>
//        /// Create a config model of the user. If not found, one is created.
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="userId"></param>
//        /// <returns></returns>
//        public static WidgetDashboardConfigModel create(CPBaseClass cp, int userId) {
//            try {
//                WidgetDashboardConfigModel result = load(cp, userId);
//                if (result != null) { return result; }
//                //
//                result.nodeList = [];
//                // 
//                // -- try legacy config
//                result.save(cp, userId);
//                return result;
//            } catch (Exception ex) {
//                cp.Site.ErrorReport(ex);
//                throw;
//            }
//        }
//        //
//        // ====================================================================================================
//        /// <summary>
//        /// load config for a user. Returns null if config file not found
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="userId"></param>
//        /// <returns></returns>
//        private static WidgetDashboardConfigModel load(CPBaseClass cp, int userId) {
//            WidgetDashboardConfigModel result = null;
//            string userConfigFilename = @"dashboard\widgetdashconfig." + userId + ".json";
//            string jsonConfigText = cp.CdnFiles.Read(userConfigFilename);
//            if ((!string.IsNullOrWhiteSpace(jsonConfigText)))
//                result = cp.JSON.Deserialize<WidgetDashboardConfigModel>(jsonConfigText);
//            return result;
//        }
//        // 
//        // ====================================================================================================
//        /// <summary>
//        /// save config for a specified user. Needed to save for the default user=0
//        /// </summary>
//        /// <param name="cp"></param>
//        /// <param name="userId"></param>
//        public void save(CPBaseClass cp, int userId) {
//            cp.CdnFiles.Save(@"dashboard\widgetdashconfig." + userId + ".json", cp.JSON.Serialize(this));
//        }
//        // 
//        // ====================================================================================================
//        /// <summary>
//        /// save config for the current user
//        /// </summary>
//        /// <param name="cp"></param>
//        public void save(CPBaseClass cp) {
//            save(cp, cp.User.Id);
//        }
//    }

//    // 
//    public class ConfigWrapper {
//        public string guid;
//    }
//    // 
//    public class NameValueModel {
//        public string name;
//        public string value;
//    }
//    // 
//    public enum ConfigNodeState {
//        closed = 1,
//        open = 2
//    }
//    // 
//    public class WidgetDashboardConfigModel_node {
//        /// <summary>
//        /// The htmlId of the node.
//        /// Also the key into the dictionary of these objects.
//        /// Also the prefix for other html structions:
//        /// </summary>
//        public string key;
//        /// <summary>
//        /// optional, if provided this icon will link to the content list page
//        /// </summary>
//        public string contentName;
//        public string contentGUID;
//        /// <summary>
//        /// opional, if provided this icon will link to the execution of this addon
//        /// </summary>
//        public string addonGUID;
//        /// <summary>
//        /// title for this node
//        /// </summary>
//        public string title;
//        /// <summary>
//        /// open, closed, etc - convert to an enumeration
//        /// </summary>
//        public ConfigNodeState state;
//        /// <summary>
//        /// the width
//        /// </summary>
//        public int sizex;
//        /// <summary>
//        /// the height
//        /// </summary>
//        public int sizey;
//        /// <summary>
//        /// if node is for an addon, set these request properties first
//        /// </summary>
//        public List<NameValueModel> addonArgList;
//        /// <summary>
//        /// The style top for the object
//        /// </summary>
//        public int x;
//        /// <summary>
//        /// the style left for the object
//        /// </summary>
//        public int y;
//        /// <summary>
//        /// The html style z index for this object
//        /// </summary>
//        public int z;
//        /// <summary>
//        /// If not empty, this link will be used for the dashboard icon. Use for navigator entries setup as links.
//        /// </summary>
//        public string link;
//        /// <summary>
//        /// currently not used. The id of the wrapper object
//        /// </summary>
//        public int wrapperId;
//    }
//}
