using Nexticz.Module.Mmo.Washing.Infrastructure.KitCounters;
using Shouldly;

namespace Nexticz.Module.Mmo.Washing.Tests.Unit.Infrastructure;

public class GlobalKitsCounterTests
{
    private readonly GlobalKitsCounter _counter = CreateSut();

    [Fact]
    public void GetAndIncrementCurrentValue_ConsecutiveCalls_IncrementsCorrectly()
    {

        // Act
        var first = _counter.GetAndIncrementCurrentValue();
        var second = _counter.GetAndIncrementCurrentValue();
        var third = _counter.GetAndIncrementCurrentValue();

        // Assert
        first.ShouldBe(1);
        second.ShouldBe(2);
        third.ShouldBe(3);
    }

    [Fact]
    public async Task GetAndIncrementCurrentValue_ConcurrentCalls_ThreadSafe()
    {
        // Act
        const int threadCount = 1000;
        var tasks = new Task[threadCount];
        var results = new int[threadCount];

        for (var i = 0; i < threadCount; i++)
        {
            var index = i;
            tasks[i] = Task.Run(() => results[index] = _counter.GetAndIncrementCurrentValue());
        }

        await Task.WhenAll(tasks);

        // Assert
        Array.Sort(results);
        for (var i = 0; i < threadCount; i++)
        {
            results[i].ShouldBe(i + 1); // Expect 1, 2, ..., 1000
        }
    }

    private static GlobalKitsCounter CreateSut() => new();
}