using System;
using System.Collections.Generic;
using System.Text;

namespace FortniteInformation
{
    class ChunkManifest
    {

        public string AppNameString;
        public string BuildVersionString;
        public List<ChunkManifestFile> FileManifestList;
        public Dictionary<string, string> ChunkHashList;
        public Dictionary<string, string> DataGroupList;

    }
}
