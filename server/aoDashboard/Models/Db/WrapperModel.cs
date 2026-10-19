
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.Dashboard.Models {
    public class WrapperModel : BaseModel, ICloneable {
        // 
        // ====================================================================================================
        // -- const
        public const string contentName = "wrappers";
        public const string contentTableName = "ccWrappers";
        private const string contentDataSource = "default";
        // 
        // ====================================================================================================
        // -- instance properties
        public string copytext { get; set; }
        public string javascriptbodyend { get; set; }
        public string javascriptinhead { get; set; }
        public string javascriptonload { get; set; }
        public string JSFilename { get; set; }
        public string OtherHeadTags { get; set; }
        public string stylesfilename { get; set; }
        public string targetstring { get; set; }
        // 
        // ====================================================================================================
        public static WrapperModel @add(CPBaseClass cp) {
            return @add<WrapperModel>(cp);
        }
        // 
        // ====================================================================================================
        public static WrapperModel create(CPBaseClass cp, int recordId) {
            return create<WrapperModel>(cp, recordId);
        }
        // 
        // ====================================================================================================
        public static WrapperModel create(CPBaseClass cp, string recordGuid) {
            return create<WrapperModel>(cp, recordGuid);
        }
        // 
        // ====================================================================================================
        public static WrapperModel createByName(CPBaseClass cp, string recordName) {
            return createByName<WrapperModel>(cp, recordName);
        }
        // 
        // ====================================================================================================
        public new void save(CPBaseClass cp) {
            base.save(cp);
        }
        // 
        // ====================================================================================================
        public static void delete(CPBaseClass cp, int recordId) {
            delete<WrapperModel>(cp, recordId);
        }
        // 
        // ====================================================================================================
        public static void delete(CPBaseClass cp, string ccGuid) {
            delete<WrapperModel>(cp, ccGuid);
        }
        // 
        // ====================================================================================================
        public static List<WrapperModel> createList(CPBaseClass cp, string sqlCriteria, string sqlOrderBy = "id") {
            return createList<WrapperModel>(cp, sqlCriteria, sqlOrderBy);
        }
        // 
        // ====================================================================================================
        public static string getRecordName(CPBaseClass cp, int recordId) {
            return getRecordName<WrapperModel>(cp, recordId);
        }
        // 
        // ====================================================================================================
        public static string getRecordName(CPBaseClass cp, string ccGuid) {
            return getRecordName<WrapperModel>(cp, ccGuid);
        }
        // 
        // ====================================================================================================
        public static int getRecordId(CPBaseClass cp, string ccGuid) {
            return getRecordId<WrapperModel>(cp, ccGuid);
        }
        // 
        // ====================================================================================================
        public static int getCount(CPBaseClass cp, string sqlCriteria) {
            return getCount<WrapperModel>(cp, sqlCriteria);
        }
        // 
        // ====================================================================================================
        public string getUploadPath(string fieldName) {
            return getUploadPath<WrapperModel>(fieldName);
        }
        // 
        // ====================================================================================================
        // 
        public WrapperModel Clone(CPBaseClass cp) {
            WrapperModel result = (WrapperModel)Clone();
            result.id = cp.Content.AddRecord(contentName);
            result.ccguid = cp.Utils.CreateGuid();
            result.save(cp);
            return result;
        }
        // 
        // ====================================================================================================
        // 
        public object Clone() {
            return MemberwiseClone();
        }
        // 
        public static Dictionary<string, WrapperModel> getGuidDictionary(CPBaseClass cp) {
            var result = new Dictionary<string, WrapperModel>();
            try {
                var wrapperList = createList(cp, "");
                foreach (WrapperModel wrapper in wrapperList) {
                    if (!result.ContainsKey(wrapper.ccguid)) {
                        result.Add(wrapper.ccguid, wrapper);
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
    }
}