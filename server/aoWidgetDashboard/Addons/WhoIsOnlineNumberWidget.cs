using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Models.Domain;
using Contensive.WidgetDashboard.Models.View;
using System;

namespace Contensive.WidgetDashboard.Addons {
    public class WhoIsOnlineNumberWidget : Contensive.BaseClasses.AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                Contensive.WidgetDashboard.Models.Domain.WidgetNumberModel data = new() {
                    minWidth = 2,
                    minHeight = 2,
                    number = "100",
                    subhead = "Online Users",
                    description = "Number of users currently online. A user ",
                    refreshSeconds = 0
                };
                return data;
                //string layout = cp.Layout.GetLayout(Constants.numberDashWidgetLayoutGuid, Constants.numberDashWidgetLayoutName, Constants.numberDashWidgetLayoutPathFilename);
                //return new WidgetHtmlContentModel {
                //    htmlContent = cp.Mustache.Render(layout, data),
                //    refreshSeconds = data.refreshSeconds,
                //    minWidth = data.minWidth,
                //    minHeight = data.minHeight
                //};
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }

        }
    }
}
