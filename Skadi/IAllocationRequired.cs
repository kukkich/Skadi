namespace Skadi;

public interface IAllocationRequired<in T>
{
    public void Allocate(T param);
}