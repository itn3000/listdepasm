using ConsoleAppFramework;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

ConsoleApp.Run(args, (string asm, string[]? searchPath = null, int maxDepth = 1) =>
{
    var asmpath = new FileInfo(asm);
    if (!asmpath.Exists)
    {
        throw new System.IO.FileNotFoundException($"assembly path({asm}) does not exist");
    }
    searchPath = searchPath ?? [];
    var record = GetRecord(asmpath, searchPath, maxDepth, 0);
    if (record != null)
    {
        using var stdout = Console.OpenStandardOutput();
        JsonSerializer.Serialize(stdout, record, AssemblyRecordContext.Default.AssemblyRecord);
    }
});

AssemblyRecord? GetRecord(FileInfo fi, string[] searchPath, int maxDepth, int depth)
{
    if (!fi.Exists)
    {
        return null;
    }
    var asm = Assembly.LoadFile(fi.FullName);
    var asmname = asm.GetName();
    var referencedAssemblies = new List<AssemblyRecord>();
    var location = fi.Directory != null ? fi.Directory.FullName : "";
    if (maxDepth > depth)
    {
        foreach (var childasmname in asm.GetReferencedAssemblies())
        {
            if (fi.Directory == null)
            {
                continue;
            }
            // var childfi = fi.Directory.EnumerateFiles($"{childasmname.Name}.dll").FirstOrDefault();
            var childfi = FindAssemblyLocation(childasmname, [.. searchPath, fi.Directory.FullName]);
            if (childfi != null)
            {
                var childrecord = GetRecord(childfi, searchPath, maxDepth, depth + 1);
                if (childrecord != null)
                {
                    referencedAssemblies.Add(childrecord);
                }
            }
            else
            {
                referencedAssemblies.Add(new AssemblyRecord(childasmname.Name ?? "", childasmname.Version != null ? childasmname.Version.ToString() : "", "", []));
            }
        }
    }
    return new AssemblyRecord(asmname.Name ?? "", asmname.Version != null ? asmname.Version.ToString() : "", location, [.. referencedAssemblies]);
}

FileInfo? FindAssemblyLocation(AssemblyName asmname, string[] searchPath)
{
    foreach(var dir in searchPath)
    {
        var di = new DirectoryInfo(dir);
        if(di.Exists)
        {
            var fi = di.EnumerateFiles($"{asmname.Name}.dll").FirstOrDefault();
            if(fi != null)
            {
                return fi;
            }
            fi = di.EnumerateFiles($"{asmname.Name}.exe").FirstOrDefault();
            if(fi != null)
            {
                return fi;
            }
        }
    }
    return null;
}

record class AssemblyRecord(string Name, string Version, string Location, AssemblyRecord[] Dependencies);
[JsonSerializable(typeof(AssemblyRecord))]
[JsonSourceGenerationOptions(WriteIndented = true)]
partial class AssemblyRecordContext : JsonSerializerContext
{

}