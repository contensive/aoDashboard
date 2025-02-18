using Contensive.BaseClasses;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;

namespace Contensive.WidgetDashboard.Models.View {
    internal class DashboardViewModel {
        //
        private CPBaseClass cp;
        public List<DashboardViewModel_Widgets> widgets { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        /// Create a config model of the user. If not found, one is created.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DashboardViewModel create(CPBaseClass cp) {
            try {
                DashboardViewModel result = load(cp);
                if (result != null) { return result; }
                //
                result = new DashboardViewModel() {
                    widgets = [
                        new DashboardViewModel_Widgets() { 
                            x=0,
                            y=0, 
                            width = 2, 
                            height = 2, 
                            content = cp.CdnFiles.Read("dashboard\\sampleWidget.html"), 
                            key="{E9285C2A-9A53-4170-A630-D520566F192A}", 
                            link="https://www.contensive.com",
                            addonGuid = "{E9285C2A-9A53-4170-A630-D520566F192A}"
                        },
                        new DashboardViewModel_Widgets() { x=2,y=0, width = 2, height = 2, content = "Widget 2", key="{6E528FC4-077F-4E4B-8294-DB8FF6BE818C}", link="https://www.contensive.com" },
                        new DashboardViewModel_Widgets() { x=4,y=0, width = 1, height = 1, content = "Widget 3", key="{D5129330-C2DA-41EA-AD02-B35CC8C26CF9}", link="https://www.contensive.com" },
                        new DashboardViewModel_Widgets() { x=4,y=1, width = 1, height = 1, content = "Widget 4", key="{03807CCD-3CA0-4EEE-B6E0-A16B62B14A4E}", link="https://www.contensive.com" },
                        new DashboardViewModel_Widgets() { x=5,y=0, width = 2, height = 2, content = "Widget 5", key="{AC5568C2-1D5F-4D76-AFDC-7DAFC56FE71E}", link="https://www.contensive.com" }
                    ]
                };
                result.save(cp);
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
        private static DashboardViewModel load(CPBaseClass cp) {
            DashboardViewModel result = null;
            string userConfigFilename = @"dashboard\widgetdashconfig." + cp.User.Id + ".json";
            string jsonConfigText = cp.PrivateFiles.Read(userConfigFilename);
            if (!string.IsNullOrWhiteSpace(jsonConfigText)) {
                result = cp.JSON.Deserialize<DashboardViewModel>(jsonConfigText);
            }
            return result;
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// save config for the current user
        /// </summary>
        /// <param name="cp"></param>
        public void save(CPBaseClass cp) {
            cp.PrivateFiles.Save(@"dashboard\widgetdashconfig." + cp.User.Id + ".json", cp.JSON.Serialize(this));
        }
    }
    internal class DashboardViewModel_Widgets {
        public string key { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string content { get; set; }
        public string link { get; set; }
        public string addonGuid { get; set; }
    }
}
