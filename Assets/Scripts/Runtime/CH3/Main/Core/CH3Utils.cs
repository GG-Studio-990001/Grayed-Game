using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// CH3 공통 유틸리티 함수
    /// </summary>
    public static class CH3Utils
    {
        /// <summary>
        /// 대소문자 구분 없이 자식 오브젝트를 찾습니다.
        /// </summary>
        public static Transform FindChildByNameIgnoreCase(Transform parent, string name)
        {
            if (parent == null || string.IsNullOrEmpty(name)) return null;
            
            Transform child = parent.Find(name);
            if (child != null) return child;

            foreach (Transform t in parent)
            {
                if (t.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return t;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 대소문자 구분 없이 재귀적으로 자식 오브젝트를 찾습니다.
        /// </summary>
        public static Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent == null || string.IsNullOrEmpty(name)) return null;
            
            foreach (Transform child in parent)
            {
                if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }
                
                Transform found = FindChildRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 스프라이트에 맞게 콜라이더 크기를 업데이트합니다.
        /// </summary>
        public static void UpdateColliderToSprite(BoxCollider collider, SpriteRenderer spriteRenderer)
        {
            if (collider == null || spriteRenderer == null) return;
            
            if (spriteRenderer.sprite != null)
            {
                Bounds spriteBounds = spriteRenderer.sprite.bounds;
                collider.size = spriteBounds.size;
                collider.center = spriteBounds.center;
            }
        }
        
        /// <summary>
        /// GridObject의 타입에 따라 콜라이더의 isTrigger를 설정합니다.
        /// </summary>
        public static void SetColliderTriggerByGridObject(BoxCollider collider, GridObject gridObject, CH3_LevelData data)
        {
            if (collider == null || gridObject == null) return;
            
            var interactable = gridObject as InteractableGridObject;
            var structure = gridObject as Structure;
            var producer = gridObject as Producer;
            
            collider.isTrigger = !(interactable != null || producer != null || (structure != null && data != null && data.isBlocking));
        }
        
        /// <summary>
        /// Transform에서 BoxCollider를 가져오거나 추가합니다.
        /// </summary>
        public static BoxCollider GetOrAddBoxCollider(Transform transform)
        {
            if (transform == null) return null;
            
            BoxCollider collider = transform.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = transform.gameObject.AddComponent<BoxCollider>();
            }
            return collider;
        }
    }
}

