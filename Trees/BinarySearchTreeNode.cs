﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trees
{
    internal class BinarySearchTreeNode<T> : BinaryTreeNode<T> where T : IComparable<T>
    {
        private bool RemoveUsingPredecessor = false;

        public BinarySearchTreeNode() : base() { }
        public BinarySearchTreeNode(T data) : base(data, null) { }
        public BinarySearchTreeNode(T data, BinarySearchTreeNode<T> left, BinarySearchTreeNode<T> right)
        {
            base.Value = data;
            NodeList<T> children = new NodeList<T>(2);
            children[0] = left;
            children[1] = right;

            base.Neighbors = children;
        }

        public new BinarySearchTreeNode<T> Left
        {
            get
            {
                if (base.Neighbors == null) {
                    return null;
                } else {
                    return (BinarySearchTreeNode<T>) base.Neighbors[0];
                }
            }
            set
            {
                if (base.Neighbors == null) {
                    base.Neighbors = new NodeList<T>(2);
                }

                base.Neighbors[0] = value;
            }
        }

        public new BinarySearchTreeNode<T> Right
        {
            get
            {
                if (base.Neighbors == null) {
                    return null;
                } else {
                    return (BinarySearchTreeNode<T>) base.Neighbors[1];
                }
            }
            set
            {
                if (base.Neighbors == null) {
                    base.Neighbors = new NodeList<T>(2);
                }

                base.Neighbors[1] = value;
            }
        }

        public delegate object VisitNode(params object[] arguments);
        //arguments[0]: input data
        //arguments[1]: null or node containing arguments[0]
        //arguments[2]: null or parent of arguments[1]
        //arguments[3]: true if arguments[1] is Left child of arguments[2]
        public struct VisitNodeArgument
        {
            public const int Data = 0,
            Node = 1,
            Parent = 2,
            WentLeft = 3;
        }

        public static VisitNode VisitReturnTrue =
                (object[] arguments) => {
                    return true;
                };

        public static VisitNode VisitReturnFalse =
                (object[] arguments) => {
                    return false;
                };

        public static VisitNode VisitReturnNull =
                (object[] arguments) => {
                    return null;
                };

        public virtual object FindAndVisit(T data, VisitNode onSuccess, VisitNode onFailure)
        {
            bool wentLeft = false;
            object[] arguments = { data, this, null, false };
            BinarySearchTreeNode<T> previous = null;

            for (BinaryTreeNode<T> current = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Node];
                current != null;
                arguments[VisitNodeArgument.Node] = current) {
                int result = current.Value.CompareTo(data);

                if (result == 0) {
                    return onSuccess(data, current, previous, wentLeft);
                } else if (result > 0) {
                    arguments[VisitNodeArgument.WentLeft] = true;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = current.Left;
                } else {
                    arguments[VisitNodeArgument.WentLeft] = false;

                    arguments[VisitNodeArgument.Parent] = current;
                    current = current.Right;
                }
            }

            return onFailure(arguments);
        }

        public virtual bool Contains(T data)
        {
            return (bool) FindAndVisit(data, VisitReturnTrue, VisitReturnFalse);
        }

        internal virtual BinarySearchTreeNode<T> Find(T data)
        {
            VisitNode onSuccess =
                (object[] arguments) => {
                    return arguments[VisitNodeArgument.Node];
                };

            return (BinarySearchTreeNode<T>) FindAndVisit(data, onSuccess, VisitReturnNull);
        }

        internal virtual BinarySearchTreeNode<T> GetParentOf(T data)
        {
            VisitNode onSuccess =
                (object[] arguments) => {
                    return arguments[VisitNodeArgument.Parent];
                };

            return (BinarySearchTreeNode<T>) FindAndVisit(data, onSuccess, VisitReturnNull);
        }

        public virtual void Add(T data)
        {
            VisitNode onFailure =
                (object[] arguments) => {
                    T leafData = (T) arguments[VisitNodeArgument.Data];
                    var leaf = new BinarySearchTreeNode<T>(leafData);

                    var parent = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Parent];
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    if (wentLeft) {
                        parent.Left = leaf;
                    } else {
                        parent.Right = leaf;
                    }

                    return true;
                };

            FindAndVisit(data, VisitReturnFalse, onFailure);
        }


        public virtual bool Remove(T data)
        {
            return Remove(data, !(RemoveUsingPredecessor = !RemoveUsingPredecessor));
        }

        public bool Remove(T data, bool removeUsingPredecessor)
        {
            VisitNode onSuccessUsingPredecessor =
                 (object[] arguments) => {
                     var node = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Node];
                     var parent = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Parent];
                     var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                     if (node.Left == null) {
                         if (!wentLeft) {
                             parent.Right = node.Right;
                         } else {
                             parent.Left = node.Right;
                         }
                     } else {
                         BinarySearchTreeNode<T> predecessorParent, predecessor = GetPredecessorOf(node, out predecessorParent);

                         predecessorParent.Right = predecessor.Left;

                         predecessor.CopyChildrenOf(node);

                         if (!wentLeft) {
                             parent.Right = predecessor;
                         } else {
                             parent.Left = predecessor;
                         }
                     }

                     return true;
                 };
            VisitNode onSuccessUsingSuccessor =
                (object[] arguments) => {
                    var node = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Node];
                    var parent = (BinarySearchTreeNode<T>) arguments[VisitNodeArgument.Parent];
                    var wentLeft = (bool) arguments[VisitNodeArgument.WentLeft];

                    if (node.Right == null) {
                        if (wentLeft) {
                            parent.Left = node.Left;
                        } else {
                            parent.Right = node.Left;
                        }
                    } else {
                        BinarySearchTreeNode<T> successorParent, successor = GetSuccessorOf(node, out successorParent);

                        successorParent.Left = successor.Right;

                        successor.CopyChildrenOf(node);

                        if (wentLeft) {
                            parent.Left = successor;
                        } else {
                            parent.Right = successor;
                        }
                    }

                    return true;
                };

            return (bool) FindAndVisit(data,
                removeUsingPredecessor ? onSuccessUsingPredecessor : onSuccessUsingSuccessor,
                VisitReturnFalse);
        }

        internal virtual BinarySearchTreeNode<T> Min()
        {
            BinarySearchTreeNode<T> dummy;
            return Min(out dummy);
        }

        internal BinarySearchTreeNode<T> Min(out BinarySearchTreeNode<T> successorParent)
        {
            successorParent = null;
            var node = this;
            if (node != null) {
                while (node.Left != null) {
                    successorParent = node;
                    node = (BinarySearchTreeNode<T>) node.Left;
                }
            }
            return  node;
        }

        internal virtual BinarySearchTreeNode<T> Max()
        {
            BinarySearchTreeNode<T> dummy;
            return Max(out dummy);
        }

        internal BinarySearchTreeNode<T> Max(out BinarySearchTreeNode<T> successorParent)
        {
            successorParent = null;
            var node = this;
            if (node != null) {
                while (node.Right != null) {
                    successorParent = node;
                    node = (BinarySearchTreeNode<T>) node.Right;
                }
            }
            return node;
        }

        internal BinarySearchTreeNode<T> GetPredecessorOf(BinarySearchTreeNode<T> node)
        {
            BinarySearchTreeNode<T> dummy;
            return GetPredecessorOf(node, out dummy);
        }

        internal BinarySearchTreeNode<T> GetPredecessorOf(BinarySearchTreeNode<T> node, out BinarySearchTreeNode<T> successorParent)
        {
            successorParent = null;
            if (node == null || node.Right == null) {
                return null;
            }
            return Max(out successorParent);
        }

        internal BinarySearchTreeNode<T> GetSuccessorOf(BinarySearchTreeNode<T> node)
        {
            BinarySearchTreeNode<T> dummy;
            return GetSuccessorOf(node, out dummy);
        }

        internal BinarySearchTreeNode<T> GetSuccessorOf(BinarySearchTreeNode<T> node, out BinarySearchTreeNode<T> successorParent)
        {
            successorParent = null;
            if (node == null || node.Left == null) {
                return null;
            }
            return Min(out successorParent);
        }

    }
}
