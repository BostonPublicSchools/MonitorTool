using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core
{
    /// <summary>
    /// This will scan all the assemblies (*.dll|exe) in a given folder for a given set 
    /// of types and will return all the Type(s) that implement the interfaces specified
    /// </summary>
    public class TypeDiscovery : ITypeDiscovery
    {
        private static readonly string _binFolder;
        private static readonly TypeDiscoveryConfig _config;
        private static List<string> _filesToExclude;

        static TypeDiscovery()
        {
            _binFolder = SmartLocation.GetBinFolder();
            _config = Container.Resolve<TypeDiscoveryConfig>();

            _filesToExclude = new List<string>();
            var exclude = new List<string>();

            if (_config.Exclude == null) 
                return;

            _config.Exclude.ForEach(e => exclude.AddRange(Directory.GetFiles(_binFolder, e)));
            _filesToExclude = exclude.Distinct().ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TypeDiscovery()
        {
        }

        /// <summary>
        /// Simple helper to locate a single type in *.dll|exe
        /// </summary>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        public static bool Discover<T>(out Type[] matchingTypes)
        {
            return Discover<T>(t => true, out matchingTypes);
        }

        /// <summary>
        /// Simple helper to locate a single type in *.dll|exe
        /// </summary>
        /// <param name="matchingTypes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool Discover<T>(Predicate<Type> filter, out Type[] matchingTypes)
        {
            return Discover(typeof(T), filter, out matchingTypes);
        }

        /// <summary>
        /// Simple helper to locate matching types in *.dll|exe
        /// </summary>
        /// <param name="matchingTypes"></param>
        /// <param name="interfaceType"> </param>
        /// <returns></returns>
        public static bool Discover(Type interfaceType, out Type[] matchingTypes)
        {
            return Discover(interfaceType, x => true, out matchingTypes);
        }

        public static bool Discover(Type interfaceType, Predicate<Type> filter, out Type[] matchingTypes)
        {
            return new TypeDiscovery().Locate(interfaceType, filter, out matchingTypes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        public bool Locate(Type interfaceType, out Type[] matchingTypes)
        {
            return Locate(interfaceType, x => true, out matchingTypes);
        }

        public bool Locate(Type interfaceType, Predicate<Type> filter, out Type[] matchingTypes)
        {
            Logger.Info("Scanning for Type: {0}", interfaceType.Name);

            var candidates = Directory.GetFiles(_binFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));
            var filesToScan = candidates.Except(_filesToExclude);

            var implementingTypes = new List<Type>();

            filesToScan.ForEach(
                assemblyFile =>
                    {
                        try
                        {
                            Logger.Debug("\tScanning {0}...", Path.GetFileName(assemblyFile));

                            var assembly = Assembly.LoadFrom(assemblyFile);
                            var matches = assembly.GetExportedTypes()
                                .Where(t => filter(t) && !t.IsAbstract &&
                                            interfaceType.IsAssignableFrom(t)).ToList();

                            if (matches.Any())
                                implementingTypes.AddRange(matches);

                            //implementingTypes.AddRange(from type in assembly.GetExportedTypes()
                            //                           where filter(type) &&
                            //                                 !type.IsAbstract &&
                            //                                 (Array.Find(interfaceType, interfaceToFind =>
                            //                                                             (type.GetInterface(
                            //                                                                 interfaceToFind, true) !=
                            //                                                              null)) != null)
                            //                           select type);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(Logger.Event.During("TypeDiscovery")
                                             .Description("Scanning {0}", assemblyFile)
                                             .Encountered(ex));
                        }
                    });

            matchingTypes = implementingTypes.ToArray();
            Logger.Info("Found {0} implementations", matchingTypes.Length);
            implementingTypes.ForEach(t => Logger.Debug("\t{0}.{1}", t.Namespace, t.Name));
            return (matchingTypes.Length > 0);
        }
    }
}