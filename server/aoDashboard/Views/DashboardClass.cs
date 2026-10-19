using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.Dashboard.Views {
    public class DashboardClass : AddonBaseClass {
        /// <summary>
        /// addon that generates the dashboard html
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                int iconZIndexTop = 0;
                // 
                // -- create dashboard html and additional javascript
                var config = Models.ConfigModel.create(cp, cp.User.Id);
                string Dashboard = "";
                foreach (var kvp in config.nodeList) {
                    Dashboard += Models.NodeModel.getNodeHtml(cp, kvp.Value);
                    iconZIndexTop = iconZIndexTop > kvp.Value.z ? iconZIndexTop : kvp.Value.z;
                }
                // 
                // - add to page
                cp.Doc.AddBodyEnd($"<script type=\"text/javascript\">/* set ztop */ var iconZIndexTop={iconZIndexTop};</script>");
                result = cp.Html.div(Dashboard, "dashboard", "dashBoardWrapper", "dashBoardWrapper");
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
    }
}