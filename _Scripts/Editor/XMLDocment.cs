using System;
using System.Text;

// !!!!!!!!!!!!!!!!!注意不同于C# 的自身的xml 类 XmlDocument
public class XMLDocment  {
    private StringBuilder builder;
    public XMLDocment()
    {
        builder = new StringBuilder();
    }

    public void StartObject(string name)
    {
        builder.AppendFormat("<{0}>", name);
    }

    public void EndObject(string name)
    {
        builder.AppendFormat("</{0}>", name);
    }

    public void CreateElement(string name,string value)
    {
        builder.AppendFormat("<{0}>{1}</{0}>",name,value);
    }

    public override string ToString()
    {
        return builder.ToString();
    }

}
