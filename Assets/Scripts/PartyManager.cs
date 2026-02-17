<<<<<<< HEAD
using UnityEngine;
using System.Collections.Generic; // List [Need]

public class PartyManager : MonoBehaviour
{
    [SerializeField] private List<Character> members = new List<Character>();
    public List<Character> Members { get { return members; } }


    [SerializeField] private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars { get { return selectChars; } }

    public static PartyManager instance;


    void Awake() 
    {
        instance = this;
    }
=======
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars { get { return selectChars; } }
    

    public static PartyManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
>>>>>>> c17e185ca2ac52f5962968a9ab73b1617dcc89b6
}
