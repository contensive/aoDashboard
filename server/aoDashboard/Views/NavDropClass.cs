
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.Dashboard.Views {
    public class NavDropClass : AddonBaseClass {
        /// <summary>
        /// remote method called when new node is added by dragging an admin navigator
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass CP) {
            string result = "";
            try {
                if (!CP.User.IsAuthenticated) { return ""; }
                //
                //
                var request = new Models.RequestModel(CP);
                if (request?.id is null)
                    return result;
                // 
                var config = Models.ConfigModel.create(CP, CP.User.Id);
                var wrapper = Models.WrapperModel.create(CP, config.defaultWrapper.guid);
                switch (request.id.ToLower().Substring(0, 1) ?? "") {
                    case "a": {
                            // 
                            // -- An Addon was dragged onto the desktop
                            int requestAddonId = CP.Utils.EncodeInteger(request.id.Substring(1));
                            if (requestAddonId > 0) {
                                var addon = DbBaseModel.create<AddonModel>(CP, requestAddonId);
                                if (addon is not null) {
                                    string AddonName = addon.name;
                                    string IconFileName = addon.iconFilename;
                                    int IconWidth;
                                    int IconHeight;
                                    int IconSprites;
                                    if (string.IsNullOrWhiteSpace(IconFileName)) {
                                        // 
                                        // -- Default Icon
                                        IconFileName = "/dashboard/addon.png";
                                        IconWidth = 57;
                                        IconHeight = 59;
                                        IconSprites = 4;
                                    } else {
                                        // 
                                        // -- Custom Icon
                                        IconWidth = CP.Utils.EncodeInteger(addon.iconWidth);
                                        IconHeight = CP.Utils.EncodeInteger(addon.iconHeight);
                                        IconSprites = CP.Utils.EncodeInteger(addon.iconSprites);
                                    }
                                    if (string.IsNullOrEmpty(addon.ccguid)) {
                                        addon.ccguid = CP.Utils.CreateGuid();
                                    }
                                    string ShortcutHref = "?addonid=" + addon.id;
                                    // 
                                    // -- Add this add-on to the config and return the Icon
                                    string nodeKey = "addon" + addon.id + "-" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
                                    var newNode = new Models.ConfigNodeModel() {
                                        addonArgList = new List<Models.NameValueModel>(),
                                        addonGUID = addon.ccguid,
                                        contentGUID = "",
                                        contentName = "",
                                        link = "",
                                        key = nodeKey,
                                        sizex = 200,
                                        sizey = 300,
                                        state = Models.ConfigNodeState.closed,
                                        title = addon.name,
                                        wrapperId = wrapper is null ? 0 : wrapper.id,
                                        x = request.x,
                                        y = request.y,
                                        z = 9999
                                    };
                                    config.nodeList.Add(nodeKey, newNode);
                                    config.save(CP);
                                    result = Models.NodeModel.getNodeHtml(CP, newNode);
                                }
                            }

                            break;
                        }
                    case "c": {
                            // 
                            // -- A content link was dragged onto the desktop
                            int contentId = CP.Utils.EncodeInteger(request.id.Substring(1));
                            if (contentId > 0) {
                                var content = DbBaseModel.create<ContentModel>(CP, contentId);
                                if (content is not null) {
                                    if (string.IsNullOrEmpty(content.iconLink)) {
                                        // 
                                        // -- Default Icon
                                        content.iconLink = "/cclib/images/icons/content.png";
                                        content.iconWidth = 57;
                                        content.iconHeight = 59;
                                        content.iconSprites = 4;
                                    }
                                    string ShortcutHref = "?cid=" + content.id;
                                    // 
                                    // Add this add-on to the config and return the Icon
                                    string nodeKey = "content" + content.id + "-" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
                                    var newNode = new Models.ConfigNodeModel() {
                                        addonArgList = new List<Models.NameValueModel>(),
                                        addonGUID = "",
                                        contentGUID = content.ccguid,
                                        contentName = content.name,
                                        link = "",
                                        key = nodeKey,
                                        sizex = 77,
                                        sizey = 77,
                                        state = Models.ConfigNodeState.closed,
                                        title = content.name,
                                        wrapperId = wrapper is null ? 0 : wrapper.id,
                                        x = request.x,
                                        y = request.y,
                                        z = 9999
                                    };
                                    config.nodeList.Add(nodeKey, newNode);
                                    config.save(CP);
                                    result = Models.NodeModel.getNodeHtml(CP, newNode);
                                }
                            }

                            break;
                        }
                    case "n": {
                            // 
                            // -- A navigator entry not realted to content or addon
                            int navigatorId = CP.Utils.EncodeInteger(request.id.Substring(1));
                            if (navigatorId > 0) {
                                var navigatorEntry = Models.NavigatorEntryModel.create(CP, navigatorId);
                                if (navigatorEntry is not null) {
                                    // 
                                    // Add this add-on to the config and return the Icon
                                    string nodeKey = "nav" + navigatorEntry.id + "-" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
                                    var newNode = new Models.ConfigNodeModel() {
                                        addonArgList = new List<Models.NameValueModel>(),
                                        addonGUID = "",
                                        contentGUID = "",
                                        contentName = "",
                                        link = navigatorEntry.linkpage,
                                        key = nodeKey,
                                        sizex = 77,
                                        sizey = 77,
                                        state = Models.ConfigNodeState.closed,
                                        title = navigatorEntry.name,
                                        wrapperId = wrapper is null ? 0 : wrapper.id,
                                        x = request.x,
                                        y = request.y,
                                        z = 9999
                                    };
                                    config.nodeList.Add(nodeKey, newNode);
                                    config.save(CP);
                                    result = Models.NodeModel.getNodeHtml(CP, newNode);
                                }
                            }

                            break;
                        }

                    default: {
                            result = "<!-- no object found for this id -->";
                            break;
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