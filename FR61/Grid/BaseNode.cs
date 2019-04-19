using System.Diagnostics;

namespace RiggVar.FR
{

    /// <summary>
    /// TBaseNode is a base class for building a tree or list of nodes,
    /// where each Node has a TBaseRowCollection of items.
    /// The referenced TBaseColBO defines editable columns of the collection 
    /// and provides validation for input into these columns. 
    /// </summary>
    public class TBaseNode<G, B, N, C, I, PC, PI>
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        public string NameID;
        public int Layout;

        public TBaseNode()
        {
            Collection = new C
            {
                BaseNode = (N)this
            };
        }

        public B ColBO
        {
            [DebuggerStepThrough]
            get; set;
        }

        public C Collection
        {
            [DebuggerStepThrough]
            get;
        }

        public bool Modified { get; set; }

        public virtual void Calc()
        {
        }

    }
}
