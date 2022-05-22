namespace AutoPay.API.Services.Configuration.Structure;

public class Comment : Node
{
    public char Prefix { get; set; }
    public string Value { get; set; }

    public override string Serialize()
    {
        return Prefix + Value;
    }
}