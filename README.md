# NS.Shared.CacheProvider

A caching provider for .NET applications, offering a straightforward and efficient way to handle caching within your projects.

## Overview

`NS.Shared.CacheProvider` provides an easy-to-use interface for managing cache operations such as retrieving, adding, updating, and deleting cached items. It supports asynchronous operations, making it suitable for modern .NET applications.

## Installation

Install the package via the NuGet Package Manager Console:

```bash
Install-Package NS.Shared.CacheProvider
```

Or use the .NET CLI:

```bash
dotnet add package NS.Shared.CacheProvider
```

Alternatively, you can add the package directly to your project file:

```xml
<PackageReference Include="NS.Shared.CacheProvider" Version="x.y.z" />
```

Replace `x.y.z` with the latest version number.

## Getting Started

### Configuration

To configure the caching provider in your application, add the provider to your services in the `Startup` class or equivalent:

```csharp
using NS.Shared.CacheProvider;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add NSCacheProvider to the services
        services.AddNSCacheProvider();
    }
}
```

### Using INSCacheProvider

Inject `INSCacheProvider` into your classes to use the caching functionality:

```csharp
using NS.Shared.CacheProvider;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SampleService
{
    private readonly INSCacheProvider _cacheProvider;

    public SampleService(INSCacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }

    public async Task<string> GetDataAsync(string key)
    {
        // Retrieve data from cache
        var cachedData = await _cacheProvider.GetAsync<string>(key);

        if (cachedData != null)
        {
            return cachedData;
        }

        // If not cached, perform data fetching logic and cache the result
        var data = "Your data fetching logic here";
        await _cacheProvider.SetOrUpdateAsync(key, data, TimeSpan.FromMinutes(10));

        return data;
    }

    public async Task<List<string>> GetAllKeysAsync()
    {
        // Get all cache keys
        return await _cacheProvider.GetKeysAsync();
    }

    public async Task DeleteDataAsync(string key)
    {
        // Delete data from cache
        await _cacheProvider.DeleteAsync<string>(key);
    }
}
```

### Interface Methods

`INSCacheProvider` offers the following methods:

- `Task<List<string>> GetKeysAsync();`  
  Retrieves all keys from the cache.

- `Task<T?> GetAsync<T>(string key);`  
  Retrieves a cached item by key.

- `Task DeleteAsync<T>(string key);`  
  Deletes a cached item by key.

- `Task SetOrUpdateAsync<T>(string key, T value, TimeSpan? expiryTime = null);`  
  Adds or updates a cached item, with an optional expiration time.

## Support

For support or questions, please send an email to [Nashef.Systems@gmail.com](mailto:Nashef.Systems@gmail.com).
