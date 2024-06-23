using UnityEngine;
using System.Collections;

public class scr_player : MonoBehaviour {
    
    //FLAGS
    public bool isMoving; // Flag que indica se o jogador esta se movendo
    public bool isPushing; // Flag que indica se o jogador esta empurrando algo
    public bool isSpiritual; // Flag que indica se o jogador esta na forma espiritual
    //VALORES FLUTUANTES
    public float speed; // Velocidade de movimento do jogador
    //MELHORIA FUTURA: Diminuir as variaveis abaixos para uma unica
    public float colliderRadius = 0.5f; // Raio do collider para detecção de obstaculos
    public float boxColliderRadius = 0.4f; // Raio do collider para detecção de obstaculos
    public float hitColliderRadius = 0.3f; // Raio do collider para detecção de obstaculos
    //LAYERS
    public LayerMask notWalkableLayer; // Camada que define onde o jogador não pode andar
    public LayerMask fullCollisionLayer; // Camada que define onde o jogador não pode andar mesmo em espirito
    //VETORES
    private Vector2 direction; // Direçao para onde o jogador esta tentando se mover
    private Vector3 targetPos; // Espaco que o jogador vai ficar depois de andar
    //SCRIPTS
    public scr_player_spiritual_projection script_player_spiritual_projection; // Referência ao script de projeção espiritual do jogador

    void Update() {
        // Jogador não esta se movendo E não esta empurrando uma caixa?
        if (!isMoving && !isPushing) {
            //Direcao recebe valor das entradas horizontais e verticais
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Permite o movimento apenas em 4 direções (horizontal ou vertical)
            if (direction.x != 0) direction.y = 0;

            // Direcao diferente de zero em X e Y
            if (direction != Vector2.zero) {
                // A posição que o jogador vai ser movido é a soma da posicao atual mais a direcao
                targetPos = transform.position + new Vector3(direction.x, direction.y, 0);

                // Jogador pode mover a caixa?
                if (CanPushBox(targetPos)) {
                    // Tenta empurrar uma caixa se houver uma na posicao alvo
                    TryPushBox(targetPos); 
                } 
                // Jogador pode se mover para a posicao alvo OU esta na forma espiritual E nao e a parede com colisao maxima?
                else if ((IsWalkable(targetPos) || isSpiritual) && isFullCollision(targetPos)) {
                    // Inicia a coroutine para mover o jogador
                    StartCoroutine(Move(targetPos)); 
                }
            }
        }
    }

    // Coroutine que move o jogador para a posicao do parametro
    IEnumerator Move(Vector3 targetPos) {
        isMoving = true; // Ativa a flag de movimento

        // Enquanto a distancia entre o personagem e o destino for maior que 0 (aproximadamente), executa
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            // Muda a posicao atual do personagem para mais perto do destino a uma velocidade definida
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            // Espera o proximo frame para entrar no loop novamente
            yield return null;
        }
     
        transform.position = targetPos; //Atribui a posicao alvo para a posicao do personagem para evitar numeros quebrados
        isMoving = false; // Desativa a flag de movimento
        isPushing = false; // Desativa a flag que indica que o personagem esta empurrando algo, usado para evitar movimentacao indesejada
    }

    // MELHORIA FUTURA: Mesclar os dois metodos abaixo (IsWalkable E isFullCollision) para evitar codigo duplicado
    //Funcao que retornar um valor booleano caso o personagem possa se mover para a posicao alvo
    private bool IsWalkable(Vector3 targetPos) {
        // Verifica se a posição alvo é caminhável (não há colisão com notWalkableLayer)
        return Physics2D.OverlapCircle(targetPos, colliderRadius, notWalkableLayer) == null;
    }
    //Funcao que retornar um valor booleano caso o personagem possa se mover para a posicao alvo
    private bool isFullCollision(Vector3 targetPos) {
        // Verifica se a posição alvo é caminhável (não há colisão com fullCollisionLayer)
        return Physics2D.OverlapCircle(targetPos, colliderRadius, fullCollisionLayer) == null;
    }

    // MELHORIA FUTURA: Usar somente a layer ou a tag para as verificacoes
    private bool CanPushBox(Vector3 targetPos) {
        // Atribui as informacoes de qualquer objeto na layer PushableBox na posição alvo
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPos, hitColliderRadius, 1 << LayerMask.NameToLayer("PushableBox"));

        // Existe alguma informacao na variavel hitCollider?
        if (hitCollider != null) {
            // Faz a funcao retornar TRUE se o objeto do collider tem a tag PushableBox, ou FALSE se tiver outra tag
            return hitCollider.CompareTag("PushableBox");
        }
        return false; // Retorna FALSE caso nao tenha informacoes na variavel hitCollider
    }

    // Metodo que tenta empurar caixas que estao na posicao passada no parametro
    private void TryPushBox(Vector3 targetPos) {
        // Atribui as informacoes de qualquer objeto na layer PushableBox na posição alvo
        Collider2D boxCollider = Physics2D.OverlapCircle(targetPos, boxColliderRadius, 1 << LayerMask.NameToLayer("PushableBox"));

        // Existe alguma informacao na variavel boxCollider?
        if (boxCollider != null) {
            // Recebe as informacoes do script do objeto pai do collider
            scr_crate crateScript = boxCollider.GetComponent<scr_crate>();
            // Existe algum script no componente?
            if (crateScript != null) {
                // Verifica se a caixa não está sendo empurrada
                if (!crateScript.isBeingPushed) {
                    // Define a direcao que a caixa vai ser empurrada com base no valor das entradas horizontais e verticais
                    Vector2 pushDirection = new Vector2(direction.x, direction.y);
                    // Define a posicao que a caixa vai ficar apos o empurrao, que é a soma da posicao atual mais a direcao do empurrao
                    Vector3 newBoxPos = boxCollider.transform.position + new Vector3(pushDirection.x, pushDirection.y, 0);

                    // Verifica se a posicao que a caixa vai ficar nao e uma parede
                    if (IsWalkable(newBoxPos)) {
                        // Verifica se o script consegue empurrar a caixa
                        if (crateScript.CanBePushed(pushDirection)) {
                            crateScript.Push(pushDirection); // Chama o método Push da caixa
                            isPushing = true; // Atva a flag que indica que o personagem esta empurrando algo
                            // Inicia a coroutine para mover o jogador para onde a caixa estava
                            StartCoroutine(Move(transform.position + new Vector3(direction.x, direction.y, 0)));
                        }
                    }
                }
            }
        }
    }
}
