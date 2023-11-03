namespace LogRipper.Models
{
    internal interface IProgressionPackUnpack
    {
        void SetProgress(long totalSize, long currentPos);
    }
}
