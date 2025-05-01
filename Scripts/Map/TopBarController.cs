// using UnityEngine;
// using TMPro;

// public class TopBarController : MonoBehaviour {
//     [SerializeField] private TextMeshProUGUI hpText;
//     [SerializeField] private TextMeshProUGUI goldText;
//     [SerializeField] private TextMeshProUGUI soulsText;

//     private void Start() {
//         UpdateUI();
//     }

//     public void UpdateUI() {
//         hpText.text = $"{GameManager.Instance.playerHP}";
//         goldText.text = $"{GameManager.Instance.playerGold}";
//         soulsText.text = $"{GameManager.Instance.playerSouls}";
//     }
// }