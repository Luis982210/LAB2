using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab2.Arbol
{
    public class NodoAsteriscoo<TKey, T>
    {

        private int grado;

        public NodoAsteriscoo(int grado)
        {
            this.grado = grado;
            this.Hijos = new List<NodoAsteriscoo<TKey, T>>(grado + 1);
            this.Entradas = new List<BEntry<TKey, T>>(grado);
        }

        public List<NodoAsteriscoo<TKey, T>> Hijos { get; set; }

        public List<BEntry<TKey, T>> Entradas { get; set; }

        public bool EsHoja
        {
            get
            {
                return this.Hijos.Count == 0;
            }
        }

        public bool AlcanzaMaximaEntrada
        {
            get
            {
                return this.Entradas.Count == (this.grado);
            }
        }

        public bool AlcanzaMaximaEntradaRaiz
        {
            get
            {
                return this.Entradas.Count == ((4 / 3) * (this.grado - 1));
            }
        }

        public bool AlcanzaMinimaEntrada
        {
            get
            {
                return this.Entradas.Count == ((2 / 3) * (this.grado - 1));
            }
        }
    }
}
