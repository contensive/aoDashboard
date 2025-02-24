using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.WidgetDashboard.Models.Domain {
    /// <summary>
    /// This dash widget simple returns html for the content
    /// </summary>
    public class WidgetNumberModel : WidgetBaseModel {
        /// <summary>
        /// The html with css and javascript to render the widget
        /// </summary>
        public string number { get; set; }
        public string subhead { get; set; }
        public string description { get; set; }

    }
}
