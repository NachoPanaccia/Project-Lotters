using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MenuNameManager : MonoBehaviour
{

    [SerializeField] private InputField nicknameInput;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Text errorText;

    private Coroutine mensajeErrorCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("nickname"))
        {
            nicknameInput.text = PlayerPrefs.GetString("nickname");
        }
        confirmButton.onClick.AddListener(SetNickname);
        errorText.text = "";
    }

    private void SetNickname()
    {
        string nickname = nicknameInput.text.Trim();

        string[] nombresProhibidos = { "admin", "administrador", "Admin", "Administrador"};

        foreach (string prohibido in nombresProhibidos)
        {
            if (nickname.Equals(prohibido, System.StringComparison.OrdinalIgnoreCase))
            {
                MostrarError("Ese nickname est� reservado y no puede ser usado.");
                return;
            }
        }

        if (string.IsNullOrEmpty(nickname))
        {
            MostrarError("El nickname no puede estar vac�o.");
            return;
        }

        PhotonNetwork.NickName = nickname;
        PlayerPrefs.SetString("nickname", nickname);
        errorText.text = "";
        Debug.Log("Nickname seteado correctamente: " + nickname);
    }

    private void MostrarError(string mensaje)
    {
        if (mensajeErrorCoroutine != null)
        {
            StopCoroutine(mensajeErrorCoroutine);
        }

        errorText.text = mensaje;
        mensajeErrorCoroutine = StartCoroutine(LimpiarMensajeErrorLuegoDeTiempo(3f));
    }
    
    private IEnumerator LimpiarMensajeErrorLuegoDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        errorText.text = "";
        mensajeErrorCoroutine = null;
    }
}
