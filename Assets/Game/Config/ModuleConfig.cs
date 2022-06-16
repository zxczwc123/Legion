using Game.Framework;

namespace Game.Config
{
    public class ModuleConfig
    {
        public static ModuleInfo[] Value = new ModuleInfo[]
        {
            new ModuleInfo() {name = "Login", type = typeof(Login.Login)},
        };
    }
}