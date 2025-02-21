using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Models.Domain;
using Contensive.WidgetDashboard.Models.View;
using System;

namespace Contensive.WidgetDashboard.Addons {
    public class SampleDashWidget : Contensive.BaseClasses.AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                Contensive.WidgetDashboard.Models.Domain.DashWidgetAddonResultModel result = new() {
                    minWidth = 2,
                    minHeight = 2,
                    htmlContent = "" +
                        "<div class=\"d-flex justify-content-center align-items-center\" style=\"height: 100vh;\">" +
                            "<h4 class=\"text-center\">" +
                                "Sample" +
                                "<br>Dashboard" +
                                "<br>Widget" +
                            "</h4>" +
                        "</div>",
                    refreshSeconds = 0
                };
                return result; 
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }

        }
    }
}
