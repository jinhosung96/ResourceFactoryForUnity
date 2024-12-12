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
