﻿using System;
using System.Collections.Generic;

namespace JetPacketSystem.Packeting.Ack;

/// <summary>
/// A class for efficiently storing idempotency keys
/// </summary>
public class IdempotencyKeyStore {
    private readonly Node first;

    /// <summary>
    /// Used for quickly checking if a key has been used
    /// </summary>
    private uint highest;

    public IdempotencyKeyStore() {
        this.first = new Node();
        this.first.range = new Range(0);
    }

    public bool Put(uint key) {
        if (key < 1) {
            throw new Exception("Key cannot be below 1, it must be 1 or above");
        }

        if (key > this.highest) {
            this.highest = key;
        }

        Node node = this.first;
        Node prev = node;
        Range range = node.range;
        while (true) {
            if (range.IsBetween(key)) {
                return false;
            }
            else if (range.IsAbove(key)) {
                if (node.range.max == (key - 1)) {
                    node.range.IncrMax();
                    if (node.next == null) {
                        return true;
                    }
                    else if (node.next.range.min == (key + 1)) {
                        node.range.SetMax(node.next.range.max);
                        node.next.Remove();
                        return true;
                    }
                }
                else {
                    prev = node;
                    node = node.next;
                    if (node == null) {
                        Node newNode = new Node();
                        newNode.range = new Range(key);
                        newNode.AddAfter(prev);
                        return true;
                    }
                    else {
                        range = node.range;
                    }
                }
            }
            else if (range.IsBelow(key)) {
                if (node.prev == prev) {
                    bool prevIncrement = false;
                    if (prev.range.max == (key - 1)) {
                        prev.range.IncrMax();
                        prevIncrement = true;
                    }
                    if (node.range.min == (key + 1)) {
                        if (prevIncrement) {
                            prev.range.SetMax(node.range.max);
                            node.Remove();
                        }

                        return true;
                    }
                    else if (prevIncrement) {
                        return true;
                    }
                    else {
                        Node newNode = new Node();
                        newNode.range = new Range(key);
                        newNode.InsertBetween(prev, node);
                        return true;
                    }
                }
                else {
                    prev = node;
                    node = node.prev;
                    if (node == null) {
                        throw new Exception("Huh...");
                    }

                    range = node.range;
                }
            }
            else {
                throw new Exception("What.....");
            }
        }
    }

    public bool HasKey(uint key) {
        if (key < 1) {
            throw new Exception("Key cannot be below 1, it must be 1 or above");
        }

        if (key > this.highest) {
            return false;
        }
        else {
            Node node = this.first;
            while(node != null) {
                if (node.range.IsBetween(key)) {
                    return true;
                }
                else if (node.range.IsBelow(key)) {
                    return false;
                }
                else {
                    // it's above, so just continue and ignore it
                    node = node.next;
                }
            }

            return false;
        }
    }


    // [1-4] [7-20] [45-50]
    // Removing 11, goes to
    // [1-4] [7-10] [12-20] [45-50]
    // 
    // [1-4] [7-20] [45-50]
    // Removing 20, goes to
    // [1-4] [7-19] [45-50]
    public bool Remove(uint key) {
        if (key > this.highest) {
            return false;
        }

        Node node = this.first;
        Node prev = node;
        Range range = node.range;
        while (true) {
            if (range.IsBetween(key)) {
                if (key == range.max) {
                    node.range.DecrMax();
                    return true;
                }
                else if (key == range.min) {
                    node.range.IncrMin();
                    return true;
                }
                else {
                    node.range.max = (key - 1);
                    Node newNode = new Node();
                    newNode.range = new Range(key + 1, range.max);
                    newNode.InsertAfter(node);
                    return true;
                }
            }
            else if (range.IsAbove(key)) {
                prev = node;
                node = node.next;
                if (node == null) {
                    return false;
                }
                else {
                    range = node.range;
                }
            }
            else if (range.IsBelow(key)) {
                if (node.prev == prev) {
                    return false;
                }

                prev = node;
                node = node.prev;
                if (node == null) {
                    return false;
                }

                range = node.range;
            }
            else {
                throw new Exception("What.....");
            }
        }
    }

