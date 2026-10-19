
using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.Dashboard.Views {
    public class DelNodeClass : AddonBaseClass {
        /// <summary>
        /// remote method called when a node is deleted on the desktop (click the delete link on the handle)
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
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
                            config2.nodeList.Remove(nodeKey);
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