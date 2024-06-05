using System.Collections;
using UnityEngine;

public class scr_crate : MonoBehaviour {
    public float speed;
    public LayerMask notWalkableLayer;
    public bool isBeingPushed = false;
    private Vector2 pushDirection;

    void Update() {
        if (pushDirection != Vector2.zero && !isBeingPushed) {
            Vector3 targetPos = transform.position + new Vector3(pushDirection.x, pushDirection.y, 0);

            if (CanBePushed(pushDirection)) {
                StartCoroutine(Move(targetPos));
            }
            pushDirection = Vector2.zero;
        }
    }

    public bool CanBePushed(Vector2 direction) {
        Vector3 newBoxPos = transform.position + new Vector3(direction.x, direction.y, 0);

        // Verificar se a nova posição da caixa é caminhável
        return IsWalkable(newBoxPos);
    }

    public void Push(Vector2 direction) {
        if (!isBeingPushed) {
            pushDirection = direction;
        }
    }

    IEnumerator Move(Vector3 targetPos) {
        isBeingPushed = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isBeingPushed = false;
    }

    private bool IsWalkable(Vector3 targetPos) {
        return Physics2D.OverlapCircle(targetPos, 0, notWalkableLayer) == null;
    }
}
