// ========================================================
// 描 述：EventManager.cs 
// 作 者： 
// 时 间：2020/01/03 17:46:46 
// 版 本：2019.2.1f1 
// ========================================================

using System;
using System.Collections.Generic;
using Game.Common;

namespace Framework.Core {
    public class UpdateManager : MonoSingleton<UpdateManager> {

        private List<Action> _updates = new List<Action>();

        public void AddUpdate(Action update) {
            if (this._updates.Contains(update)) {
                return;
            }
            this._updates.Add(update);
        }

        public void DelUpdate(Action update) {
            if (!this._updates.Contains(update)) {
                return;
            }
            this._updates.Remove(update);
        }

        public void Update() {
            for(var i = 0; i < this._updates.Count; i++) {
                var update = this._updates[i];
                update();
            }
        }
        
        protected override void OnInit()
        {
            
        }

        protected override void OnDestroy()
        {
            
        }
    }
}