using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.RuleProviders.CacheProviders;

namespace Nager.PublicSuffix.UnitTest.CacheProviders;

[TestClass]
public class MemoryCacheProviderTest
{
    private MemoryCacheProvider _target;
    
    [TestInitialize]
    public void Initialize()
    {
        _target = new MemoryCacheProvider();
    }
    
    [TestMethod]
    public async Task No_Data_Set()
    {   
        Assert.IsFalse(_target.IsCacheValid());
        Assert.IsNull(await _target.GetAsync()!);
    }
    
    [TestMethod]
    public async Task Valid_Data_Set()
    {
        await _target.SetAsync("d");
        Assert.IsTrue(_target.IsCacheValid());
        Assert.AreEqual("d", await _target.GetAsync()!);
    }
    
    [TestMethod]
    public async Task Expired_Data_Set()
    {
        _target = new MemoryCacheProvider(TimeSpan.FromMilliseconds(500));
        await _target.SetAsync("d");
        await Task.Delay(1000);
        Assert.IsFalse(_target.IsCacheValid());
        Assert.IsNull(await _target.GetAsync()!);
    }
}