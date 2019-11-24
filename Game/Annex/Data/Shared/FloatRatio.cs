namespace Annex.Data.Shared
{
    public class FloatRatio : Float
    {
        public readonly Float Numerator;
        public readonly Float Denominator;

        public override float Value {
            get => this.Numerator.Value / this.Denominator.Value;
            set => this.Denominator.Value = this.Denominator.Value;
        }

        public FloatRatio(Float numerator, Float denominator) {
            this.Numerator = numerator;
            this.Denominator = denominator;
        }
    }
}
