using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.WidgetDashboard.Models.Domain {
    /// <summary>
    /// This is the data model returned by addons used for dashboard widgets.
    /// Addons are rendered in two ways.
    /// 1) during the dashboard render, it can be rendered server-side
    /// 2) in the UI, the dashboard can ajax back to a dashboard remote method to return an array of these models for each widget
    /// </summary>
    public class DashWidgetBaseModel {
        /// <summary>
        /// The minimum width of the widget in gridStack units, 12 units is full width
        /// </summary>
        public int minWidth { get; set; }
        /// <summary>
        /// The minimum height of the widget in gridStack units, 12 units is full width
        /// </summary>
        public int minHeight { get; set; }
        /// <summary>
        /// The number of seconds to refresh the widget. typically 0 for no refresh
        /// </summary>
        public int refreshSeconds { get; set; }

    }
}
