namespace Wolfpack.Core.Containers
{
    public class If<T>
    {
        public static bool IsNotRegistered()
        {
            return !Container.IsRegistered(typeof (T));
        }
    }
}