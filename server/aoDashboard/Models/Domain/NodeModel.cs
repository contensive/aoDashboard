using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;

namespace Contensive.Addons.Dashboard.Models {
    public class NodeModel {
        //
        // =====================================================================================
        /// <summary>
        /// the entire contructed object that is moved around on the dashboard.
        /// From inside out:
        /// DodadContent - The inner-most part of the Dodad, the actual content that is on the desktop
        /// DesignWrapper - the visible border for all Dodads
        /// ControlWrapper = the absolute positioned wrapper that contains all drag/drop/resize/toolbar features
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="nodeConfig"></param>
        /// <returns></returns>
        public static string getNodeHtml(CPBaseClass cp, ConfigNodeModel nodeConfig) {
            string result = "";
            try {
                string AddonOptions = "";
                foreach (var nvp in nodeConfig.addonArgList) {
                    AddonOptions += $"&{nvp.name}={nvp.value}";
                }
                AddonOptions = string.IsNullOrEmpty(AddonOptions) ? "" : AddonOptions.Substring(1);
                int WrapperWidth = 77;
                int WrapperHeight = 77;
                int IconWidth = 57;
                int IconHeight = 59;
                int IconSprites = 1;
                bool IsIcon = true;
                string IconFileName = "";
                string ShortcutHref = "";
                string ItemHTMLClass = "";
                string ToolBar = "";
                string title = "";
                AddonModel addon = null;
                string IconHtml = "";
                if (!string.IsNullOrEmpty(nodeConfig.addonGUID) && nodeConfig.addonGUID != "0") {
                    //
                    // -- render addon (addonGuid legacy is id)
                    if (int.TryParse(nodeConfig.addonGUID, out int addonId) && nodeConfig.addonGUID.Length < 10) {
                        addon = DbBaseModel.create<AddonModel>(cp, addonId);
                    } else {
                        addon = DbBaseModel.create<AddonModel>(cp, nodeConfig.addonGUID);
                    }
                    if (addon is null) {
                        //
                        // -- this node is not valid, exit
                        return string.Empty;
                    }
                    title = addon.name;
                    if (nodeConfig.state == ConfigNodeState.closed) {
                        //
                        // Addon is just an icon
                        ItemHTMLClass = "dashNode iconNode";
                        IsIcon = true;
                        IconFileName = addon.iconFilename;
                        IconWidth = cp.Utils.EncodeInteger(addon.iconWidth);
                        IconHeight = cp.Utils.EncodeInteger(addon.iconHeight);
                        IconSprites = cp.Utils.EncodeInteger(addon.iconSprites);
                        IconHtml = addon.iconHtml;
                        ShortcutHref = string.IsNullOrWhiteSpace(nodeConfig.link) ? $"?addonid={addon.id}" : nodeConfig.link;
                        ToolBar += $"<a alt=\"Remove from dashboard\" title=\"Remove from dashboard\" href=\"#\" onClick=\"dashDeleteNode('{nodeConfig.key}','{nodeConfig.key}');return false;\"><i title=\"close\" class=\"fas fa-window-close\" style=\"color:#222\"></i></a>";
                    } else {
                        //
                        // open Addon running in a wrapper
                        ItemHTMLClass = "dashNode windowNode";
                        IsIcon = false;
                        WrapperWidth = nodeConfig.sizex;
                        WrapperHeight = nodeConfig.sizey;
                        ShortcutHref = string.IsNullOrWhiteSpace(nodeConfig.link) ? "" : nodeConfig.link;
                        ToolBar += $"<div style=\"float:left\">{title}</div>";
                        ToolBar += $"<a alt=\"Minimize\" title=\"Minimize\" href=\"#\" onClick=\"closeNode('{nodeConfig.key}','{nodeConfig.key}');return false;\"><i title=\"restore\" class=\"fas fa-window-minimize\" style=\"color:#222\"></i></a>";
                        ToolBar += $"<a alt=\"Remove from dashboard\" title=\"Remove from dashboard\" href=\"#\" onClick=\"dashDeleteNode('{nodeConfig.key}','{nodeConfig.key}');return false;\"><i title=\"close\" class=\"fas fa-window-close\" style=\"color:#222\"></i></a>";
                    }
                    if (string.IsNullOrWhiteSpace(IconHtml) && string.IsNullOrEmpty(IconFileName)) {
                        IconFileName = "/dashboard/addon.png";
                        IconWidth = 57;
                        IconHeight = 59;
                        IconSprites = 4;
                    }
                } else if (!string.IsNullOrEmpty(nodeConfig.contentGUID) || !string.IsNullOrEmpty(nodeConfig.contentName)) {
                    //
                    // -- Node is content
                    IsIcon = true;
                    ItemHTMLClass = "dashNode iconNode";
                    ContentModel content = null;
                    if (!string.IsNullOrEmpty(nodeConfig.contentGUID)) {
                        content = DbBaseModel.create<ContentModel>(cp, nodeConfig.contentGUID);
                    } else {
                        content = DbBaseModel.createByUniqueName<ContentModel>(cp, nodeConfig.contentName);
                    }
                    if (content is null) {
                        //
                        // -- this node is not valid
                        return string.Empty;
                    }
                    IconHtml = content.iconHtml;
                    IconFileName = content.iconLink;
                    IconWidth = content.iconWidth;
                    IconHeight = content.iconHeight;
                    IconSprites = content.iconSprites;
                    ShortcutHref = string.IsNullOrWhiteSpace(nodeConfig.link) ? $"?cid={content.id}" : nodeConfig.link;
                    content.id = 0;
                    AddonOptions = "";
                    nodeConfig.state = ConfigNodeState.closed;
                    title = content.name;
                    ToolBar = $"<a href=\"#\" onClick=\"dashDeleteNode('{nodeConfig.key}','{nodeConfig.key}');return false;\"><i title=\"close\" class=\"fas fa-window-close\" style=\"color:#222\"></i></a>";
                    if (string.IsNullOrWhiteSpace(IconHtml) && string.IsNullOrEmpty(IconFileName)) {
                        IconFileName = "/dashboard/content.png";
                        IconWidth = 57;
                        IconHeight = 59;
                        IconSprites = 4;
                    }
                } else {
                    //
                    // -- simple link icon
                    IsIcon = true;
                    ItemHTMLClass = "dashNode iconNode";
                    IconFileName = "/dashboard/content.png";
                    IconWidth = 57;
                    IconHeight = 59;
                    IconSprites = 4;
                    ShortcutHref = nodeConfig.link;
                    nodeConfig.state = ConfigNodeState.closed;
                    title = nodeConfig.title;
                    ToolBar = $"<a href=\"#\" onClick=\"dashDeleteNode('{nodeConfig.key}','{nodeConfig.key}');return false;\"><i title=\"close\" class=\"fas fa-window-close\" style=\"color:#222\"></i></a>";
                }
                //
                // -- Build inner content of icon or addon
                string DoDadContent = "";
                if (nodeConfig.state == ConfigNodeState.closed) {
                    //
                    // Icon Shortcut (Content or Addon)
                    if (string.IsNullOrWhiteSpace(IconHtml)) {
                        //
                        // -- image based icon
                        if (IconSprites < 2) {
                            //
                            // Flat image
                            IconHtml = $"<img title=\"{title}\" border=\"0\" src=\"{IconFileName}\">";
                        } else {
                            //
                            // Sprites
                            IconHtml = Controllers.DashboardIconController.getDashboardIconHtml("", IconWidth, IconHeight, IconSprites, true, "", IconFileName, cp.Http.CdnFilePathPrefix, title, title, "", 0L, IconHtml);
                        }
                    }
                    DoDadContent = $"\r\n\t\t<a class=\"shortcut\" href=\"{ShortcutHref}\">{IconHtml}<div>{title}</div></a>";
                } else if (addon is not null) {
                    //
                    // -- execute the addon
                    DoDadContent = cp.Addon.Execute(addon.ccguid);
                }
                //
                // -- add wrapper (deprecated)
                string DodadWrappedContent = DoDadContent;
                string DesignResizerHTMLId = $"designResizer{nodeConfig.key}";
                //
                // -- add Design-Resizer (an inner wrapper to use for resizing the design wrapper with the control wrapper)
                if (!IsIcon) {
                    //
                    // non-icon, add resize
                    DodadWrappedContent = $"<div id=\"{DesignResizerHTMLId}\" class=\"designResizer\" style=\"height: {WrapperHeight}px;\">{DodadWrappedContent}</div>";
                }
                //
                // Build Toolbar wrapper
                ToolBar = $"<div class=\"toolBarInner\"><div class=\"toolBarGrip\"><i class=\"fas fa-grip-horizontal\" style=\"color:#222;\"></i></div>{ToolBar}</div>";
                string ToolBarHTMLId = $"toolBar{nodeConfig.key}";
                ToolBar = $"<div class=\"toolBar\" onmouseout=\"outTools(this)\" onmouseover=\"overTools(this)\" id=\"{ToolBarHTMLId}\">{ToolBar}</div>";
                //
                // Assemble common elements from all item types
                string Dodad = $"<div id=\"{nodeConfig.key}\" class=\"{ItemHTMLClass}\" onmouseout=\"outBody('{ToolBarHTMLId}')\" onmouseover=\"overBody('{ToolBarHTMLId}')\" style=\"top:{nodeConfig.y}px; left:{nodeConfig.x}px; width: {WrapperWidth}px;\">{ToolBar}{DodadWrappedContent}</div>";
                //
                // ----- Control Wrapper - Make this new item resizeable and draggable
                string ResizeableJS = "";
                string HandleJS = "";
                if (!IsIcon) {
                    //
                    // non-icon
                    ResizeableJS = $".resizable({{alsoResize: '#{DesignResizerHTMLId}',stop: function(event, ui) {{var e=document.getElementById('{nodeConfig.key}');var r=document.getElementById('{DesignResizerHTMLId}');fetch('/dashboardresize?ptr={nodeConfig.key}&x='+e.style.width+'&y='+r.style.height);dashResize();}}}})";
                    HandleJS = $",handle: '#{ToolBarHTMLId}'";
                }
                result = Dodad;
            } catch (ArgumentException ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}
