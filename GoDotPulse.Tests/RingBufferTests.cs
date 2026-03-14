using Godot;
using Xunit;
using GodotPulse;

namespace GoDotPulse.Tests;

public class RingBufferTests
{
    [Fact]
    public void Constructor_WithValidSize_Succeeds()
    {
        var buffer = new RingBuffer<float>(10);
        Assert.Equal(0, buffer.Count);
        Assert.Equal(10, buffer.MaxSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidSize_Throws(int size)
    {
        var ex = Assert.Throws<ArgumentException>(() => new RingBuffer<float>(size));
        Assert.Contains("positive", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Push_IncrementsCount()
    {
        var buffer = new RingBuffer<float>(5);
        buffer.Push(1.0f);
        Assert.Equal(1, buffer.Count);
        buffer.Push(2.0f);
        Assert.Equal(2, buffer.Count);
    }

    [Fact]
    public void Push_DoesNotExceedMaxSize()
    {
        var buffer = new RingBuffer<float>(3);
        buffer.Push(1.0f);
        buffer.Push(2.0f);
        buffer.Push(3.0f);
        buffer.Push(4.0f); // Overwrites the first value
        Assert.Equal(3, buffer.Count);
    }

    [Fact]
    public void Get_ReturnsCorrectValues()
    {
        var buffer = new RingBuffer<float>(5);
        buffer.Push(10.0f);
        buffer.Push(20.0f);
        buffer.Push(30.0f);

        Assert.Equal(10.0f, buffer.Get(0));
        Assert.Equal(20.0f, buffer.Get(1));
        Assert.Equal(30.0f, buffer.Get(2));
    }

    [Fact]
    public void Get_OutOfRange_ReturnsDefault()
    {
        var buffer = new RingBuffer<float>(5);
        buffer.Push(10.0f);
        
        Assert.Equal(default, buffer.Get(-1));
        Assert.Equal(default, buffer.Get(1)); // Only 1 element, so index 1 is out of range
        Assert.Equal(default, buffer.Get(100));
    }

    [Fact]
    public void ToArray_ReturnsCorrectOrder()
    {
        var buffer = new RingBuffer<float>(5);
        buffer.Push(10.0f);
        buffer.Push(20.0f);
        buffer.Push(30.0f);

        var array = buffer.ToArray();
        Assert.Equal(3, array.Length);
        Assert.Equal(10.0f, array[0]);
        Assert.Equal(20.0f, array[1]);
        Assert.Equal(30.0f, array[2]);
    }

    [Fact]
    public void ToArray_WithWrap_ReturnsCorrectOrder()
    {
        var buffer = new RingBuffer<float>(3);
        buffer.Push(1.0f);
        buffer.Push(2.0f);
        buffer.Push(3.0f);
        buffer.Push(4.0f); // Wraps, overwrites 1
        buffer.Push(5.0f); // Wraps, overwrites 2

        var array = buffer.ToArray();
        Assert.Equal(3, array.Length);
        Assert.Equal(3.0f, array[0]); // Oldest
        Assert.Equal(4.0f, array[1]);
        Assert.Equal(5.0f, array[2]); // Newest
    }
}

public class GodotPulseConfigTests
{
    [Fact]
    public void Config_DefaultValues_AreValid()
    {
        var config = new GodotPulseConfig();
        Assert.NotEmpty(config.ToggleAction);
        Assert.True(config.Opacity >= 0 && config.Opacity <= 1);
        Assert.True(config.Scale > 0);
        Assert.False(config.EnableInRelease);
        Assert.NotEmpty(config.ThemePath);
    }

    [Fact]
    public void Config_ThresholdValues_AreConsistent()
    {
        var config = new GodotPulseConfig();
        Assert.True(config.DrawCallWarning < config.DrawCallCritical);
        Assert.True(config.TargetFps > 0);
    }
}
