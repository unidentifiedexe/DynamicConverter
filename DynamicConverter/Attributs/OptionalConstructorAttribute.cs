using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverter.Attributs
{

    /// <summary> 引数ありのコンストラクタを用いてインスタンスを生成できるようにします。 </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class OptionalConstructorAttribute : Attribute
    {
        public int Priority { get; set; }

        private readonly string[] _paramNames;

        public ICollection<string> ParamNames => _paramNames;
        public OptionalConstructorAttribute(params string[] paramNames)
        {
            _paramNames = (paramNames?.Clone() as string[]) ?? Array.Empty<string>();

        }
    }
}
