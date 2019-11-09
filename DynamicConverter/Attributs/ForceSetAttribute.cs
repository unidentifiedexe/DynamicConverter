using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverter.Attributs
{
    /// <summary>
    /// <see cref="BindingFlags.NonPublic"/> な setter を持つプロパティにも代入するようにします。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForceSetAttribute : Attribute
    {
        public ForceSetAttribute()
        {

        }
    }

}
