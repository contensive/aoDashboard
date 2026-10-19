
using Contensive.BaseClasses;
using System;
using System.Xml;

namespace Contensive.Addons.Dashboard.Controllers {
    public class DashboardIconController {
        //
        // ====================================================================================================
        /// <summary>
        /// create addon icon
        /// </summary>
        /// <param name="AdminURL"></param>
        /// <param name="IconWidth"></param>
        /// <param name="IconHeight"></param>
        /// <param name="IconSprites"></param>
        /// <param name="IconIsInline"></param>
        /// <param name="IconImgID"></param>
        /// <param name="IconFilename"></param>
        /// <param name="serverFilePath"></param>
        /// <param name="IconAlt"></param>
        /// <param name="IconTitle"></param>
        /// <param name="ACInstanceID"></param>
        /// <param name="IconSpriteColumn"></param>
        /// <returns></returns>
        public static string getDashboardIconHtml(string AdminURL, long IconWidth, long IconHeight, long IconSprites, bool IconIsInline, string IconImgID, string IconFilename, string serverFilePath, string IconAlt, string IconTitle, string ACInstanceID, long IconSpriteColumn, string iconHtml) {
            string result = "";
            //
            if (string.IsNullOrEmpty(IconAlt)) {
                IconAlt = "Add-on";
            }
            if (string.IsNullOrEmpty(IconTitle)) {
                IconTitle = "Rendered as Add-on";
            }
            if (string.IsNullOrEmpty(IconFilename)) {
                //
                // No icon given, use the default
                //
                if (IconIsInline) {
                    IconFilename = "/ccLib/images/IconAddonInlineDefault.png";
                    IconWidth = 62L;
                    IconHeight = 17L;
                    IconSprites = 0L;
                } else {
                    IconFilename = "/ccLib/images/IconAddonBlockDefault.png";
                    IconWidth = 57L;
                    IconHeight = 59L;
                    IconSprites = 4L;
                }
            } else if (IconFilename.Contains("://")) {
                //
                // icon is an Absolute URL - leave it
                //
            } else if (IconFilename.StartsWith("/")) {
                //
                // icon is Root Relative, leave it
                //
            } else {
                //
                // icon is a virtual file, add the serverfilepath
                //
                IconFilename = serverFilePath + IconFilename;
            }
            if (IconWidth == 0L || IconHeight == 0L) {
                IconSprites = 0L;
            }

            if (IconSprites == 0L) {
                //
                // just the icon
                //
                result = $"<img border=0 id=\"{IconImgID}\" onDblClick=\"window.parent.OpenAddonPropertyWindow(this,'{AdminURL}');\" alt=\"{IconAlt}\" title=\"{IconTitle}\" src=\"{IconFilename}\"";
                if (IconWidth != 0L) {
                    result += $" width=\"{IconWidth}px\"";
                }
                if (IconHeight != 0L) {
                    result += $" height=\"{IconHeight}px\"";
                }
                if (IconIsInline) {
                    result += " style=\"vertical-align:middle;display:inline;\" ";
                } else {
                    result += " style=\"display:block\" ";
                }
                if (!string.IsNullOrEmpty(ACInstanceID)) {
                    result += $" ACInstanceID=\"{ACInstanceID}\"";
                }
                return result + ">";
            } else {
                //
                // Sprite Icon
                //
                return getIconSprite(IconImgID, IconSpriteColumn, IconFilename, IconWidth, IconHeight, IconAlt, IconTitle, $"window.parent.OpenAddonPropertyWindow(this,'{AdminURL}');", IconIsInline, ACInstanceID);
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// create an icon as a 4 image sprite
        /// </summary>
        /// <param name="TagID"></param>
        /// <param name="SpriteColumn"></param>
        /// <param name="IconSrc"></param>
        /// <param name="IconWidth"></param>
        /// <param name="IconHeight"></param>
        /// <param name="IconAlt"></param>
        /// <param name="IconTitle"></param>
        /// <param name="onDblClick"></param>
        /// <param name="IconIsInline"></param>
        /// <param name="ACInstanceID"></param>
        /// <returns></returns>
        public static string getIconSprite(string TagID, long SpriteColumn, string IconSrc, long IconWidth, long IconHeight, string IconAlt, string IconTitle, string onDblClick, bool IconIsInline, string ACInstanceID) {
            //
            long bgPosX = -1 * SpriteColumn * IconWidth;
            long hoverPosY = 2L * IconHeight;
            string result = $"<img border=0 id=\"{TagID}\" onMouseOver=\"this.style.backgroundPosition='{bgPosX}px -{hoverPosY}px';\" onMouseOut=\"this.style.backgroundPosition='{bgPosX}px 0px'\" onDblClick=\"{onDblClick}\" alt=\"{IconAlt}\" title=\"{IconTitle}\" src=\"https://s3.amazonaws.com/cdn.contensive.com/assets/20191111/images/spacer.gif\"";
            string ImgStyle = $"background:url({IconSrc}) {bgPosX}px 0px no-repeat;width:{IconWidth}px;height:{IconHeight}px;";
            if (IconIsInline) {
                ImgStyle += "vertical-align:middle;display:inline;";
            } else {
                ImgStyle += "display:block;";
            }
            if (!string.IsNullOrEmpty(ACInstanceID)) {
                result += $" ACInstanceID=\"{ACInstanceID}\"";
            }
            result += $" style=\"{ImgStyle}\">";
            return result;
        }
        //
        // ====================================================================================================
        /// <summary>
        /// get xml attribute
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        internal static string getAttribute(CPBaseClass cp, XmlNode node, string attrName) {
            string result = "";
            try {
                var attr = node.Attributes[attrName];
                if (attr is not null) {
                    result = attr.Value;
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }

}
