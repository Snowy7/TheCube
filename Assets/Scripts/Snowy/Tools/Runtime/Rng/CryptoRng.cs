using Snowy.Rng.BytesBased;

namespace Snowy.Rng
{
    public class CryptoRng : BytesBasedRng
    {
        public CryptoRng() : base(new CryptoBytes())
        {

        }
    }
}
