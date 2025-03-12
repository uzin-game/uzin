using System.Collections;
using System.Collections.Generic;

namespace Tiles
{
    public class BasicGround : Case
    {
        public List<Case> AboveNeighbors;
        public List<Case> UnderNeighbors;
        public List<Case> RightNeighbors;
        public List<Case> LeftNeighbors;

        public BasicGround(List<Case> above, List<Case> under, List<Case> right, List<Case> left) : base (above)
        {
            AboveNeighbors = above;
            UnderNeighbors = under;
            RightNeighbors = right;
            LeftNeighbors = left;
        }
    }
}