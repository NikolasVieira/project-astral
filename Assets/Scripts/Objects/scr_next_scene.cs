using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_next_scene : MonoBehaviour {
    public int sceneIndex;
    private scr_player script_player; // Referência ao script do jogador

    private void Start() {
        // Obtém a referência ao script scr_player no jogador
        script_player = FindObjectOfType<scr_player>();

        if (script_player == null) {
            Debug.LogError("Script scr_player não encontrado!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Verifica se script_player não é nulo antes de acessá-lo
        if (script_player != null && !script_player.isSpiritual) {
            Debug.Log("O jogador não está em modo espiritual. Carregando cena...");
            SceneManager.LoadScene(sceneIndex);
        } else {
            Debug.Log("O jogador está em modo espiritual. Não carregando cena.");
        }
    }
}
