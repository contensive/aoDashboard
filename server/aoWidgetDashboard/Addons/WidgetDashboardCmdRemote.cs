using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Controllers;
using Contensive.WidgetDashboard.Models.View;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace Contensive.WidgetDashboard.Addons {
    /// <summary>
    /// Remote Method called from the dashboard with commands
    /// </summary>
    public class WidgetDashboardCmdRemote : Contensive.BaseClasses.AddonBaseClass {
        /// <summary>
        /// addon interface
        /// all commands return a result as type DashboardViewModel, with a widget list that needs to be updated on the UI. 
        /// It may be empty if nothing needs to be updated.
        /// see DashboardViewModel
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                string requestJson = cp.Request.Body;
                List<WDS_Request_Widget> requestWidgets = cp.JSON.Deserialize<List<WDS_Request_Widget>>(requestJson);
                if (requestWidgets.Count == 0) { return ""; }
                //
                ConfigModel userDashboardConfig = ConfigModel.create(cp);
                if (userDashboardConfig is null) { return ""; }
                //
                List<WDS_Response> result = [];
                foreach (WDS_Request_Widget requestWidget in requestWidgets) {
                    var userDashboardConfigWidget = userDashboardConfig.widgets.Find(row => row.key == requestWidget.key);
                    //
                    if (requestWidget.cmd == "delete") {
                        if (userDashboardConfigWidget is null) { continue; }
                        userDashboardConfig.widgets.Remove(userDashboardConfigWidget);
                        continue;
                    }
                    //
                    if (requestWidget.cmd == "refresh") {
                        if (userDashboardConfigWidget is null) { continue; }
                        result.Add(new WDS_Response { 
                            key = requestWidget.key, 
                            htmlContent = WidgetRenderController.renderWidget(cp, userDashboardConfigWidget).htmlContent,
                            link = userDashboardConfigWidget.link
                        });
                        continue;
                    }
                    if (requestWidget.cmd == "save") {
                        if (userDashboardConfigWidget is null) {
                            userDashboardConfigWidget = new ConfigWidgetModel {
                                key = requestWidget.key
                            };
                            userDashboardConfig.widgets.Add(userDashboardConfigWidget);
                        }
                        userDashboardConfigWidget.x = requestWidget.x;
                        userDashboardConfigWidget.y = requestWidget.y;
                        userDashboardConfigWidget.width = requestWidget.w;
                        userDashboardConfigWidget.height = requestWidget.h;
                        userDashboardConfigWidget.addonGuid = requestWidget.addonGuid;
                        result.Add(new WDS_Response {
                            key = requestWidget.key,
                            htmlContent = WidgetRenderController.renderWidget(cp, userDashboardConfigWidget).htmlContent,
                            link = userDashboardConfigWidget.link
                        });
                        continue;
                    }
                }
                userDashboardConfig.save(cp);
                return result;
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
        public string addonGuid { get; set; }
    }
    //
    public class WDS_Response {
        //
        public string key { get; set; }
        //
        public string htmlContent { get; set; }
        //
        public string link { get; set; }
    }
}
