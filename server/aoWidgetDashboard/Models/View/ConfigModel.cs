﻿using Contensive.BaseClasses;
using Contensive.WidgetDashboard.Controllers;
using Contensive.WidgetDashboard.Models.Domain;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data;

namespace Contensive.WidgetDashboard.Models.View {
    internal class ConfigModel {
        //
        private CPBaseClass cp;
        public List<addWidget> addWidgetList { get; set; }
        public List<ConfigWidgetModel> widgets { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        /// Create the dashboard viewModel from the config file of the user and render the htmlContent.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ConfigModel create(CPBaseClass cp) {
            try {
                ConfigModel config = load(cp);
                if (config?.widgets != null && config.widgets.Count > 0) {
                    //
                    // -- render the htmlcontent and reutrn
                    ConfigModel result = WidgetRenderController.renderWidgets(cp, config);
                    buildAddWidgetList(cp, result);
                    return result;
                }
                //
                // -- iniitalize with default widgets
                config = new ConfigModel() {
                    widgets = [
                        new ConfigWidgetModel() {
                            x=0,
                            y=0,
                            width = 2,
                            height = 2,
                            htmlContent = cp.CdnFiles.Read("dashboard\\sampleWidget.html"),
                            key="E928",
                            link="https://www.contensive.com",
                            addonGuid = Constants.sampleDashboardWidgetGuid
                        },
                        new ConfigWidgetModel() { x=2,y=0, width = 2, height = 2, htmlContent = "Widget 2", key="6E52", link="https://www.contensive.com" },
                        new ConfigWidgetModel() { x=4,y=0, width = 1, height = 1, htmlContent = "Widget 3", key="D512", link="https://www.contensive.com" },
                        new ConfigWidgetModel() { x=4,y=1, width = 1, height = 1, htmlContent = "Widget 4", key="0380", link="https://www.contensive.com" },
                        new ConfigWidgetModel() { x=5,y=0, width = 2, height = 2, htmlContent = "Widget 5", key="AC55", link="https://www.contensive.com" }
                    ]
                };
                //
                // -- save the new view model before rendering the htmlcontent
                config.save(cp);
                //
                // -- after save, render the htmlContent and get the widget list
                ConfigModel result2 = WidgetRenderController.renderWidgets(cp, config);
                buildAddWidgetList(cp, result2);
                return result2;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }

        private static void buildAddWidgetList(CPBaseClass cp, ConfigModel result) {
            //
            if (cp.Db.IsTableField("ccAggregateFunctions", "adminWidget")) {
                result.addWidgetList = new List<addWidget>();
                using (DataTable dt = cp.Db.ExecuteQuery("select name,ccguid from ccAggregateFunctions where adminWidget>0 order by name")) {
                    if (dt.Rows.Count > 0) {
                        result.addWidgetList = new List<addWidget>();
                        foreach (DataRow row in dt.Rows) {
                            result.addWidgetList.Add(new addWidget() {
                                name = cp.Utils.EncodeText(row["name"]),
                                guid = cp.Utils.EncodeText(row["ccguid"])
                            });
                        }
                    }
                }
            }
        }

        //
        // ====================================================================================================
        /// <summary>
        /// load config for a user. Returns null if config file not found
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static ConfigModel load(CPBaseClass cp) {
            ConfigModel result = null;
            string userConfigFilename = @"dashboard\widgetdashconfig." + cp.User.Id + ".json";
            string jsonConfigText = cp.PrivateFiles.Read(userConfigFilename);
            if (!string.IsNullOrWhiteSpace(jsonConfigText)) {
                result = cp.JSON.Deserialize<ConfigModel>(jsonConfigText);
            }
            return result;
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// save config for the current user
        /// </summary>
        /// <param name="cp"></param>
        public void save(CPBaseClass cp) {
            cp.PrivateFiles.Save(@"dashboard\widgetdashconfig." + cp.User.Id + ".json", cp.JSON.Serialize(this));
        }
    }
    //
    public class addWidget {
        public string name { get; set; }
        public string guid { get; set; }
    }   
}
