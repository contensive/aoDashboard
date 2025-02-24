using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Controllers;
using Contensive.WidgetDashboard.Models.Domain;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;

namespace Contensive.WidgetDashboard.Models.View {
    internal class DashboardViewModel {
        //
        private CPBaseClass cp;
        public List<DashboardWidgetViewModel> widgets { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        /// Create the dashboard viewModel from the config file of the user and render the htmlContent.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DashboardViewModel create(CPBaseClass cp) {
            try {
                DashboardViewModel viewModel = load(cp);
                if (viewModel?.widgets != null && viewModel.widgets.Count > 0) {
                    //
                    // -- render the htmlcontent and reutrn
                    return WidgetRenderController.renderWidgets(cp, viewModel);
                }
                //
                // -- iniitalize with default widgets
                viewModel = new DashboardViewModel() {
                    widgets = [
                        new DashboardWidgetViewModel() {
                            x=0,
                            y=0,
                            width = 2,
                            height = 2,
                            htmlContent = cp.CdnFiles.Read("dashboard\\sampleWidget.html"),
                            key="E928",
                            link="https://www.contensive.com",
                            addonGuid = Constants.sampleDashboardWidgetGuid
                        },
                        new DashboardWidgetViewModel() { x=2,y=0, width = 2, height = 2, htmlContent = "Widget 2", key="6E52", link="https://www.contensive.com" },
                        new DashboardWidgetViewModel() { x=4,y=0, width = 1, height = 1, htmlContent = "Widget 3", key="D512", link="https://www.contensive.com" },
                        new DashboardWidgetViewModel() { x=4,y=1, width = 1, height = 1, htmlContent = "Widget 4", key="0380", link="https://www.contensive.com" },
                        new DashboardWidgetViewModel() { x=5,y=0, width = 2, height = 2, htmlContent = "Widget 5", key="AC55", link="https://www.contensive.com" }
                    ]
                };
                //
                // -- save the new view model before rendering the htmlcontent
                viewModel.save(cp);
                //
                // -- after save, render the htmlContent
                DashboardViewModel layout = WidgetRenderController.renderWidgets(cp, viewModel);
                return layout;
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
}
