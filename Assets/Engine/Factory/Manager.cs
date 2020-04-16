namespace Engine.Factory
{
    public abstract class Manager
    {
        public abstract void init(params System.Object[] parameters);
        public abstract void update();
        public abstract void shutdown();
    }
}