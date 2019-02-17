using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Helpers
{
    public class PriceEqualityComparer<TPrice> : IEqualityComparer<TPrice> where TPrice : Price
    {
        public bool Equals(TPrice x, TPrice y) => x.Time == y.Time;

        public int GetHashCode(TPrice obj) => obj.Time.GetHashCode();
    }
}