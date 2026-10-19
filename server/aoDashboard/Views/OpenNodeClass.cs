using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.Dashboard.Views {
    public class OpenNodeClass : AddonBaseClass {
        /// <summary>
        /// change a node state to open and return the html
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass CP) {
            string result = "";
            try {
                if (!CP.User.IsAuthenticated) { return ""; }
                var request = new Models.RequestModel(CP);
                if (!string.IsNullOrWhiteSpace(request.key)) {
                    var config = Models.ConfigModel.create(CP, CP.User.Id);
                    if (config is not null) {
                        if (config.nodeList.ContainsKey(request.key)) {
                            var configNode = config.nodeList[request.key];
                            configNode.state = Models.ConfigNodeState.open;
                            config.save(CP);
                            result = Models.NodeModel.getNodeHtml(CP, configNode);
                        }
                    }
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
    }
}