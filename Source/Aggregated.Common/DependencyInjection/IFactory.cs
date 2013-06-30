namespace Aggregated.Common.DependencyInjection
{
    public interface IFactory<out T>
    {
        T Make();
    }
}
