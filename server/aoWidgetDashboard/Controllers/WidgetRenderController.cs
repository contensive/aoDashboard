using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Models.Domain;
using Contensive.WidgetDashboard.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.WidgetDashboard.Controllers {
    internal class WidgetRenderController {
        //
        // ====================================================================================================
        /// <summary>
        /// render the htmlContent property for all the addons in the view model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static DashboardViewModel renderWidgets(CPBaseClass cp, DashboardViewModel viewModel) {
            DashboardViewModel result = new() {
                widgets = []
            };
            foreach (var widget in viewModel.widgets) {
                result.widgets.Add(renderWidget(cp, widget));
            }
            return result;
        }
        //
        // ====================================================================================================
        /// <summary>
        /// render the the htmlContent property for the widget
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="widget"></param>
        /// <returns></returns>
        public static DashboardWidgetViewModel renderWidget(CPBaseClass cp, DashboardWidgetViewModel widget) {
            if (string.IsNullOrWhiteSpace(widget.addonGuid)) { return widget; }
            //
            string dashWidgetAddonResultJson = cp.Addon.Execute(widget.addonGuid);
            if (string.IsNullOrEmpty(dashWidgetAddonResultJson)) { return widget; }
            //
            WidgetHtmlContentModel DashWidgetAddonResult = null;
            try {
                DashWidgetAddonResult = cp.JSON.Deserialize<WidgetHtmlContentModel>(dashWidgetAddonResultJson);
            } catch (Exception) {
                cp.Site.ErrorReport($"Error deserializing widget data for widget {widget.addonGuid}");
                return widget;
            }
            if (DashWidgetAddonResult == null) { return widget; }
            //
            widget.htmlContent = DashWidgetAddonResult.htmlContent;
            widget.width = (widget.width > DashWidgetAddonResult.minWidth) ? widget.width : DashWidgetAddonResult.minWidth;
            widget.height = (widget.height > DashWidgetAddonResult.minHeight) ? widget.height : DashWidgetAddonResult.minHeight;
            return widget;
        }
    }
}
