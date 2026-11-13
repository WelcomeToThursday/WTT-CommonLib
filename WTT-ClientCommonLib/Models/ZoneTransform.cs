namespace WTTClientCommonLib.Models;

public class ZoneTransform
{
    public ZoneTransform(string x, string y, string z, string w = "0")
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public string X { get; set; }
    public string Y { get; set; }
    public string Z { get; set; }
    public string W { get; set; }
}

public class ZoneTransforms
{
    public ZoneTransform Position { get; set; }
    public ZoneTransform Rotation { get; set; }
    public ZoneTransform Scale { get; set; }
}
