namespace MissionDevBack.Models;

public class ZimbraMail
{
    public int s { get; }
    public int d { get; }
    public string l { get; }
    public string cid { get; }
    public string f { get; }
    public int rev { get; }
    public int id { get; }
    public string su { get; }
    public string fr { get; }
    public bool cm { get; }
    public ZimbraMailExpeditor[] e { get; }
    public string sf { get; }

}

public class ZimbraMailExpeditor
{
    public string a { get; }
    public string d { get; }
    public string p { get; }
    public string t { get; }
}