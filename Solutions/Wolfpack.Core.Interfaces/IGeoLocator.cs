namespace Wolfpack.Core.Interfaces
{
    using Entities;

    public interface IGeoLocator
    {
        GeoData Locate();
    }
}