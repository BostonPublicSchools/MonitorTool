using System;

namespace Wolfpack.Core.Interfaces
{
    public interface ITypeDiscovery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        bool Locate(Type interfaceType, out Type[] matchingTypes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="matchingTypes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool Locate(Type interfaceType, Predicate<Type> filter, out Type[] matchingTypes);
    }
}