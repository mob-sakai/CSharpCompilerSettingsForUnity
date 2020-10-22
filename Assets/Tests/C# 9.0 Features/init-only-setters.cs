/*
[NOT SUPPORTED] >> .Net 5 is required.

Init Only Setters

This proposal adds the concept of init only properties and indexers to C#.
These properties and indexers can be set at the point of object creation
but become effectively `get` only once object creation has completed.
This allows for a much more flexible immutable model in C#.
*/

#if NET_5
namespace CSharp_9_Features.InitOnlySetters
{
    using System;

    public struct WeatherObservation
    {
        public DateTime RecordedAt { get; init; }
        public decimal TemperatureInCelsius { get; init; }
        public decimal PressureInMillibars { get; init; }

        public override string ToString() =>
            $"At {RecordedAt:h:mm tt} on {RecordedAt:M/d/yyyy}: " +
            $"Temp = {TemperatureInCelsius}, with {PressureInMillibars} pressure";
    }
}
#endif
