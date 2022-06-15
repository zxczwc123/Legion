// ========================================================
// 描 述：Circlebytes.cs
// 功 能：环形缓存区
// 创 建：高辉
// 时 间：2021/11/16 14:59:01 
// 版 本：2020.3.18f1c1 
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace Framework.SocketNet {
    
    public class Circlebytes : IDisposable {

        public int MaxCapacity {
            get {
                return 8192;
            }
        }

        private Queue<byte[]> _queueBytes = new Queue<byte[]>();

        private Queue<byte[]> _queueCache = new Queue<byte[]>();

        public byte[] _lastBytes;

        public int LastIndex { get; set; }

        public int FirstIndex { get; set; }

        public byte[] FirstBytes {
            get {
                if (this._queueBytes.Count == 0) {
                    this.AddQueue();
                }

                return this._queueBytes.Peek();
            }
        }

        public int Length {
            get {
                int c;
                if (this._queueBytes.Count == 0) c = 0;
                else c = (this._queueBytes.Count - 1) * MaxCapacity + this.LastIndex - this.FirstIndex;
                if (c < 0)
                {
                    Debug.LogError($"CircularBuffer count < 0: {this._queueBytes.Count}, {this.LastIndex}, {this.FirstIndex}");
                }
                return c;
            }
        }

        public Circlebytes() {
            this.AddQueue();
        }

        public void AddQueue() {
            byte[] bytes = null;
            if (this._queueCache.Count > 0) {
                bytes = this._queueCache.Dequeue();
            } else {
                bytes = new byte[MaxCapacity];
            }
            this._queueBytes.Enqueue(bytes);
            this._lastBytes = bytes;
        }

        public void RemoveFirst() {
            this._queueCache.Enqueue(this._queueBytes.Dequeue());
        }

        public void Write(byte[] writeBytes, int offset, int count) {
            int alreadyCopyCount = 0;
            while (count - alreadyCopyCount > 0) {
                if (this._queueBytes.Count == 0 || this.LastIndex == MaxCapacity) {
                    this.AddQueue();
                    this.LastIndex = 0;
                }
                int n = count - alreadyCopyCount;
                if (this.MaxCapacity - this.LastIndex > n) {
                    Array.Copy(writeBytes,offset + alreadyCopyCount,this._lastBytes,this.LastIndex,count - alreadyCopyCount);
                    this.LastIndex += n;
                    alreadyCopyCount += n;
                } else {
                    Array.Copy(writeBytes,offset + alreadyCopyCount,this._lastBytes,this.LastIndex,this.MaxCapacity - this.LastIndex);
                    alreadyCopyCount += this.MaxCapacity - this.LastIndex;
                    this.LastIndex = this.MaxCapacity;
                }
            }
        }

        public void Read(byte[] targetBytes, int offset, int count) {
            if (this._queueBytes.Count == 0 || this.Length < count) 
                throw new Exception($"Read From Empty or Length don't Enough : QueueCount : {this._queueBytes.Count} Count : {count} Length : {this.Length}");
            int alreadyReadCount = 0;
            while (count - alreadyReadCount > 0) {
                int n = count - alreadyReadCount;
                if (this.MaxCapacity - this.FirstIndex > n) {
                    Array.Copy(this.FirstBytes,this.FirstIndex,targetBytes,offset + alreadyReadCount,count - alreadyReadCount);
                    alreadyReadCount += n;
                    this.FirstIndex += n;
                } else {
                    Array.Copy(this.FirstBytes,this.FirstIndex,targetBytes,offset + alreadyReadCount,this.MaxCapacity - this.FirstIndex);
                    alreadyReadCount += this.MaxCapacity - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }
        }

        public void Dispose() {
            this._queueBytes = null;
            this._queueCache = null;
            this._lastBytes = null;
        }
    }
}
