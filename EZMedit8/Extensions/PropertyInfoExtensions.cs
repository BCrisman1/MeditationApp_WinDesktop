using EZMedit8.Attributes;
using System.Linq;
using System.Reflection;


namespace EZMedit8.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static int PropertyOrder(this PropertyInfo propertyInfo)
        {
            int output;
            var orderAttr = (PropertyOrderAttribute)propertyInfo.GetCustomAttributes(typeof(PropertyOrderAttribute), true).SingleOrDefault();
            output = orderAttr != null ? orderAttr.Order : int.MaxValue;
            return output;
        }
    }
}