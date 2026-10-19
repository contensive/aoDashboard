
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.Dashboard.Views {
    public class ResizeClass : AddonBaseClass {
        public override object Execute(CPBaseClass CP) {
            string returnHtml = "";
            try {
                if (!CP.User.IsAuthenticated) { return ""; }
                var request = new Models.RequestModel(CP);
                string nodeKey = request.key;
                if (!string.IsNullOrWhiteSpace(nodeKey)) {
                    var config2 = Models.ConfigModel.create(CP, CP.User.Id);
                    if (config2 is not null) {
                        if (config2.nodeList.ContainsKey(nodeKey)) {
                            config2.nodeList[nodeKey].sizex = request.x;
                            config2.nodeList[nodeKey].sizey = request.y;
                            config2.save(CP);
                        }
                    }
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex);
                throw;
            }
            return returnHtml;
        }
    }
}