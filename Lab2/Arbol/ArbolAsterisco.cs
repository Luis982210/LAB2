using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Arbol
{
    public class ArbolAsterisco<TKey, T> where TKey : IComparable<TKey>
    {
        public NodoAsteriscoo<TKey, T> Raiz { get; set; }

        public int Grado { get; private set; }

        public int Altura { get; private set; }

        public ArbolAsterisco(int grado)
        {
            if (grado < 2)
            {
                throw new ArgumentException("Arbol B debe ser de por lo menos grado 2", "grado");
            }
            this.Raiz = new NodoAsteriscoo<TKey, T>(grado);
            this.Grado = grado;
            this.Altura = 1;
        }

        public BEntry<TKey, T> Search(TKey Llave)
        {
            return this.BusquedaInterna(this.Raiz, Llave);
        }

        public void Insertar(TKey nuevaLlave, T nuevoApuntador)
        {
            InsertarRecursivo(this.Raiz, nuevaLlave, nuevoApuntador, null);            
        }        
        private void InsertarRecursivo(NodoAsteriscoo<TKey, T> nodo, TKey nuevaLlave, T nuevoApuntador, NodoAsteriscoo<TKey, T> nodoPadre)
        {
            int posicionInsertar = nodo.Entradas.TakeWhile(entry => nuevaLlave.CompareTo(entry.LLave) >= 0).Count();
            //Es hoja
            if (nodo.EsHoja)
            {
                if (this.Raiz == nodo)
                {
                    this.Raiz.Entradas.Insert(posicionInsertar, new BEntry<TKey, T>() { LLave = nuevaLlave, Apuntador = nuevoApuntador });
                    if (this.Raiz.AlcanzaMaximaEntrada)
                    {
                        // nuevo nodo y se necesita dividir
                        NodoAsteriscoo<TKey, T> viejaRaiz = this.Raiz;
                        this.Raiz = new NodoAsteriscoo<TKey, T>(this.Grado);
                        this.Raiz.Hijos.Add(viejaRaiz);
                        this.DividirHijo(this.Raiz, 0, viejaRaiz);
                        this.Altura++;
                    }
                    return;
                }
                else
                {
                    nodo.Entradas.Insert(posicionInsertar, new BEntry<TKey, T>() { LLave = nuevaLlave, Apuntador = nuevoApuntador });
                    if (nodo.AlcanzaMaximaEntrada)
                    {
                        posicionInsertar = nodoPadre.Entradas.TakeWhile(entry => nuevaLlave.CompareTo(entry.LLave) >= 0).Count();
                        DividirHijo(nodoPadre, posicionInsertar, nodo);
                    }
                    return;
                }
            }
            //No es Hoja
            else
            {
                this.InsertarRecursivo(nodo.Hijos[posicionInsertar], nuevaLlave, nuevoApuntador, nodo);
                if (nodoPadre == null)
                {
                    if (nodo.AlcanzaMaximaEntrada)
                    {
                        NodoAsteriscoo<TKey, T> viejaRaiz = this.Raiz;
                        this.Raiz = new NodoAsteriscoo<TKey, T>(this.Grado);
                        this.Raiz.Hijos.Add(viejaRaiz);
                        this.DividirHijo(this.Raiz, 0, viejaRaiz);
                        this.Altura++;
                    }
                    return;
                }
                else
                {
                    if (nodo.AlcanzaMaximaEntrada)
                    {
                        DividirHijo(nodoPadre, posicionInsertar, nodo);
                    }
                    return;
                }
            }

        }

        private BEntry<TKey, T> BusquedaInterna(NodoAsteriscoo<TKey, T> node, TKey key)
        {
            int i = node.Entradas.TakeWhile(entry => key.CompareTo(entry.LLave) > 0).Count();
            if (i < node.Entradas.Count && node.Entradas[i].LLave.CompareTo(key) == 0)
            {
                return node.Entradas[i];
            }
            return node.EsHoja ? null : this.BusquedaInterna(node.Hijos[i], key);
        }

        public void Eliminar(TKey LlaveEliminar)
        {
            this.EliminarInterno(this.Raiz, LlaveEliminar);
            // Si la ultima raiz de la entrada fue movida a un nodo hijo la remueve
            if (this.Raiz.Entradas.Count == 0 && !this.Raiz.EsHoja)
            {
                this.Raiz = this.Raiz.Hijos.Single();
                this.Altura--;
            }

        }

        private void EliminarInterno(NodoAsteriscoo<TKey, T> nodo, TKey LlaveEliminar)
        {
            int i = nodo.Entradas.TakeWhile(entrada => LlaveEliminar.CompareTo(entrada.LLave) > 0).Count();
            // Encontro la llave en un nodo, la elimina
            if (i < nodo.Entradas.Count && nodo.Entradas[i].LLave.CompareTo(LlaveEliminar) == 0)

            {

                this.EliminarLlaveNodo(nodo, LlaveEliminar, i);

                return;

            }
            // Eliminar la lleva de un sub arbol
            if (!nodo.EsHoja)
            {
                this.EliminarLlaveSuArbolAsterisco(nodo, LlaveEliminar, i);
            }
        }


        private void EliminarLlaveSuArbolAsterisco(NodoAsteriscoo<TKey, T> NodoPadre, TKey LlaveEliminar, int IndiceSuArbolAsterisco)
        {
            NodoAsteriscoo<TKey, T> NodoHijo = NodoPadre.Hijos[IndiceSuArbolAsterisco];

            //para que no se creen mas hojas sin que sirvan
            if (NodoHijo.AlcanzaMinimaEntrada)
            {
                int indiceIzquierdo = IndiceSuArbolAsterisco - 1;
                NodoAsteriscoo<TKey, T> HijoIzquierdo = IndiceSuArbolAsterisco > 0 ? NodoPadre.Hijos[indiceIzquierdo] : null;
                int indiceDerecho = IndiceSuArbolAsterisco + 1;
                NodoAsteriscoo<TKey, T> HijoDerecho = IndiceSuArbolAsterisco < NodoPadre.Hijos.Count - 1 ? NodoPadre.Hijos[indiceDerecho] : null;
                if (HijoIzquierdo != null && HijoIzquierdo.Entradas.Count > this.Grado - 1)
                {
                    NodoHijo.Entradas.Insert(0, NodoPadre.Entradas[IndiceSuArbolAsterisco]);
                    NodoPadre.Entradas[IndiceSuArbolAsterisco] = HijoIzquierdo.Entradas.Last();
                    HijoIzquierdo.Entradas.RemoveAt(HijoIzquierdo.Entradas.Count - 1);

                    if (!HijoIzquierdo.EsHoja)
                    {
                        NodoHijo.Hijos.Insert(0, HijoIzquierdo.Hijos.Last());
                        HijoIzquierdo.Hijos.RemoveAt(HijoIzquierdo.Hijos.Count - 1);
                    }
                }
                else if (HijoDerecho != null && HijoDerecho.Entradas.Count > this.Grado - 1)
                {

                    NodoHijo.Entradas.Add(NodoPadre.Entradas[IndiceSuArbolAsterisco]);
                    NodoPadre.Entradas[IndiceSuArbolAsterisco] = HijoDerecho.Entradas.First();
                    HijoDerecho.Entradas.RemoveAt(0);

                    if (!HijoDerecho.EsHoja)
                    {
                        NodoHijo.Hijos.Add(HijoDerecho.Hijos.First());
                        HijoDerecho.Hijos.RemoveAt(0);
                    }
                }
                else
                {
                    if (HijoIzquierdo != null)
                    {
                        NodoHijo.Entradas.Insert(0, NodoPadre.Entradas[IndiceSuArbolAsterisco - 1]);
                        var oldEntries = NodoHijo.Entradas;
                        NodoHijo.Entradas = HijoIzquierdo.Entradas;
                        NodoHijo.Entradas.AddRange(oldEntries);
                        if (!HijoIzquierdo.EsHoja)
                        {
                            var oldChildren = NodoHijo.Hijos;
                            NodoHijo.Hijos = HijoIzquierdo.Hijos;
                            NodoHijo.Hijos.AddRange(oldChildren);
                        }
                        NodoPadre.Hijos.RemoveAt(indiceIzquierdo);
                        NodoPadre.Entradas.RemoveAt(IndiceSuArbolAsterisco - 1);
                    }
                    else
                    {
                        Debug.Assert(HijoDerecho != null, "Nodo debe tener por lo menos un hermano");

                        NodoHijo.Entradas.Add(NodoPadre.Entradas[IndiceSuArbolAsterisco]);
                        NodoHijo.Entradas.AddRange(HijoDerecho.Entradas);
                        if (!HijoDerecho.EsHoja)

                        {

                            NodoHijo.Hijos.AddRange(HijoDerecho.Hijos);
                        }
                        NodoPadre.Hijos.RemoveAt(indiceDerecho);
                        NodoPadre.Entradas.RemoveAt(IndiceSuArbolAsterisco);
                    }
                }
            }
            this.EliminarInterno(NodoHijo, LlaveEliminar);
        }


        private void EliminarLlaveNodo(NodoAsteriscoo<TKey, T> nodo, TKey LlaveEliminar, int indiceLlaveNodo)
        {
            if (nodo.EsHoja)
            {
                nodo.Entradas.RemoveAt(indiceLlaveNodo);
                return;
            }
            NodoAsteriscoo<TKey, T> predecesorHijo = nodo.Hijos[indiceLlaveNodo];
            if (predecesorHijo.Entradas.Count >= this.Grado)
            {
                BEntry<TKey, T> predecessor = this.EliminarPredecesor(predecesorHijo);
                nodo.Entradas[indiceLlaveNodo] = predecessor;
            }
            else
            {
                NodoAsteriscoo<TKey, T> succesorHijo = nodo.Hijos[indiceLlaveNodo + 1];
                if (succesorHijo.Entradas.Count >= this.Grado)
                {
                    BEntry<TKey, T> successor = this.EliminarSucesor(predecesorHijo);
                    nodo.Entradas[indiceLlaveNodo] = successor;

                }
                else
                {
                    predecesorHijo.Entradas.Add(nodo.Entradas[indiceLlaveNodo]);
                    predecesorHijo.Entradas.AddRange(succesorHijo.Entradas);
                    predecesorHijo.Hijos.AddRange(succesorHijo.Hijos);



                    nodo.Entradas.RemoveAt(indiceLlaveNodo);
                    nodo.Hijos.RemoveAt(indiceLlaveNodo + 1);
                    this.EliminarInterno(predecesorHijo, LlaveEliminar);

                }

            }

        }

        private BEntry<TKey, T> EliminarPredecesor(NodoAsteriscoo<TKey, T> nodo)
        {
            if (nodo.EsHoja)
            {
                var result = nodo.Entradas[nodo.Entradas.Count - 1];
                nodo.Entradas.RemoveAt(nodo.Entradas.Count - 1);
                return result;
            }
            return this.EliminarPredecesor(nodo.Hijos.Last());
        }

        private BEntry<TKey, T> EliminarSucesor(NodoAsteriscoo<TKey, T> nodo)
        {
            if (nodo.EsHoja)
            {
                var result = nodo.Entradas[0];
                nodo.Entradas.RemoveAt(0);
                return result;
            }
            return this.EliminarPredecesor(nodo.Hijos.First());
        }


        // cuando el nodo se llena y se crean dos hijos
        private void DividirHijo(NodoAsteriscoo<TKey, T> padreNodo, int nodoCorrer, NodoAsteriscoo<TKey, T> nodoMover)
        {

            var nuevoNodo = new NodoAsteriscoo<TKey, T>(this.Grado);
            if (Grado % 2 == 0)
            {
                padreNodo.Entradas.Insert(nodoCorrer, nodoMover.Entradas[(this.Grado / 2) - 1]);
            }
            else
            {
                padreNodo.Entradas.Insert(nodoCorrer, nodoMover.Entradas[(this.Grado / 2)]);
            }

            if (Grado % 2 == 0)
            {
                nuevoNodo.Entradas.AddRange(nodoMover.Entradas.GetRange((this.Grado / 2), (this.Grado / 2)));
                nodoMover.Entradas.RemoveRange((this.Grado / 2) - 1, (this.Grado / 2) + 1);
            }
            else
            {
                nuevoNodo.Entradas.AddRange(nodoMover.Entradas.GetRange((this.Grado / 2) + 1, this.Grado / 2));
                nodoMover.Entradas.RemoveRange((this.Grado / 2), (this.Grado / 2) + 1);
            }



            if (!nodoMover.EsHoja)
            {
                if (Grado % 2 == 0)
                {
                    nuevoNodo.Hijos.AddRange(nodoMover.Hijos.GetRange((this.Grado / 2), (this.Grado / 2) + 1));
                    nodoMover.Hijos.RemoveRange((this.Grado / 2), (this.Grado / 2) + 1);
                }
                else
                {
                    nuevoNodo.Hijos.AddRange(nodoMover.Hijos.GetRange((this.Grado / 2) + 1, (this.Grado / 2) + 1));
                    nodoMover.Hijos.RemoveRange((this.Grado / 2) + 1, (this.Grado / 2) + 1);
                }

            }
            padreNodo.Hijos.Insert(nodoCorrer + 1, nuevoNodo);
        }

    }
}
