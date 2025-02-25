﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.WidgetDashboard.Models.View {
    //
    /// <summary>
    /// The data structure stored in the DashboardViewModel, which is stored in the config file, and used to render the dashboard.
    /// The render widget process populates the htmlContent property, and verifies the width and height.
    /// 
    /// </summary>
    public class ConfigWidgetModel {
        //
        /// <summary>
        /// unique string that identifies this instance of the widget
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// The gridstack coordintate
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// The gridstack coordintate
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// The gridstack width
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// The gridstack height
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// if provided, the widget will be linked to this url
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// the number of seconds to refresh the widget. 0 for no refresh
        /// </summary>
        public int refreshSeconds { get; set; }
        /// <summary>
        /// the guid of the addon that provides the html
        /// </summary>
        public string addonGuid { get; set; }
        /// <summary>
        /// the html content provided by the addon
        /// </summary>
        public string htmlContent { get; set; }
    }
}
