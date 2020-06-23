using System.Collections.Generic;

namespace FortniteInformation
{
    class AppManifest
    {

        public string appName;
        public string labelName;
        public string buildVersion;
        public string catalogItemId;
        public string expires;
        public string asset_id;
        public Dictionary<string, AppManifestItem> items;

    }
}
