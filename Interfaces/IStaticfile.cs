namespace static_sv.Interfaces
{
    public interface IStaticfile
    {
        public Task DeleteImage(string url, string xStaticSig);
    }
}