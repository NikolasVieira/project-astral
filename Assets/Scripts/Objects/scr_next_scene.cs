using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_next_scene : MonoBehaviour {
    public int sceneIndex;
    private scr_player script_player; // Refer�ncia ao script do jogador

    private void Start() {
        // Obt�m a refer�ncia ao script scr_player no jogador
        script_player = FindObjectOfType<scr_player>();

        if (script_player == null) {
            Debug.LogError("Script scr_player n�o encontrado!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Verifica se script_player n�o � nulo antes de acess�-lo
        if (script_player != null && !script_player.isSpiritual) {
            Debug.Log("O jogador n�o est� em modo espiritual. Carregando cena...");
            SceneManager.LoadScene(sceneIndex);
        } else {
            Debug.Log("O jogador est� em modo espiritual. N�o carregando cena.");
        }
    }
}
