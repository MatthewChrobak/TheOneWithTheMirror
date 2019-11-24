namespace Annex.Data.Shared
{
    public class IntRatio : Int
    {
        public readonly Float Numerator;
        public readonly Float Denominator;

        public override int Value {
            get => (int)(this.Numerator.Value * 100  / this.Denominator.Value);
            set => this.Denominator.Value = this.Denominator.Value;
        }

        public IntRatio(Float numerator, Float denominator) {
            this.Numerator = numerator;
            this.Denominator = denominator;
        }
    }
}
