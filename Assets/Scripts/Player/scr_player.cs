using UnityEngine;
using System.Collections;

public class scr_player : MonoBehaviour {
    public float speed; // Velocidade de movimento do jogador
    public LayerMask notWalkableLayer; // Camada que define onde o jogador não pode andar
    public LayerMask fullCollisionLayer; // Camada que define onde o jogador não pode andar mesmo em espirito
    public bool isMoving; // Flag que indica se o jogador está se movendo
    public bool isPushing; // Flag que indica se o jogador está movendo algo
    public bool isSpiritual; // Flag que indica se o jogador está na forma espiritual
    private Vector2 direction; // Direção para onde o jogador está tentando se mover
    public float colliderRadius = 0.5f; // Raio do collider para detecção de obstáculos
    public float boxColliderRadius = 0.4f; // Raio do collider para detecção de obstáculos
    public float hitColliderRadius = 0.3f; // Raio do collider para detecção de obstáculos

    public scr_player_spiritual_projection script_player_spiritual_projection; // Referência ao script de projeção espiritual do jogador

    private Vector3 targetPos;

    void Update() {
        // Se o jogador não estiver se movendo e não estiver empurrando uma caixa
        if (!isMoving && !isPushing) {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Permite o movimento apenas em 4 direções (horizontal ou vertical)
            if (direction.x != 0) direction.y = 0;

            // Se a direção for diferente de zero
            if (direction != Vector2.zero) {
                // A posição que o personagem vai ser movido é a soma da direção mais a posição atual
                targetPos = transform.position + new Vector3(direction.x, direction.y, 0);

                if (CanPushBox(targetPos)) {
                    TryPushBox(targetPos); // Tenta empurrar uma caixa se houver uma na posição alvo
                } else if ((IsWalkable(targetPos) || isSpiritual) && isFullCollision(targetPos)) {
                    StartCoroutine(Move(targetPos)); // Inicia a coroutine para mover o jogador
                }
            }
        }
    }

    IEnumerator Move(Vector3 targetPos) {
        isMoving = true;

        // Move o jogador até a posição alvo
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        isPushing = false; // Reseta isPushing para false quando o movimento termina
    }

    private bool IsWalkable(Vector3 targetPos) {
        // Verifica se a posição alvo é caminhável (não há colisão com notWalkableLayer)
        return Physics2D.OverlapCircle(targetPos, colliderRadius, notWalkableLayer) == null;
    }
    private bool isFullCollision(Vector3 targetPos) {
        // Verifica se a posição alvo é caminhável (não há colisão com notWalkableLayer)
        return Physics2D.OverlapCircle(targetPos, colliderRadius, fullCollisionLayer) == null;
    }

    private bool CanPushBox(Vector3 targetPos) {
        // Verifica se há uma caixa empurrável na posição alvo
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPos, hitColliderRadius, 1 << LayerMask.NameToLayer("PushableBox"));

        if (hitCollider != null) {
            return hitCollider.CompareTag("PushableBox");
        }
        return false;
    }

    private void TryPushBox(Vector3 targetPos) {
        Collider2D boxCollider = Physics2D.OverlapCircle(targetPos, boxColliderRadius, 1 << LayerMask.NameToLayer("PushableBox"));

        if (boxCollider != null) {
            scr_crate crateScript = boxCollider.GetComponent<scr_crate>();
            if (crateScript != null) {
                // Verifica se a caixa não está sendo empurrada
                if (!crateScript.isBeingPushed) {
                    Vector2 pushDirection = new Vector2(direction.x, direction.y);
                    Vector3 newBoxPos = boxCollider.transform.position + new Vector3(pushDirection.x, pushDirection.y, 0);

                    // Verifica se o jogador pode empurrar a caixa (caminhável ou espiritual e não bloqueado)
                    if (IsWalkable(newBoxPos)) {
                        if (crateScript.CanBePushed(pushDirection)) {
                            crateScript.Push(pushDirection); // Chama o método Push da caixa
                            isPushing = true; // Define isPushing como true quando o jogador empurra a caixa
                            StartCoroutine(Move(transform.position + new Vector3(direction.x, direction.y, 0)));
                        }
                    }
                }
            }
        }
    }
}
