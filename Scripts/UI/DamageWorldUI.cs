using System.Collections;
using TMPro;
using UnityEngine;

public class DamageWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    private float dmg;

    private void Start()
    {
        damageText.text = dmg.ToString();
        StartCoroutine(DestroyAfterTimePass(2));
    }
    public void Setup(float dmg)
    {
        this.dmg = dmg;
    }

    IEnumerator DestroyAfterTimePass(int time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
