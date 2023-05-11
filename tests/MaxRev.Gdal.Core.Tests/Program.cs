using System.Linq;
using MaxRev.Gdal.Core;
using OSGeo.GDAL;
using OSGeo.OSR;
using System;
using System.IO;
using System.Reflection;
using OSGeo.OGR;
using System.Runtime.InteropServices;

namespace GdalCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"About OS: {RuntimeInformation.OSDescription}");
                Console.WriteLine($"OS Architecture: {RuntimeInformation.OSArchitecture}");
                Console.WriteLine($"Runtime Identifier: {RuntimeInformation.RuntimeIdentifier}");

                Console.WriteLine($"Working directory: {Directory.GetCurrentDirectory()}");
                Console.WriteLine("Trying to configure all twice");
                GdalBase.ConfigureAll();
                GdalBase.ConfigureAll();
                Console.WriteLine("GDAL configured");
                Console.WriteLine("OGR Vector Drivers: {0}", Ogr.GetDriverCount());
                var rasterCount = Enumerable
                    .Range(0, Gdal.GetDriverCount()).Select(Gdal.GetDriver)
                    .Count(x => x.GetMetadataItem("DCAP_RASTER", null) == "YES");
                Console.WriteLine("GDAL Raster Drivers: {0}", rasterCount);

                var version = Assembly.GetAssembly(typeof(MaxRev.Gdal.Core.GdalBase))
                         .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                     .InformationalVersion;

                Console.WriteLine($"Package version: {version}");

                Console.WriteLine(string.Join('\n',
                    $"GDAL Version: {Gdal.VersionInfo("RELEASE_NAME")}",
                    $"GDAL INFO: {Gdal.VersionInfo("")}"));
                var spatialReference = new SpatialReference(null);
                spatialReference.SetWellKnownGeogCS("wgs84");

                for (int driverIndex = 0; driverIndex < Gdal.GetDriverCount(); driverIndex++)
                {
                    var driverByName = Gdal.GetDriver(driverIndex);
                    Console.WriteLine(
                        driverByName != default ? $"{driverByName.ShortName} loaded successfully" : $"Failed to load {driverByName.ShortName}");
                }

                // retrieve list of drivers for tests
                Console.WriteLine("Test string:");
                var driverList = string.Join(',', Enumerable.Range(0, Gdal.GetDriverCount())
                    .Select(i => Gdal.GetDriver(i).ShortName)
                    .OrderBy(x => x)
                    .Select(x => $"\"{x}\""));
                Console.WriteLine(driverList);
                Console.WriteLine("Drivers:" + Gdal.GetDriverCount());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
