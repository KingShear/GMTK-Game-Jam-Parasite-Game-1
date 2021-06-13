using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDiceTarget : MonoBehaviour
{
    public Text target_number_text;
    public Text target_number_text_backing;

    // Start is called before the first frame update
    void Start()
    {
        int rand_int_target = Random.Range(1, 7);
        target_number_text.text = rand_int_target.ToString();
        target_number_text_backing.text = rand_int_target.ToString();
    }
}
