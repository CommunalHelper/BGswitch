using Monocle;
using System;

namespace Celeste.Mod.BGswitch {
    [Tracked]
    public class BGModeListener : Component {
        public Action<bool> OnChange;

        public BGModeListener(Action<bool> onChange) 
            : base(false, false) {
            OnChange = onChange;
        }
    }
}
