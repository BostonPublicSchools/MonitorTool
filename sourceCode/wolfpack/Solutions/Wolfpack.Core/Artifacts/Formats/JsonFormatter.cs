using System;
using System.IO;
using Wolfpack.Core.Interfaces;

namespace Wolfpack.Core.Artifacts.Formats
{
    public class JsonFormatter : IArtifactFormatter
    {
        public string ContentType
        {
            get { return "application/json"; }
        }

        public Stream Serialize(object data)
        {
            throw new NotImplementedException();
        }

        public void Validate(object data)
        {
            throw new NotImplementedException();
        }
    }
}