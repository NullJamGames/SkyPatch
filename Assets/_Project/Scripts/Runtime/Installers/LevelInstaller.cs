using Zenject;

namespace NJG.Runtime.Installers
{
    public class LevelInstaller : MonoInstaller<LevelInstaller>
    {
        public override void InstallBindings()
        {
            // TODO: probably don't need to make it as POCO, makes it easier for designer..
            //Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle().NonLazy();
        }
    }
}