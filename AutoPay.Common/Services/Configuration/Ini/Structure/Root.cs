namespace AutoPay.API.Services.Configuration.Structure;

public class Root : Node
{
    public IList<Node> Nodes { get; } = new List<Node>();

    public Section FindSection(string name)
    {
        foreach (var node in Nodes)
        {
            if (node is Section section && section.Value == name)
            {
                return section;
            }
        }

        return null;
    }

    public KeyValue FindPair(string key)
    {
        foreach (var node in Nodes)
        {
            if (node is KeyValue keyValue && keyValue.Key == key)
            {
                return keyValue;
            }
        }

        return null;
    }

    public override string Serialize()
    {
        IEnumerable<string> serializedNodes = Nodes.Select(x => x.Serialize());
        return string.Join('\n', serializedNodes);
    }
}