using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameInitializer : MonoBehaviourPunCallbacks
{
    [Header("Prefabs por rol")]
    [SerializeField] private GameObject prefabPolicia;
    [SerializeField] private GameObject[] prefabsLadrones; // �ndice 0 = Ladr�n 1, etc.

    [Header("Puntos de aparici�n")]
    [SerializeField] private Transform spawnPolicia;
    [SerializeField] private Transform[] spawnsLadrones;

    private void Start()
    {
        int slot = PlayerPrefs.GetInt("MiSlot", -99);
        Debug.Log($"[GameInitializer] Slot cargado: {slot}");

        switch (slot)
        {
            case -1:
                Debug.Log("Instanciando POLIC�A");
                PhotonNetwork.Instantiate(prefabPolicia.name, spawnPolicia.position, Quaternion.identity);
                break;

            case 0:
            case 1:
            case 2:
                Debug.Log($"Instanciando LADR�N {slot + 1}");
                PhotonNetwork.Instantiate(prefabsLadrones[slot].name, spawnsLadrones[slot].position, Quaternion.identity);
                break;

            default:
                Debug.LogWarning("Slot inv�lido. No se puede instanciar jugador.");
                break;
        }
    }
}
