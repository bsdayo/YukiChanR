using System.Reflection;
using System.Resources;
using System.Text.Json;

namespace YukiChanR.Core.Utils;

public class YukiResourceManager
{
    private readonly Assembly _resAssembly;

    private readonly string _resAssemblyName;

    private readonly string? _resRelativePath;

    public YukiResourceManager(Assembly resAssembly, string? resRelativePath = null)
    {
        _resAssembly = resAssembly;
        _resAssemblyName = resAssembly.GetName().Name!;
        _resRelativePath = resRelativePath;
    }

    /// <summary>
    /// 获取资源流
    /// </summary>
    /// <param name="name">不带 Assembly 名称的资源名</param>
    /// <returns></returns>
    /// <exception cref="MissingManifestResourceException">如果未找到资源文件则触发</exception>
    public Stream GetStream(string name)
    {
        var fullname = _resRelativePath is null
            ? $"{_resAssemblyName}.{name}"
            : $"{_resAssemblyName}.{_resRelativePath}.{name}";
        var stream = _resAssembly.GetManifestResourceStream(fullname);

        if (stream is null)
            throw new MissingManifestResourceException($"MISSING RESOURCE: {fullname}");

        return stream;
    }

    public T GetJson<T>(string name)
    {
        var stream = GetStream(name);
        return JsonSerializer.Deserialize<T>(stream)!;
    }

    public byte[] GetData(string name)
    {
        var stream = GetStream(name);
        var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }
}