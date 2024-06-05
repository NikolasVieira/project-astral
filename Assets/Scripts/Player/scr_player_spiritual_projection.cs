using UnityEngine;

public class scr_player_spiritual_projection : MonoBehaviour {
    public Sprite normalSprite; // Sprite normal do jogador
    public Sprite spiritualSprite; // Sprite da forma espiritual
    public GameObject prefabInertBody; // Corpo que vai ser deixado para trás

    private GameObject currentInertBody; // Referência do corpo inerte
    private SpriteRenderer spriteRenderer; // Referência do componente da sprite

    private scr_player script_player; // Referência ao script do jogador

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        script_player = GetComponent<scr_player>(); // Atribui automaticamente, mas verifique se é necessário
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !script_player.isMoving) {
            if (currentInertBody == null)
                MakeAstralProjection();
            else
                BodyReturn();
        }
    }

    void MakeAstralProjection() {
        currentInertBody = Instantiate(prefabInertBody, transform.position, Quaternion.identity);
        spriteRenderer.sprite = spiritualSprite;

        // Habilita a capacidade de atravessar paredes e empurrar caixas
        script_player.isSpiritual = true;
    }

    void BodyReturn() {
        if (currentInertBody != null) {
            transform.position = currentInertBody.transform.position;
            spriteRenderer.sprite = normalSprite;
            Destroy(currentInertBody);
            currentInertBody = null;

            // Desabilita a capacidade de atravessar paredes e empurrar caixas
            script_player.isSpiritual = false;
        }
    }
}
