# ResourceFactory

### Overview
ResourceFactory is an integrated object creation framework that supports various object creation methods such as Addressable, VContainer, and Pooling.

### Features
- Provides consistent API for various creation methods including Pooling, Addressable, and VContainer
- Addressable support (requires adding Scripting Define Symbol for ADDRESSABLE_SUPPORT)
- VContainer support (requires adding Scripting Define Symbol for VCONTAINER_SUPPORT)

### Requirements
#### Required Libraries
- UniRx
- UniTask

#### Optional Libraries
- VContainer
- Addressable

#### Scripting Define Symbols
- UNIRX_SUPPORT
- UNITASK_SUPPORT
- VCONTAINER_SUPPORT (optional)
- ADDRESSABLE_SUPPORT (optional)

## Installation

### Via Package Manager

For Unity 2019.3.4f1 or higher, you can install the package directly through the Package Manager using a Git URL.

1. Open Package Manager (Window > Package Manager)
2. Click '+' button and select "Add package from git URL"
3. Enter the following URL:
```
https://github.com/jinhosung96/ResourceFactoryForUnity.git
```

Alternatively, you can add it directly to your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.jhs-library.auto-path-generator": "https://github.com/jinhosung96/ResourceFactoryForUnity.git"
  }
}
```

To install a specific version, add the #{version} tag to the URL:
```
https://github.com/jinhosung96/ResourceFactoryForUnity.git#1.0.0
```

### Usage

#### Factory Types
ResourceFactory provides two types:
1. `ResourceFactory<T> where T : Object`
   - Creates on each Load request and destroys on each Release request
2. `ResourcePoolFactory<T> where T : Object`
   - Creates a pool with predetermined size for reuse

#### Code Examples
```csharp
public ResourceFactroy<RoomButton> RoomButtonFactory { get; }
public ResourcePoolFactroy<GameObject> ProjectileFactory { get; }

// Loading resources
var roomButton = await RoomButtonFactory.LoadAsync(roomButtonTransform);
RoomButtonFactory.Release(roomButton);

var projectile = await ProjectileFactory.LoadAsync(projectileTransform);
ProjectileFactory.Release(projectile);
```
