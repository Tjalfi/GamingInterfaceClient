namespace GamingInterfaceClient.Network
{
    class CommandResult<T>
    {
        public bool successful = false;
        public string error = "";
        public T response = default(T);
    }
}
