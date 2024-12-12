# ResourceFactory

### 개요
ResourceFactory는 Addressable, VContainer, Pooling 등의 다양한 객체 생성 방식에 대응하는 통합 객체 생성 프레임워크입니다.

### 특징
- Pooling, Addressable, VContainer 등의 다양한 생성 방식을 일관된 API로 제공
- Addressable 지원 (ADDRESSABLE_SUPPORT에 대한 Scripting Define Symbol 추가 필요)
- VContainer 지원 (VCONTAINER_SUPPORT에 대한 Scripting Define Symbol 추가 필요)

### 요구 사항
#### 필수 라이브러리
- UniRx
- UniTask

#### 선택 라이브러리
- VContainer
- Addressable

#### Scripting Define Symbols
- UNIRX_SUPPORT
- UNITASK_SUPPORT
- VCONTAINER_SUPPORT (선택)
- ADDRESSABLE_SUPPORT (선택)

### 사용 방법

#### Factory 생성
ResourceFactory는 두 가지 타입을 제공합니다:
1. `ResourceFactory<T> where T : Object`
   - Load 요청 시마다 생성하고 Release 요청 시마다 파괴
2. `ResourcePoolFactory<T> where T : Object`
   - 미리 지정된 개수만큼 풀을 생성하여 재사용

#### 예제 코드
```csharp
public ResourceFactroy<RoomButton> RoomButtonFactory { get; }
public ResourcePoolFactroy<GameObject> ProjectileFactory { get; }

// 리소스 로드
var roomButton = await RoomButtonFactory.LoadAsync(roomButtonTransform);
RoomButtonFactory.Release(roomButton);

var projectile = await ProjectileFactory.LoadAsync(projectileTransform);
ProjectileFactory.Release(projectile);
```
