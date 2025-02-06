using Contensive.BaseClasses;
using System;

namespace Contensive.WidgetDashboard.Addons {
    public class WidgetDashboardAddon : Contensive.BaseClasses.AddonBaseClass {
        public override object Execute(CPBaseClass CP) {
            try {
                string layout = CP.Layout.GetLayout(Constants.widgetDashboardLayoutGuid,Constants.widgetDashboardLayoutName,Constants.widgetDashboardLayoutPathFilename);
                return CP.Mustache.Render(layout, new {
                }); 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex);
                throw;
            }

        }
    }
}
