using UnityEngine;

namespace Runtime.CH3.Main
{
	public class BlockedArea : Structure
	{
		private void Reset()
		{
			isBlocking = true;
		}

		private void OnDrawGizmos()
		{
			// BlockedArea는 빨간색으로 표시
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(transform.position + Vector3.up * 0.05f, new Vector3(0.9f, 0.1f, 0.9f));
			
			// 중앙에 X 표시
			Gizmos.color = Color.red;
			Vector3 center = transform.position + Vector3.up * 0.1f;
			float size = 0.3f;
			Gizmos.DrawLine(center + new Vector3(-size, 0, -size), center + new Vector3(size, 0, size));
			Gizmos.DrawLine(center + new Vector3(-size, 0, size), center + new Vector3(size, 0, -size));
		}
	}
}


