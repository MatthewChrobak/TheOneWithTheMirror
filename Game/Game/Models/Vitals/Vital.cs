using Annex.Data.Shared;

namespace Game.Models.Vitals
{
    public class Vital
    {
        public Float Current = new Float(100);
        public Float Maximum = new Float(100);
        public Float Regen = new Float(0);

        public Int GetRatio() {
            return new IntRatio(Current, Maximum);
        }

        public Float GetPureRatio() {
            return new FloatRatio(Current, Maximum);
        }
    }
}
