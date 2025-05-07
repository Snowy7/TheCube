using Snowy.Rng.BytesBased;

namespace Snowy.Rng
{
    public class GuidRng : BytesBasedRng
    {
        public GuidRng() : base(new GuidBytes())
        {
        }
    }
}
