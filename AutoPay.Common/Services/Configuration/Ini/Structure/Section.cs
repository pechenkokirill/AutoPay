namespace AutoPay.API.Services.Configuration.Structure;

public class Section : Root
{
    public string Value { get; set; }
    public override string Serialize()
    {
        return '[' + Value + ']' + '\n' + base.Serialize();
    }
}