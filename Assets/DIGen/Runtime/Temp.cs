namespace DIGen.Runtime
{
    public abstract class Temp
    {
        public virtual bool IsRunning { get; set; }
        public bool Run()
        {
            return IsRunning;
        }
    }
}