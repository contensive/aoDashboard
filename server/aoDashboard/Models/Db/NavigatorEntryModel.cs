
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.Dashboard.Models {
    public class NavigatorEntryModel : BaseModel, ICloneable {
        // 
        // ====================================================================================================
        // -- const
        public const string contentName = "Navigator Entries";
        public const string contentTableName = "ccMenuEntries";
        private const string contentDataSource = "default";
        // 
        // ====================================================================================================
        /// <summary>
        /// The link to the entries action
        /// </summary>
        /// <returns></returns>
        public string linkpage { get; set; }
        public int addonId { get; set; }
        public int contentId { get; set; }

        // 
        // ====================================================================================================
        public static NavigatorEntryModel @add(CPBaseClass cp) {
            return @add<NavigatorEntryModel>(cp);
        }
        // 
        // ====================================================================================================
        public static NavigatorEntryModel create(CPBaseClass cp, int recordId) {
            return create<NavigatorEntryModel>(cp, recordId);
        }
        // 
        // ====================================================================================================
        public static NavigatorEntryModel create(CPBaseClass cp, string recordGuid) {
            return create<NavigatorEntryModel>(cp, recordGuid);
        }
        // 
        // ====================================================================================================
        public static NavigatorEntryModel createByName(CPBaseClass cp, string recordName) {
            return createByName<NavigatorEntryModel>(cp, recordName);
        }
        // 
        // ====================================================================================================
        public new void save(CPBaseClass cp) {
            base.save(cp);
        }
        // 
        // ====================================================================================================
        public static void delete(CPBaseClass cp, int recordId) {
            delete<NavigatorEntryModel>(cp, recordId);
        }
        // 
        // ====================================================================================================
        public static void delete(CPBaseClass cp, string ccGuid) {
            delete<NavigatorEntryModel>(cp, ccGuid);
        }
        // 
        // ====================================================================================================
        public static List<NavigatorEntryModel> createList(CPBaseClass cp, string sqlCriteria, string sqlOrderBy = "id") {
            return createList<NavigatorEntryModel>(cp, sqlCriteria, sqlOrderBy);
        }
        // 
        // ====================================================================================================
        public static string getRecordName(CPBaseClass cp, int recordId) {
            return getRecordName<NavigatorEntryModel>(cp, recordId);
        }
        // 
        // ====================================================================================================
        public static string getRecordName(CPBaseClass cp, string ccGuid) {
            return getRecordName<NavigatorEntryModel>(cp, ccGuid);
        }
        // 
        // ====================================================================================================
        public static int getRecordId(CPBaseClass cp, string ccGuid) {
            return getRecordId<NavigatorEntryModel>(cp, ccGuid);
        }
        // 
        // ====================================================================================================
        public static int getCount(CPBaseClass cp, string sqlCriteria) {
            return getCount<NavigatorEntryModel>(cp, sqlCriteria);
        }
        // 
        // ====================================================================================================
        public string getUploadPath(string fieldName) {
            return getUploadPath<NavigatorEntryModel>(fieldName);
        }
        // 
        // ====================================================================================================
        // 
        public NavigatorEntryModel clone(CPBaseClass cp) {
            NavigatorEntryModel result = (NavigatorEntryModel)Clone();
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

    }
}