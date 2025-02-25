using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Models.View;
using System;

namespace Contensive.WidgetDashboard.Addons {
    public class WidgetDashboardAddon : Contensive.BaseClasses.AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                string layout = cp.Layout.GetLayout(Constants.dashboardLayoutGuid,Constants.dashboardLayoutName,Constants.dashboardLayoutPathFilename);
                ConfigModel viewModel = ConfigModel.create(cp);
                return cp.Mustache.Render(layout, viewModel); 
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }

        }
    }
}
