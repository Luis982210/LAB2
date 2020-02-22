using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab2.Arbol
{
    public class BEntry<TKey, T> : IEquatable<BEntry<TKey, T>>, IEntry<TKey, T>
    {
        public TKey LLave { get; set; }

        public T Apuntador { get; set; }

        public bool Equals(BEntry<TKey, T> other)
        {
            return this.LLave.Equals(other.LLave) && this.Apuntador.Equals(other.Apuntador);
        }

    }
    public interface IEntry<TKey, T>
    {
        bool Equals(BEntry<TKey, T> other);
    }
}
