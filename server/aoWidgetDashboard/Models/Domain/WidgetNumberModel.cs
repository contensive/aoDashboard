﻿
namespace Contensive.WidgetDashboard.Models.Domain {
    /// <summary>
    /// This dash widget simple returns html for the content
    /// </summary>
    public class WidgetNumberModel : WidgetBaseModel {
        public WidgetNumberModel() : base() {
            widgetType = WidgetTypeEnum.number;
        }
        public string number { get; set; }
        public string subhead { get; set; }
        public string description { get; set; }

    }
}
