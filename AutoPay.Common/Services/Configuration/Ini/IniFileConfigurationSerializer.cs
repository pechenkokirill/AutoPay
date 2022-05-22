using AutoPay.API.Services.Configuration.Structure;
using AutoPay.Common.Models;

namespace AutoPay.API.Services.Configuration;

public class IniFileConfigurationSerializer : IAppConfigurationSerializer
{
    private const string KeyDelimiter = ":";
    private readonly string _filePath;
    private Root _configStructure;

    public IniFileConfigurationSerializer(string filePath)
    {
        _filePath = filePath;
    }
    public IDictionary<string, string> Deserialize()
    {
        var config = new Dictionary<string, string>();
        _configStructure = new Root();
        string[] lines;
        try
        {
            lines = File.ReadAllLines(_filePath);
        }
        catch
        {
            return config;
        }

        var root = _configStructure;
        var currentSection = string.Empty;
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }
            
            if (trimmedLine.StartsWith('#') ||
                trimmedLine.StartsWith('/') ||
                trimmedLine.StartsWith(';'))
            {
                _configStructure.Nodes.Add(new Comment
                {
                    Prefix = trimmedLine[0],
                    Value = trimmedLine.Substring(1)
                });
                continue;
            }

            if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']'))
            {
                var currentSectionValue = trimmedLine.Substring(1, trimmedLine.Length - 2);
                currentSection = currentSectionValue + KeyDelimiter;
                root = new Section
                {
                    Value = currentSectionValue
                };
                _configStructure.Nodes.Add(root);
                continue;
            }

            var separator = trimmedLine.IndexOf('=');
            if (separator < 0)
            {
                continue;
            }

            var key = trimmedLine.Substring(0, separator).Trim();
            var value = trimmedLine.Substring(separator + 1).Trim();

            if (!config.ContainsKey(key))
            {
                config.Add(currentSection + key, value);
                root.Nodes.Add(new KeyValue
                {
                    Key = key,
                    Value = value
                });
            }
        }

        return config;
    }

    public void Serialize(IDictionary<string, string> data)
    {
        if (_configStructure is null)
        {
            _configStructure = new Root();
        }
        
        foreach (var configPair in data)
        {
            var fullKey = configPair.Key;
            var value = configPair.Value;
            var indexOfSection = fullKey.IndexOf(':');
            var section = fullKey.Substring(0, indexOfSection);
            var key = fullKey.Substring(indexOfSection + 1);

            var root = _configStructure;
            if (!string.IsNullOrWhiteSpace(section))
            {
                var configSection = _configStructure.FindSection(section);

                if (configSection is null)
                {
                    var sectionNode = new Section
                    {
                        Value = section
                    };
                    _configStructure.Nodes.Add(sectionNode);
                    root = sectionNode;
                }
                else
                {
                    root = configSection;
                }
            }

            var configKeyValue = root.FindPair(key);
            if (configKeyValue is not null)
            {
                configKeyValue.Value = value;
            }
            else
            {
                root.Nodes.Add(new KeyValue()
                {
                    Key = key,
                    Value = value
                });
            }
        }

        var file = _configStructure.Serialize();

        try
        {
            File.WriteAllText(_filePath, file);
        }
        catch
        {
            // ignored
        }
    }
}