    public void Clear() {
        Node next = this.first;
        while (next != null) {
            Node node = next;
            next = next.next;
            node.Invalidate();
        }

        this.first.Invalidate();
        this.first.range = new Range(0);
    }

    public IEnumerable<uint> GetEnumerator() {
        Node node = this.first;
        while(node != null) {
            for(uint i = node.range.min, end = node.range.max + 1; i < end; i++) {
                yield return i;
            }

            node = node.next;
        }
    }

    // [] --- [] --- []
    // [] --- []
    // []
    //        []
    //               []
    // [] --- US --- [] --- []
    // [] --- [] --- US --- []
    private class Node {
        public Node prev;
        public Node next;
        public Range range;

        public Node() {

        }

        /// <summary>
        /// Makes this node the given node's next node
        /// </summary>
        /// <param name="node">The new previous node</param>
        public void AddAfter(Node node) {
            node.next = this;
            this.prev = node;
        }

        /// <summary>
        /// Makes this node the given node's previous node
        /// </summary>
        /// <param name="node">The new next node</param>
        public void AddBefore(Node node) {
            node.prev = this;
            this.next = node;
        }

        public void InsertAfter(Node node) {
            if (node.next != null) {
                this.next = node.next;
                node.next.prev = this;
            }

            node.next = this;
            this.prev = node;
        }

        public void InsertBefore(Node node) {
            if (node.prev != null) {
                this.prev = node.prev;
                node.prev.next = this;
            }

            node.prev = this;
            this.next = node;
        }

        /// <summary>
        /// Connects the prev and next together, removing this entirely
        /// </summary>
        public void Remove() {
            // avoid multiple ldfld opcode
            Node next = this.next;
            Node prev = this.prev;
            if (next == null || prev == null) {
                if (prev == null) {
                    if (next == null) {
                        return;
                    }
                    else {
                        next.prev = null;
                        this.next = null;
                    }
                }
                else {
                    prev.next = null;
                    this.prev = null;
                }
            }
            else {
                prev.next = this.next;
                next.prev = this.prev;
                this.prev = null;
                this.next = null;
            }
        }

        /// <summary>
        /// Sets the next and prev nodes to null
        /// </summary>
        public void Invalidate() {
            this.next = null;
            this.prev = null;
        }

        /// <summary>
        /// Inserts this node inbetween the 2 other nodes (making sure to connect all of the next/prev nodes of the given nodes too)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void InsertBetween(Node a, Node b) {
            if (a != null) {
                this.AddAfter(a);
            }
            if (b != null) {
                this.AddBefore(b);
            }
        }

        public override string ToString() {
            return $"RangeNode({this.range.min} -> {this.range.max})";
        }
    }

    private struct Range {
        public uint min;
        public uint max;

        public Range(uint value) {
            this.min = value;
            this.max = value;
        }

        public Range(uint min, uint max) {
            this.min = min;
            this.max = max;
        }

        public void IncrMin() {
            this.min++;
        }

        public void IncrMax() {
            this.max++;
        }

        public void DecrMin() {
            this.min--;
        }

        public void DecrMax() {
            this.max--;
        }

        public void SetMin(uint min) {
            this.min = min;
        }

        public void SetMax(uint max) {
            this.max = max;
        }

        public bool IsBetween(uint value) {
            return value >= this.min && value <= this.max;
        }

        public bool IsNotBetween(uint value) {
            return value < this.min || value > this.max;
        }

        public bool IsAbove(uint value) {
            return value > this.max;
        }

        public bool IsBelow(uint value) {
            return value < this.min;
        }

        public override string ToString() {
            return $"Range({this.min} -> {this.max})";
        }
    }
}