//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;

//namespace GameLib.Engine
//{
//    /*
//     * Contains ChildCount RModel's that all fit into boundingBox
//     * 
//     */
//    class SceneNode
//    {
//        static const int MaxMembers = 8; // members per node, tweak for performance
//        static const int ChildCount = 4; // children per node. 4 because we are a quadtree
//        static const float rootWidth = 2000.0f;
//        static const BoundingBox RootBoundingBox = new BoundingBox(new Vector3(-rootWidth, float.MinValue, -rootWidth), new Vector3(rootWidth, float.MaxValue, rootWidth));

//        static BoundingBox GetChildBoundingBox(BoundingBox boundingBox, int index)
//        {
//            BoundingBox bb = new BoundingBox();
//            float widthOver2 = (boundingBox.Max.X - boundingBox.Min.X) / 2.0f;
//            float depthOver2 = (boundingBox.Max.Z - boundingBox.Min.Z) / 2.0f;
//            Vector3 min = boundingBox.Min;
//            Vector3 max = boundingBox.Max;
//            float xLeft = min.X;
//            float xRight = max.X;
//            float xCenter = xRight - xRight + widthOver2;
//            float zBottom = min.Z;
//            float zTop = max.Z;
//            float zCenter = zTop - zBottom + depthOver2;

//            /*
//             *  Origin in bottom left corner. TOp down view.
//             *  
//             *      ---------  Max
//             *      | 2 | 3 |
//             *      ---------
//             *      | 0 | 1 |
//             * Min  ---------  
//             */


//            /*
//             *          X min    Z min    X max    Z max
//             * 0        xLeft    zBottom  xCenter  zCenter    
//             * 1        xCenter  zBottom  xRight   zCenter
//             * 2        xLeft    zCenter  xCenter  zTop
//             * 3        xCenter  zCenter  xRight   zTop
//             * 
//             */

//            switch (index)
//            {
//                case 0:
//                    min.X = xLeft;
//                    min.Z = zBottom;
//                    max.X = xCenter;
//                    max.Z = zCenter;
//                    break;
//                case 1:
//                    min.X = xCenter;
//                    min.Z = zBottom;
//                    max.X = xRight;
//                    max.Z = zCenter;
//                    break;
//                case 2:
//                    min.X = xLeft;
//                    min.Z = zCenter;
//                    max.X = xCenter;
//                    max.Z = zTop;
//                    break;
//                case 3:
//                    min.X = xCenter;
//                    min.Z = zCenter;
//                    max.X = xRight;
//                    max.Z = zTop;
//                    break;
//            }
//            return bb;
//        }

//        public SceneNode()
//            : this(null, RootBoundingBox) // defer to the other constructor
//        {            
//        }

//        public SceneNode(SceneNode parent, BoundingBox bb)
//        {
//            boundingBox = bb;
//            this.parent = parent;
//            children = new List<SceneNode>(ChildCount);
//            members = new List<RModel>(MaxMembers);
//        }

//        public bool IsFull { get { return members.Count == MaxMembers; } }
        

//        public bool AddMember(RModel member)
//        {
//            if (boundingBox.Contains(member.physicsObject.Position) == ContainmentType.Contains)
//            {
//                if (IsFull)
//                {
//                    // make sure we created our children
//                    if (!createdChildren)
//                    {
//                        CreateChildren();
//                    }

//                    // we are full, pass it to the children
//                    for (int i = 0; i < children.Count; i++)
//                    {
//                        if (children[i].AddMember(member))
//                            return true;
//                    }

//                    // how did this happen?
//                    return false;
//                }
//                else
//                {
//                    // we aren't full, add it
//                    members.Add(member);                    
//                    return true;
//                }
//            }
//            else
//            {
//                // this guy isn't in our bounding box
//                return false;
//            }            
//        }

//        private void CreateChildren()
//        {
//            for (int i = 0; i < ChildCount; i++)
//            {
//                children.Add(new SceneNode(this, GetChildBoundingBox(boundingBox, i)));
//            }

//            createdChildren = true;
//        }
//        // Update scene tree at end of update
//        // have to visit each node and make sure
//        // each member is still in the right node
//        // if not then we pull it out and readd it to the tree

//        public List<RModel> members;
//        public BoundingBox boundingBox;
//        public List<SceneNode> children;
//        public SceneNode parent;
//        private bool createdChildren = false;
//    }
//}
