
using System.Collections.Generic;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Wolfpack.Core.WebServices.Interfaces
{
    public interface IWebServiceExtender
    {
        IEnumerable<ModuleRegistration> Modules { get; }
        void Execute(TinyIoCContainer container, IPipelines pipelines);
    }
}