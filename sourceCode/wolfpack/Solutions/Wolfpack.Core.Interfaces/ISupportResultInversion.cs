namespace Wolfpack.Core.Interfaces
{
    public interface ISupportResultInversion
    {
        bool InterpretZeroRowsAsAFailure { get; set; } 
    }
}