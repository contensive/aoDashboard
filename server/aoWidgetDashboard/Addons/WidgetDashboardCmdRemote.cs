using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Models.View;
using System;
using System.Collections.Generic;

namespace Contensive.WidgetDashboard.Addons {
    public class WidgetDashboardCmdRemote : Contensive.BaseClasses.AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                string requestJson = cp.Request.Body;
                List<WDS_Request_Widget> widgets = cp.JSON.Deserialize<List<WDS_Request_Widget>>(requestJson);
                if (widgets.Count == 0) { return ""; }
                //
                DashboardViewModel userConfig = DashboardViewModel.create(cp);
                if (userConfig is null) { return ""; }
                //
                foreach (WDS_Request_Widget widget in widgets) {
                    var userConfigWidget = userConfig.widgets.Find(row => row.key == widget.key);
                    //
                    if(widget.cmd == "delete") {
                        if (userConfigWidget is null) { continue; }
                        userConfig.widgets.Remove(userConfigWidget);
                        continue;
                    }
                    if (widget.cmd == "save") {
                        if (userConfigWidget is null) {
                            userConfigWidget = new DashboardViewModel_Widgets {
                                key = widget.key
                            };
                            userConfig.widgets.Add(userConfigWidget);
                        }
                        userConfigWidget.x = widget.x;
                        userConfigWidget.y = widget.y;
                        userConfigWidget.width = widget.w;
                        userConfigWidget.height = widget.h;
                        continue;
                    }
                }
                userConfig.save(cp);
                return "";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
    //
    public class WDS_Request_Widget {
        //
        /// <summary>
        /// the command to perform on this widget: save, delete
        /// </summary>
        public string cmd { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int h { get; set; }
        public int w { get; set; }
        public string key { get; set; }
    }
}
