﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Trees
{

    public abstract class BinaryTree<T> : EnumerableBinaryTree<T>, ICollection, ICollection<T> where T : IComparable<T>, ICloneable
    {
        private BinaryTreeNode<T> root = null;

        public BinaryTree() { }

        internal override IEnumerableBinaryTreeNode<T> Root
        {
            get
            {
                return root;
            }
            set
            {
                root = (BinaryTreeNode<T>) value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return root == null;
            }
        }

        public abstract void Add(T item);

        public abstract bool Contains(T item);

        public abstract int Count { get; }

        public abstract bool IsReadOnly { get; }

        public abstract bool Remove(T item);

        public abstract void CopyTo(Array array, int index);

        public abstract void CopyTo(T[] array, int arrayIndex);

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }
    }
}
