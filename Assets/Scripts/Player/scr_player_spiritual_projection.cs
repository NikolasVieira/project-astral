using UnityEngine;

public class scr_player_spiritual_projection : MonoBehaviour {
    public Sprite normalSprite; // Sprite normal do jogador
    public Sprite spiritualSprite; // Sprite da forma espiritual
    public GameObject prefabInertBody; // Corpo que vai ser deixado para tr�s

    private GameObject currentInertBody; // Refer�ncia do corpo inerte
    private SpriteRenderer spriteRenderer; // Refer�ncia do componente da sprite

    private scr_player script_player; // Refer�ncia ao script do jogador

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        script_player = GetComponent<scr_player>(); // Atribui automaticamente, mas verifique se � necess�rio
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
