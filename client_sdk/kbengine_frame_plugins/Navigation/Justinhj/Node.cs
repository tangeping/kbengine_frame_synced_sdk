namespace KBEngine
{
    // A node represents a possible state in the search
    // The user provided state type is included inside this type
    public class Node
    {
        public Node parent; // used during the search to record the parent of successor nodes
        public Node child; // used after the search for the application to view the search in reverse

        public FP g; // cost of this node + it's predecessors
        public FP h; // heuristic estimate of distance to goal
        public FP f; // sum of cumulative cost of predecessors and self and heuristic

        public Node()
        {
            Reinitialize();
        }

        public void Reinitialize()
        {
            parent = null;
            child = null;
            g = 0;
            h = 0;
            f = 0;
        }

        public MapSearchNode m_UserState;
    };

}
