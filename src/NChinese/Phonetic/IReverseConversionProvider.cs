namespace NChinese
{
    public interface IReverseConversionProvider
    {
        bool IsAvailable { get; }

        string[] Convert(string input);
    }
}
