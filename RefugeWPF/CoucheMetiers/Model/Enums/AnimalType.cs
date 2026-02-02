using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RefugeWPF.CoucheMetiers.Model.Enums
{
    internal enum AnimalType
    {
        [Description("chat")]
        Cat = 1,
        [Description("chien")]
        Dog = 2,
        [Description("inconnu")]
        Unknown = 0,
    }
}
