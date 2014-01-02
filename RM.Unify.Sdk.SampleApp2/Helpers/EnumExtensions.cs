using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace RM.Unify.Sdk.SampleApp2.Helpers
{
    public static class EnumExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue)
        {
            var res = from Enum e in Enum.GetValues(enumValue.GetType())
                      select new SelectListItem
                      {
                          Selected = e.Equals(enumValue),
                          Text = e.ToDescription(),
                          Value = e.ToString()
                      };
            res = res.ToArray();
            return res;
        }

        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}