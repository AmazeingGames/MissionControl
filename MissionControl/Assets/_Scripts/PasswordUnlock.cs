using UnityEngine;

public class PasswordUnlock : MonoBehaviour
{
    [SerializeField] GameObject lockScreen;
    [SerializeField] GameObject unlockScreen;

    [SerializeField] string password;

    public void ReadInput(string input)
    {
        if (input == password)
        {
            lockScreen.SetActive(false);
            unlockScreen.SetActive(true);
        }
    }

}
