using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Checks
{
    public class QueryCheckConfigBase : PluginConfigBase, ISupportNotificationMode, ISupportResultInversion, ISupportArtifactGeneration
    {
        public string Query { get; set; }

        /// <summary>
        /// By default, zero rows returned from a query would be interpretted
        /// as a success (eg: you are looking for critial applications errors
        /// in the event log). If you are expecting rows to be returned then
        /// set this to true should no rows be returned.
        /// </summary>
        public bool InterpretZeroRowsAsAFailure { get; set; }

        public string NotificationMode { get; set; }

        public bool? GenerateArtifacts { get; set; }
    }
}