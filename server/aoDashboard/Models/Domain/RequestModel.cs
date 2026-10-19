
using Contensive.BaseClasses;

namespace Contensive.Addons.Dashboard.Models {
    public class RequestModel {
        /// <summary>
        /// the node being acted on
        /// </summary>
        /// <returns></returns>
        public string key { get; set; }
        /// <summary>
        /// the x (left) position of the node being acted on
        /// </summary>
        /// <returns></returns>
        public int x { get; set; }
        /// <summary>
        /// the y (top) position of the node being acted on
        /// </summary>
        /// <returns></returns>
        public int y { get; set; }
        /// <summary>
        /// A key for new nodes being added. 
        /// Created by Admin Navigator (and maybe others later)
        /// For addons, the key is a followed by the addon Id
        /// </summary>
        /// <returns></returns>
        public string id { get; set; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cp"></param>
        public RequestModel(CPBaseClass cp) {
            key = cp.Doc.GetText("key");
            x = cp.Utils.EncodeInteger(cp.Doc.GetText("x").ToLower().Replace("px", ""));
            y = cp.Utils.EncodeInteger(cp.Doc.GetText("y").ToLower().Replace("px", ""));
            id = cp.Doc.GetText("id");
        }
    }
}