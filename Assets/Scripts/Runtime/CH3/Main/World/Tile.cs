using UnityEngine;

namespace Runtime.CH3.Main
{
	public class Tile : Structure
	{
		private void Reset()
		{
            // Tile은 이동을 막지 않으므로 isBlocking을 false로 설정
			isBlocking = false;
		}

	public override void Initialize(Vector2Int gridPos)
	{
		gridManager = GridSystem.Instance;
		spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
		
		// 그리드 위치 결정
		Vector2Int targetGrid = DetermineGridPosition(gridPos);
		gridPosition = targetGrid;
		
		// 월드 위치 설정 (useCustomY 고려)
		Vector3 worldPos = gridManager.GridToWorldPosition(targetGrid);
		if (useCustomY)
		{
			worldPos.y = customY;
		}
		
		transform.position = worldPos;
	}

	protected override void Start()
	{
		// GridObject.Start()만 호출 (Structure.Start()의 OccupyTiles/BlockTiles 호출 건너뛰기)
		// Tile은 타일을 점유하지 않으므로 Structure의 점유 로직을 실행하지 않음
		if (spriteRenderer == null)
		{
			spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
		}
		
		if (gridManager == null)
		{
			gridManager = GridSystem.Instance;
		}
		
		// Tile은 배경이므로 SortingOrder를 제일 낮은 값으로 고정
		if (spriteRenderer != null)
		{
			spriteRenderer.sortingOrder = int.MinValue;
		}
		
		// 콜리더 제거
		var colliders = GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			if (collider != null)
			{
				if (Application.isPlaying)
					Destroy(collider);
				else
					DestroyImmediate(collider);
			}
		}
		
		// OcclusionFader 제거
		var occlusionFaders = GetComponentsInChildren<Runtime.CH3.Main.Exploration.OcclusionFader>();
		foreach (var fader in occlusionFaders)
		{
			if (fader != null)
			{
				if (Application.isPlaying)
					Destroy(fader);
				else
					DestroyImmediate(fader);
			}
		}
	}
	}
}

