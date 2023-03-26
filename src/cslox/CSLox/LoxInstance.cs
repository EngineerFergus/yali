using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox
{
    internal class LoxInstance
    {
        private LoxClass _Klass;

        public LoxInstance(LoxClass klass)
        {
            _Klass = klass;
        }

        public override string ToString()
        {
            return $"{_Klass.Name} instance";
        }
    }
}
