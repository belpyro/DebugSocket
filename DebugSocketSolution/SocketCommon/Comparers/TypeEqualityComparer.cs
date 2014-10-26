using System;
using System.Collections.Generic;

namespace SocketCommon.Comparers
{
    public class TypeEqualityComparer: IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
            
            return x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Type obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
