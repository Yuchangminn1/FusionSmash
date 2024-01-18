using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Animations.SpringBones.GameObjectExtensions;
using UnityEngine;
using UnityEngine.UI;

public class KillLog : MonoBehaviour
{
    TMP_Text _killPlayerText;
    TMP_Text _deadPlayerText;
    Image _weaponImage;
    string _killPlayerName = "KillPlayer";
    string _deadPlayerName = "DeadPlayer";

    // Start is called before the first frame update
    private void Awake()
    {
        _killPlayerText = gameObject.FindChildByName(_killPlayerName).GetComponent<TMP_Text>();
        _deadPlayerText = gameObject.FindChildByName(_deadPlayerName).GetComponent<TMP_Text>();

        _weaponImage = GetComponentInChildren<Image>();

    }
    public void SetLog(string killPlayer, string deadPlayer, Sprite weapon)
    {
        if(killPlayer == "")
        {
            killPlayer = "닉네임";
        }
        if (deadPlayer == "")
        {
            deadPlayer = "닉네임";
        }
        _killPlayerText.text = killPlayer;
        _deadPlayerText.text = deadPlayer;
        _weaponImage.sprite = weapon;
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        _killPlayerText.color = Color.black;
        _deadPlayerText.color = Color.black;
        _weaponImage.color = Color.white;

        yield return new WaitForSeconds(3f);
        int i = 0;
        int goal = 100;
        Color mirror = new Color(0, 0, 0, 0);
        while (i < goal)
        {
            ++i;
            _killPlayerText.color = Color.Lerp(_killPlayerText.color, mirror, 0.25f);
            _deadPlayerText.color = Color.Lerp(_deadPlayerText.color, mirror, 0.25f);
            _weaponImage.color = Color.Lerp(_weaponImage.color, mirror, 0.25f);
            yield return new WaitForSeconds(0.01f);

        }
        Destroy(gameObject,0.1f);
    }
}
