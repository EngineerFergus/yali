namespace CSLox
{
    internal class Return : Exception
    {
        public readonly object? Value;

        public Return(object? value)
        {
            Value = value;
        }
    }
}
