using System;

namespace Domain.ValueObjects
{
    public class Quantity
    {
        public int Value { get; }

        public Quantity(int value, bool allowNegative = false)
        {
            if (!allowNegative && value < 0)
                throw new ArgumentException("Quantity cannot be negative");

            Value = value;
        }

        public static implicit operator int(Quantity q) => q?.Value ?? 0;
        public static explicit operator Quantity(int v) => new Quantity(v);

 
        public Quantity Increase(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be non-negative", nameof(amount));

            return new Quantity(Value + amount, allowNegative: true);
        }

        public Quantity Decrease(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be non-negative", nameof(amount));

            return new Quantity(Value - amount, allowNegative: true);
        }

      
        public (Quantity NewQuantity, int Given, int Shortfall) Consume(int requested)
        {
            if (requested < 0)
                throw new ArgumentException("Requested amount must be non-negative", nameof(requested));

            int given = Math.Min(Value, requested);
            int shortfall = requested - given;
            var newQuantity = new Quantity(Value - requested, allowNegative: true);
            return (newQuantity, given, shortfall);
        }
    }
}
