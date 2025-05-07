using UnityEngine;
using TMPro;
using Snowy.UI;
public class MissionInfoUI : MonoBehaviour
{
    public TextMeshProUGUI MissionName;
    public TextMeshProUGUI MissionReward;
    public SnButton ApplyButton;


    public void Init(string name, MissionData data, int reward = 3,GameObject panel = null)
    {
        MissionName.text = name;
        MissionReward.text = reward.ToString("0H");
        ApplyButton.OnClick.AddListener(() =>
        {
            ElevatorTeleporter.Instance.SetMissionData(data);
            if(panel != null) panel.SetActive(false);
        });
    }
}
