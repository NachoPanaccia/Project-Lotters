using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbySlotManager : MonoBehaviourPunCallbacks
{
    [Header("Textos de slots")]
    [SerializeField] private Text policeSlot;
    [SerializeField] private Text[] thiefSlots;

    // Se mapea jugador a �ndice de slot (-1 = polic�a, 0-2 = ladrones)
    private Dictionary<Player, int> jugadorASlot = new Dictionary<Player, int>();

    private void Start()
    {
        LimpiarSlots();
    }

    private void LimpiarSlots()
    {
        policeSlot.text = "POLIC�A";
        foreach (Text t in thiefSlots)
        {
            t.text = "LADR�N";
        }
        jugadorASlot.Clear();
    }

    public void AsignarJugador(Player player)
    {
        if (jugadorASlot.ContainsKey(player))
            return;
        
        if (!jugadorASlot.ContainsValue(-1))
        {
            policeSlot.text = player.NickName;
            jugadorASlot[player] = -1;
            return;
        }
        
        for (int i = 0; i < thiefSlots.Length; i++)
        {
            if (!jugadorASlot.ContainsValue(i))
            {
                thiefSlots[i].text = player.NickName;
                jugadorASlot[player] = i;
                return;
            }
        }

        Debug.LogWarning("No hay slots disponibles para el jugador " + player.NickName);
    }

    public void RemoverJugador(Player player)
    {
        if (!jugadorASlot.ContainsKey(player))
            return;

        int slot = jugadorASlot[player];
        if (slot == -1)
        {
            policeSlot.text = "POLIC�A";
        }
        else
        {
            thiefSlots[slot].text = "LADR�N";
        }

        jugadorASlot.Remove(player);
    }
}