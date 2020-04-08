
namespace Engine
{
    public abstract class Manager
    {
        internal abstract void init(params System.Object[] parameters);
        internal abstract void update();
        internal abstract void shutdown();
    }
}