using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Messages;

namespace Wolfpack.Core.WebServices.Modules
{
    public class ConfigurationModule : NancyModule
    {
        private const string BaseUrl = "/api/configuration";

        private readonly WebServiceActivityConfig _config;

        public ConfigurationModule(WebServiceActivityConfig config)
            : base(BaseUrl)
        {
            _config = config;
            Get["/tagcloud"] = _ => Response.AsJson(ConfigurationManager.GetTagCloud().ToList());
            Get["/catalogue"] = GetCatalogue;
            Post["/changerequest"] = PostChangeRequest;
            Get["/applychanges"] = GetApplyChanges;
            Get["/cancelchanges"] = GetDiscardPendingChanges;
        }

        private dynamic GetApplyChanges(dynamic request)
        {
            try
            {
                ConfigurationManager.ApplyPendingChanges(Request.Query.Restart);
                return new ConfigurationCommandResponse { Result = true };
            }
            catch (Exception e)
            {
                return new ConfigurationCommandResponse { Result = false, Error = e };
            }            
        }
        private dynamic GetDiscardPendingChanges(dynamic request)
        {
            try
            {
                ConfigurationManager.DiscardPendingChanges();
                return new ConfigurationCommandResponse { Result = true };
            }
            catch (Exception e)
            {
                return new ConfigurationCommandResponse { Result = false, Error = e };
            }            
        }

        private dynamic PostChangeRequest(dynamic request)
        {
            try
            {
                var changeRequest = this.Bind<RestConfigurationChangeRequest>();
                ConfigurationManager.Update(changeRequest.ToChangeRequest());
                return Response.AsJson(new ConfigurationCommandResponse { Result = true });
            }
            catch (Exception e)
            {
                return Response.AsJson(new ConfigurationCommandResponse { Result = false, Error = e });
            }
        }

        private dynamic GetCatalogue(dynamic request)
        {
            var catalogueRequest = this.Bind<GetConfigurationCatalogue>();
            var catalogue = ConfigurationManager.GetCatalogue(catalogueRequest.Tags);
            var response = new RestConfigurationCatalogue
            {
                InstanceId = catalogue.InstanceId,
                Links = new List<RestLink>
                {
                    new RestLink
                    {
                        Action = "Accept Changes",
                        Link = BuildLink("applychanges"),
                        Method = "GET"
                    }
                },
                Pending = catalogue.Pending.Select(
                    ce =>
                    {
                        var entry = new RestConfigurationChangeSummary(ce);
                        entry.Links.Add(new RestLink
                        {
                            Action = "Undo change",
                            Method = "GET",
                            Link = BuildLink("configuration/undo?id={0}", ce.Id)
                        });
                        return entry;
                    }
                    ).ToList(),
                Items = catalogue.Items.Select(
                    ce =>
                    {
                        var entry = new RestCatalogueEntry(ce);
                        entry.Links.Add(new RestLink
                        {
                            Action = "Create a new instance",
                            Method = "POST",
                            Link = BuildLink("configuration")
                        });
                        return entry;
                    }).ToList()
            };

            return Response.AsJson(response);
        }

        private string BuildLink(string relativePathTemplate, params object[] args)
        {
            var relativePath = string.Format(relativePathTemplate, args).TrimStart('/');
            return string.Format("{0}{1}/{2}", _config.BaseUrl.TrimEnd('/'),
                BaseUrl, relativePath);
        }
    }
}