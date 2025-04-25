namespace NJG.Runtime.Interfaces
{
    public interface IInsertReceiver<T>
    {
        public bool TryInsert(T insertable);

        public T RemoveInsertable();
    }
}