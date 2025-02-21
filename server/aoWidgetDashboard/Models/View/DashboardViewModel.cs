using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Models.Domain;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;

namespace Contensive.WidgetDashboard.Models.View {
    internal class DashboardViewModel {
        //
        private CPBaseClass cp;
        public List<DashboardViewModel_Widget> widgets { get; set; }
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
                DashboardViewModel viewModel = load(cp);
                if (viewModel?.widgets != null && viewModel.widgets.Count > 0) {
                    return renderWidgets(cp, viewModel );
                }
                //
                // -- iniitalize with default widgets
                viewModel = new DashboardViewModel() {
                    widgets = [
                        new DashboardViewModel_Widget() {
                            x=0,
                            y=0,
                            width = 2,
                            height = 2,
                            htmlContent = cp.CdnFiles.Read("dashboard\\sampleWidget.html"),
                            key="E928",
                            link="https://www.contensive.com",
                            addonGuid = Constants.sampleDashboardWidgetGuid
                        },
                        new DashboardViewModel_Widget() { x=2,y=0, width = 2, height = 2, htmlContent = "Widget 2", key="6E52", link="https://www.contensive.com" },
                        new DashboardViewModel_Widget() { x=4,y=0, width = 1, height = 1, htmlContent = "Widget 3", key="D512", link="https://www.contensive.com" },
                        new DashboardViewModel_Widget() { x=4,y=1, width = 1, height = 1, htmlContent = "Widget 4", key="0380", link="https://www.contensive.com" },
                        new DashboardViewModel_Widget() { x=5,y=0, width = 2, height = 2, htmlContent = "Widget 5", key="AC55", link="https://www.contensive.com" }
                    ]
                };
                viewModel.save(cp);
                DashboardViewModel layout = renderWidgets(cp, viewModel);
                return layout;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// render all the addons in the view model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static DashboardViewModel renderWidgets(CPBaseClass cp, DashboardViewModel viewModel ) {
            DashboardViewModel result = new();
            result.cp = cp;
            result.widgets = [];
            foreach (var widget in viewModel.widgets) {
                //if (keyFilter.Count > 0 && !keyFilter.Contains(widget.key)) { continue; }
                result.widgets.Add(renderWidget(cp, widget));
            }
            return result;
        }
        //
        // ====================================================================================================
        /// <summary>
        /// render the addon for the widget
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="widget"></param>
        /// <returns></returns>
        public static DashboardViewModel_Widget renderWidget(CPBaseClass cp, DashboardViewModel_Widget widget) {
            if (string.IsNullOrWhiteSpace(widget.addonGuid)) { return widget; }
            //
            string dashWidgetAddonResultJson = cp.Addon.Execute(widget.addonGuid);
            if (string.IsNullOrEmpty(dashWidgetAddonResultJson)) { return widget; }
            //
            DashWidgetAddonResultModel DashWidgetAddonResult = null;
            try {
                DashWidgetAddonResult = cp.JSON.Deserialize<DashWidgetAddonResultModel>(dashWidgetAddonResultJson);
            } catch (Exception) {
                cp.Site.ErrorReport($"Error deserializing widget data for widget {widget.addonGuid}");
                return widget;
            }
            if (DashWidgetAddonResult == null) { return widget; }
            //
            widget.htmlContent = DashWidgetAddonResult.htmlContent;
            widget.width = (widget.width > DashWidgetAddonResult.minWidth) ? widget.width : DashWidgetAddonResult.minWidth;
            widget.height = (widget.height > DashWidgetAddonResult.minHeight) ? widget.height : DashWidgetAddonResult.minHeight;
            return widget;
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
    public class DashboardViewModel_Widget {
        public string key { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string htmlContent { get; set; }
        public string link { get; set; }
        public string addonGuid { get; set; }
    }
}
