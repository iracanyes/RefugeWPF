using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RefugeWPF.ClassesMetiers.Model.Enums
{
    internal enum GenderType
    {
        [Description("M")]
        Male = 1,
        [Description("F")]
        Female = 2,
        [Description("Inconnu")]
        Unknown = 0,
    }
}
