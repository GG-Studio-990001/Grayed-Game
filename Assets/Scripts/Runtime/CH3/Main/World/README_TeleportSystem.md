# 텔레포트 타워 시스템 사용 방법

## 개요
텔레포트 타워 시스템은 베이스캠프와 TRPG 필드의 텔레포트 타워 간 자유로운 이동을 가능하게 합니다.

## 주요 기능
- 최초 상호작용 시 텔레포트 타워 활성화
- 활성화된 모든 텔레포트 타워 간 자유로운 이동
- 지역 리스트 UI 표시 (동적 크기 조절)
- 페이드 아웃-인 연출
- 2m 이상 멀어지면 UI 자동 닫기
- 현재 위치/비활성화 지역은 리스트에서 제외
- 지역 표시 순서: 베이스캠프 > 미카엘 > 파머 > 달러

## 씬 설정 방법

### 1. TeleporterManager 설정
- 씬에 빈 GameObject를 생성하고 `TeleporterManager` 컴포넌트를 추가합니다.
- 이 오브젝트는 씬에 하나만 존재해야 합니다 (싱글톤).

### 2. TeleportUI 설정
- Canvas에 `TeleportUI` 컴포넌트를 추가합니다.
- 인스펙터에서 다음을 설정합니다:
  - **Region List Panel**: 지역 리스트를 표시할 패널 GameObject
  - **Region List Content**: 버튼들이 생성될 부모 Transform (보통 Vertical Layout Group이 있는 Content)
  - **Region Button Prefab**: 지역 버튼 프리팹 (Button 컴포넌트와 TextMeshProUGUI가 있어야 함)
  - **Cancel Button**: 취소 버튼
  - **Region Names**: 각 지역의 표시 이름 (기본값: 베이스캠프, 파머의 농장, 달러의 동굴, 미카엘의 지옥)

### 3. 텔레포트 타워 설정
각 텔레포트 타워 GameObject에 `Teleporter` 컴포넌트가 있어야 합니다.

#### 베이스캠프 메인 텔레포트 타워:
- **Region**: `BaseCamp` 선택
- **Is Main Teleporter**: 체크
- 이 타워는 기본적으로 활성화되어 있습니다.

#### TRPG 필드 기본 텔레포트 타워:
- **Region**: 해당 지역 선택 (`Farmer`, `Dollar`, `Michael` 중 하나)
- **Is Main Teleporter**: 체크 해제
- 최초 상호작용 시 자동으로 활성화됩니다.

#### 공통 설정:
- **Interaction Range**: 상호작용 가능 거리 (기본값: 2)
- **Teleport Delay**: 텔레포트 지연 시간 (기본값: 0.1)
- **Cooldown Time**: 쿨다운 시간 (기본값: 3)
- **UI Close Distance**: UI를 닫을 거리 (기본값: 2)

### 4. UI 프리팹 구조
지역 버튼 프리팹은 다음과 같은 구조여야 합니다:
```
RegionButton (Button)
  └─ Text (TextMeshProUGUI)
```

## 동작 방식

1. **활성화**: 플레이어가 텔레포트 타워와 상호작용하면 해당 타워가 활성화됩니다.
2. **UI 표시**: 활성화된 타워와 상호작용하면 이동 가능한 지역 리스트가 화면 중앙에 표시됩니다.
3. **이동**: 리스트에서 지역을 선택하면 페이드 아웃 → 이동 → 페이드 인이 실행됩니다.
4. **UI 자동 닫기**: 플레이어가 텔레포트 타워에서 2m 이상 멀어지면 UI가 자동으로 닫힙니다.

## 주의사항

- 베이스캠프 메인 텔레포트 타워는 기본 활성화되어 있습니다.
- 다른 활성화된 타워가 없으면 베이스캠프 메인 텔레포트 타워와 상호작용해도 UI가 표시되지 않습니다.
- 현재 위치한 지역과 비활성화된 지역은 리스트에서 제외됩니다.
- FadeController가 씬에 있어야 페이드 연출이 작동합니다.

## 코드 예시

### 텔레포트 타워 활성화 상태 확인
```csharp
if (TeleporterManager.Instance.IsTeleporterActivated(TeleportRegion.Farmer))
{
    Debug.Log("파머의 농장 텔레포트 타워가 활성화되어 있습니다.");
}
```

### 특정 지역으로 텔레포트
```csharp
Teleporter teleporter = TeleporterManager.Instance.GetTeleporter(TeleportRegion.BaseCamp);
if (teleporter != null)
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    teleporter.TeleportToRegion(TeleportRegion.Farmer, player);
}
```

