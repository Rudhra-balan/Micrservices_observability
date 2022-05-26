
using System.Reflection;

namespace Webhook.Infrastructure.ShellExecutor.Bridge
{
    public static class ResourceReader
    {
        // to read the file as a Stream
        public static string ReadManifestData<TSource>(string embeddedFileName) where TSource : class
        {
            var assembly = typeof(TSource).GetTypeInfo().Assembly;
            var resourceName = assembly.GetManifestResourceNames().First(s =>
                s.EndsWith(embeddedFileName, StringComparison.CurrentCultureIgnoreCase));

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException("Could not load manifest resource stream.");
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static Stream ReadManifestStream<TSource>(string embeddedFileName) where TSource : class
        {
            var assembly = typeof(TSource).GetTypeInfo().Assembly;
            var resourceName = assembly.GetManifestResourceNames().First(s =>
                s.EndsWith(embeddedFileName, StringComparison.CurrentCultureIgnoreCase));

            using var stream = assembly.GetManifestResourceStream(resourceName);

            var fileStream = stream ?? throw new InvalidOperationException("Could not load manifest resource stream.");
            return fileStream;
        }
    }
}
