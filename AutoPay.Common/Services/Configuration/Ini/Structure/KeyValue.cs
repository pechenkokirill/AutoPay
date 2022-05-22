namespace AutoPay.API.Services.Configuration.Structure;

public class KeyValue : Node
{
    public string Key { get; set; }
    public string Value { get; set; }

    public override string Serialize()
    {
        return $"{Key} = {Value}";
    }
}