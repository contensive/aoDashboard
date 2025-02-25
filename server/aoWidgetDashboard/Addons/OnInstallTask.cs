using Contensive.BaseClasses;
using System;

namespace Contensive.WidgetDashboard.Addons {
    public class OnInstallTask : Contensive.BaseClasses.AddonBaseClass {
        public override object Execute(CPBaseClass CP) {
            try {
                int version = 0;
                int installedVersion = CP.Site.GetInteger("Widget Dashboard Version Installed", version);
                if (installedVersion < 0) {
                    //
                    //
                }
                //
                // -- udpate layout(s)
                CP.Layout.updateLayout(Constants.dashboardLayoutGuid, Constants.dashboardLayoutName, Constants.dashboardLayoutPathFilename);
                CP.Layout.updateLayout(Constants.dashboardWidgetNumberLayoutGuid, Constants.dashboardWidgetNumberLayoutName, Constants.dashboardWidgetNumberLayoutPathFilename);
                CP.Layout.updateLayout(Constants.dashboardWidgetHtmlContentLayoutGuid, Constants.dashboardWidgetHtmlContentLayoutName, Constants.dashboardWidgetHtmlContentLayoutPathFilename);
                // 
                CP.Site.SetProperty("Widget Dashboard Version Installed", version.ToString());
                return string.Empty;
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}
