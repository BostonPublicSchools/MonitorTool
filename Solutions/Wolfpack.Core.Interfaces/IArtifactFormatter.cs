using System.IO;

namespace Wolfpack.Core.Interfaces
{
    public interface IArtifactFormatter
    {
        string ContentType { get; }
        Stream Serialize(object data);
        void Validate(object data);
    }
}