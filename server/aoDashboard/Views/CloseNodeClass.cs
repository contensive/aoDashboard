using Contensive.BaseClasses;
using System;

namespace Contensive.Addons.Dashboard.Views {
    public class CloseNodeClass : AddonBaseClass {
        /// <summary>
        /// remote method to change the state of a node to closed
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                if (!cp.User.IsAuthenticated) { return ""; }
                var request = new Models.RequestModel(cp);
                if (!string.IsNullOrWhiteSpace(request.key)) {
                    var config = Models.ConfigModel.create(cp, cp.User.Id);
                    if (config is not null) {
                        if (config.nodeList.ContainsKey(request.key)) {
                            var configNode = config.nodeList[request.key];
                            configNode.state = Models.ConfigNodeState.closed;
                            config.save(cp);
                            result = Models.NodeModel.getNodeHtml(cp, configNode);
                        }
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
            return result;
        }
    }
}