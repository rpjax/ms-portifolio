namespace Aidan.Web.AccessManagement;

public class IdentityActionDescriptionAttribute : Attribute
{
    public string Text { get; set; }

    public IdentityActionDescriptionAttribute(string text)
    {
        Text = text;
    }
}
