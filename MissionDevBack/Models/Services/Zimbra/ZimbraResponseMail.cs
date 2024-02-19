namespace MissionDevBack.Models;

// MailPreview
public class ZimbraResponseMailPreview
{
    public object Header { get; set; }
    public ZimbraResponsePreviewBody Body { get; set; }
}

public class ZimbraResponsePreviewBody
{
    public ZimbraSearchResponse SearchResponse { get; set; }
}

public class ZimbraSearchResponse
{
    public string sortBy { get; set; }
    public int offset { get; set; }
    public List<ZimbraMailPreview> m { get; set; }
}

public class ZimbraMailPreview
{
    public int s { get; set; }
    public long d { get; set; }
    public string l { get; set; }
    public string cid { get; set; }
    public string f { get; set; }
    public int rev { get; set; }
    public string id { get; set; }
    public string su { get; set; }
    public string fr { get; set; }
    public bool cm { get; set; }
    public List<ZimbraMailExpeditor> e { get; set; }
    public string sf { get; set; }
}



// Mail
public class ZimbraResponseMail
{
    public object Header { get; set; }
    public ZimbraResponseBody Body { get; set; }
}

public class ZimbraResponseBody
{
    public ZimbraGetMsgResponse GetMsgResponse { get; set; }
}

public class ZimbraGetMsgResponse
{
    public List<ZimbraMail> m { get; set; }
}

public class ZimbraMail
{
    public int s { get; set; }
    public long d { get; set; }
    public string l { get; set; }
    public string cid { get; set; }
    public int rev { get; set; }
    public string id { get; set; }
    public string fr { get; set; }
    public List<ZimbraMailExpeditor> e { get; set; }
    public string su { get; set; }
    public string mid { get; set; }
    public long sd { get; set; }
    public List<ZimbraMailMp> mp { get; set; }
}

public class ZimbraMailMp
{
    public string part { get; set; }
    public string ct { get; set; }
    public int s { get; set; }
    public bool body { get; set; }
    public string content { get; set; }
    public string cd { get; set; }
    public string filename { get; set; }
    public string ci { get; set; }
    public List<ZimbraMailMp> mp { get; set; }
}



// Misc
public class ZimbraMailExpeditor
{
    public string a { get; set; }
    public string d { get; set; }
    public string p { get; set; }
    public string t { get; set; }
}
