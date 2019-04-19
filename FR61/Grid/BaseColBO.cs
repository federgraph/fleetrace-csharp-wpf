namespace RiggVar.FR
{

    public class TBaseColBO<G, B, N, C, I, PC, PI>
        where G : TColGrid<G, B, N, C, I, PC, PI>, new()
        where B : TBaseColBO<G, B, N, C, I, PC, PI>, new()
        where N : TBaseNode<G, B, N, C, I, PC, PI>, new()
        where C : TBaseRowCollection<G, B, N, C, I, PC, PI>, new()
        where I : TBaseRowCollectionItem<G, B, N, C, I, PC, PI>, new()
        where PC : TBaseColProps<G, B, N, C, I, PC, PI>, new()
        where PI : TBaseColProp<G, B, N, C, I, PC, PI>, new()
    {
        public TBaseColBO() : base()
        {
        }

        public N CurrentNode { get; set; }

        public I CurrentRow { get; set; }

        public virtual void InitColsActive(G StringGrid)
        {
            InitColsActiveLayout(StringGrid, 0);
        }

        public virtual void InitColsActiveLayout(G StringGrid, int aLayout)
        {
        }

    }

}
