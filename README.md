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

## 설치 방법

### Package Manager를 통한 설치

Unity 2019.3.4f1 이상 버전에서는 Package Manager에서 직접 Git URL을 통해 설치할 수 있습니다.

1. Package Manager 창을 엽니다 (Window > Package Manager)
2. '+' 버튼을 클릭하고 "Add package from git URL"을 선택합니다
3. 다음 URL을 입력합니다:
```
https://github.com/jinhosung96/ResourceFactoryForUnity.git
```

또는 `Packages/manifest.json` 파일에 직접 추가할 수 있습니다:
```json
{
  "dependencies": {
    "com.jhs-library.auto-path-generator": "https://github.com/jinhosung96/ResourceFactoryForUnity.git"
  }
}
```

특정 버전을 설치하려면 URL 뒤에 #{version} 태그를 추가하면 됩니다:
```
https://github.com/jinhosung96/ResourceFactoryForUnity.git#1.0.0
```

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
