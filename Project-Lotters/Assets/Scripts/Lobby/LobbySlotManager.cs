using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbySlotManager : MonoBehaviourPunCallbacks
{
    [Header("Referencias visuales")]
    [SerializeField] private Text policeSlot;
    [SerializeField] private Text[] thiefSlots;

    [Header("Botones de cambio de slot")]
    [SerializeField] private SlotChangeButton[] slotButtons;

    private Dictionary<int, (int slot, string nickname)> estadoSlots = new Dictionary<int, (int, string)>();

    private void Start()
    {
        LimpiarSlotsVisuales();
    }

    public override void OnJoinedRoom()
    {
        photonView.RPC("RPC_EnviarNicknameAlMaster", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (estadoSlots.ContainsKey(otherPlayer.ActorNumber))
        {
            estadoSlots.Remove(otherPlayer.ActorNumber);
            ActualizarVisualGlobal();
        }
    }

    [PunRPC]
    private void RPC_EnviarNicknameAlMaster(string nickname, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player player = info.Sender;
        int actorNumber = player.ActorNumber;

        if (estadoSlots.ContainsKey(actorNumber))
            return;

        int slot = AsignarSlotDisponible();
        if (slot == int.MinValue)
        {
            Debug.LogWarning("No hay slots disponibles para " + nickname);
            return;
        }

        estadoSlots[actorNumber] = (slot, nickname);
        ActualizarVisualGlobal();
    }

    private int AsignarSlotDisponible()
    {
        if (!ExisteSlotOcupado(-1)) return -1;
        for (int i = 0; i < thiefSlots.Length; i++)
        {
            if (!ExisteSlotOcupado(i)) return i;
        }
        return int.MinValue;
    }

    private bool ExisteSlotOcupado(int slot)
    {
        foreach (var kvp in estadoSlots)
        {
            if (kvp.Value.slot == slot) return true;
        }
        return false;
    }

    private void LimpiarSlotsVisuales()
    {
        policeSlot.text = "POLICÍA";
        foreach (Text t in thiefSlots) t.text = "LADRÓN";
    }

    private void ActualizarVisualGlobal()
    {
        // Limpiar todos los textos visuales
        policeSlot.text = "POLICÍA";
        foreach (Text t in thiefSlots) t.text = "LADRÓN";

        // Aplicar nombres actualizados
        foreach (var kvp in estadoSlots)
        {
            photonView.RPC("RPC_ActualizarSlotVisual", RpcTarget.All, kvp.Value.slot, kvp.Value.nickname);
        }

        // Calcular ocupación y actualizar botones
        if (PhotonNetwork.IsMasterClient)
        {
            bool[] ocupacion = new bool[4]; // 0 = policía, 1-3 = ladrones
            foreach (var entry in estadoSlots.Values)
            {
                int index = entry.slot == -1 ? 0 : entry.slot + 1;
                ocupacion[index] = true;
            }

            photonView.RPC("RPC_ActualizarEstadoBotones", RpcTarget.All, ocupacion);
        }
    }

    [PunRPC]
    private void RPC_ActualizarSlotVisual(int slot, string nickname)
    {
        if (slot == -1) policeSlot.text = nickname;
        else if (slot >= 0 && slot < thiefSlots.Length) thiefSlots[slot].text = nickname;
    }

    [PunRPC]
    private void RPC_ActualizarEstadoBotones(bool[] ocupacion)
    {
        Debug.Log("=== ACTUALIZACIÓN VISUAL DE BOTONES ===");

        foreach (var boton in slotButtons)
        {
            int slot = boton.GetTargetSlot();
            int index = slot == -1 ? 0 : slot + 1;
            bool visible = !ocupacion[index];

            Debug.Log($"Botón para slot {slot} --> visible: {visible}");
            boton.SetInteractable(visible);
        }
    }

    [PunRPC]
    private void RPC_SolicitarCambioDeSlot(int actorNumberSolicitante, int slotDeseado)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var kvp in estadoSlots)
        {
            if (kvp.Value.slot == slotDeseado)
                return;
        }

        if (!estadoSlots.ContainsKey(actorNumberSolicitante))
            return;

        string nickname = estadoSlots[actorNumberSolicitante].nickname;
        int slotAnterior = estadoSlots[actorNumberSolicitante].slot;

        estadoSlots[actorNumberSolicitante] = (slotDeseado, nickname);

        // Ahora simplemente actualizamos la visual completa
        ActualizarVisualGlobal();
    }

    [PunRPC]
    private void RPC_LimpiarSlot(int slot)
    {
        if (slot == -1)
            policeSlot.text = "POLICÍA";
        else if (slot >= 0 && slot < thiefSlots.Length)
            thiefSlots[slot].text = "LADRÓN";
    }
}