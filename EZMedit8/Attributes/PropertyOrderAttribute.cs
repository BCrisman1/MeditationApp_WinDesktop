using System;


namespace EZMedit8.Attributes
{
    public class PropertyOrderAttribute : Attribute
    {
        public int Order { get; set; }
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }
    }
}