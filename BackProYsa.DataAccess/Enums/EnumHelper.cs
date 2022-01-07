using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.ComponentModel;

namespace BackProYsa.DataAccess.Enums
{
    public static class EnumHelper
    {
        public static string GetDescription(Enum value)
        {

            var ItemText = value
                   .GetType()
                   .GetMember(value.ToString())
                   .FirstOrDefault()
                   ?.GetCustomAttribute<DescriptionAttribute>()
                   ?.Description;

            return ItemText == null ? value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()?.Name : ItemText;
        }

        public static List<int> GetEnumValue(Type EnumType)
        {
            List<int> InstellingCode = new List<int>();

            Array values = Enum.GetValues(EnumType);
            foreach (int value in values)
            {
                InstellingCode.Add(Convert.ToInt32(value.ToString()));
            }
            return InstellingCode;
        }

        public static string GetDescription(Type EnumType, int enumvalue)
        {
            string description = "";

            Array values = Enum.GetValues(EnumType);
            foreach (int value in values)
            {
                if (value == enumvalue)
                {
                    description = GetDescription((Enum)Enum.Parse(EnumType, value.ToString()));
                    break;
                }
            }
            return description;
        }
        private static int GetOrderAtt(Enum value)
        {
            var ItemText = value
                   .GetType()
                   .GetMember(value.ToString())
                   .FirstOrDefault()
                   ?.GetCustomAttribute<OrderAtt>()
                   ?.Order;

            return ItemText.HasValue ? ItemText.Value : 0;
        }


        //Tüm Nesneleri Çekmek için kullanılır, özelleştirme yoksa tek tek çağırılmaz.
        public static SelectList GetSelectList(Type EnumType)
        {
            var result = new List<SelectListItem>();
            Array values = Enum.GetValues(EnumType);

            var _dic = new Dictionary<string, int>(); // Order Attribute yer alıyorsa sıralamak için.
            foreach (int value in values)
            {
                _dic.Add(value.ToString(), GetOrderAtt((Enum)Enum.Parse(EnumType, value.ToString())));
            }

            foreach (var item in _dic.OrderBy(key => key.Value))
            {
                var listitem = new SelectListItem
                {
                    Text = GetDescription((Enum)Enum.Parse(EnumType, item.Key)),
                    Value = item.Key
                };
                result.Add(listitem);
            }


            return new SelectList(result, "Value", "Text");
        }
    }
}
