using System.IO;
using System.Xml.Linq;
using TSL.HMI.Models;

namespace TSL.HMI.Services;

public sealed class MachineLoader
{
    public MachineDefinition Load(string root)
    {
        if (string.IsNullOrWhiteSpace(root))
            throw new ArgumentException("ITS root path cannot be empty.", nameof(root));

        if (!Directory.Exists(root))
            throw new DirectoryNotFoundException($"ITS directory not found: {root}");

        var machine = new MachineDefinition { RootPath = root };

        LoadIni(machine, FindFile(root, "its.ini"));
        LoadTags(machine, FindFile(root, "VariableTags.xml"));
        LoadIo(machine, FindFile(root, "diag.xml"));

        return machine;
    }

    private static string? FindFile(string root, string fileName)
    {
        return Directory
            .EnumerateFiles(root, fileName, SearchOption.AllDirectories)
            .FirstOrDefault();
    }

    private static void LoadIni(MachineDefinition machine, string? path)
    {
        if (path is null) return;

        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('['))
                continue;

            var equalsIndex = line.IndexOf('=');
            if (equalsIndex <= 0)
                continue;

            var key = line[..equalsIndex].Trim();
            var value = line[(equalsIndex + 1)..].Trim();
            machine.Settings[key] = value;
        }

        string[] systemKeys = { "SYSTEM_NUMBER", "SYSTEM", "SystemNumber", "System" };
        foreach (var key in systemKeys)
        {
            if (!machine.Settings.TryGetValue(key, out var value))
                continue;

            machine.SystemNumber = value;
            break;
        }
    }

    private static void LoadTags(MachineDefinition machine, string? path)
    {
        if (path is null) return;

        var document = XDocument.Load(path);
        foreach (var element in document.Descendants())
        {
            var variable = Value(element, "Variable", "OptoVariable", "Tag", "Name");
            var description = Value(element, "Description", "Desc");

            if (variable.Length == 0 || description.Length == 0)
                continue;

            machine.Tags.Add(new TagDefinition(
                Value(element, "Controller"),
                Value(element, "Section"),
                description,
                variable,
                Value(element, "DataType", "Type"),
                Value(element, "Min", "Minimum"),
                Value(element, "Max", "Maximum")));
        }
    }

    private static void LoadIo(MachineDefinition machine, string? path)
    {
        if (path is null) return;

        var document = XDocument.Load(path);
        foreach (var element in document.Descendants())
        {
            var name = Value(element, "Name", "PointName", "Variable");
            var description = Value(element, "Description", "Desc", "Label");

            if (name.Length == 0 && description.Length == 0)
                continue;

            var direction = Value(element, "Direction", "Type", "IOType");
            var looksLikeIo =
                direction.Contains("input", StringComparison.OrdinalIgnoreCase) ||
                direction.Contains("output", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("i", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("o", StringComparison.OrdinalIgnoreCase);

            if (!looksLikeIo)
                continue;

            machine.Io.Add(new IoPoint(
                Value(element, "Controller", "Device"),
                name,
                description,
                Value(element, "Module", "Slot"),
                Value(element, "Channel", "Point"),
                direction));
        }
    }

    private static string Value(XElement element, params string[] names)
    {
        foreach (var name in names)
        {
            var attributeValue = (string?)element.Attribute(name);
            if (!string.IsNullOrWhiteSpace(attributeValue))
                return attributeValue.Trim();

            var childValue = (string?)element.Element(name);
            if (!string.IsNullOrWhiteSpace(childValue))
                return childValue.Trim();
        }

        return string.Empty;
    }
}
