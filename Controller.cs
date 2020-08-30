using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour
{
    // publics
    public TMP_Dropdown dropdown;
    public TMP_Text follow_text;
    public GameObject follow_ui;
    // globals
    private List<GameObject> buildings = new List<GameObject>();
    private Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
    private int current_system = 0;
    Ray ray; RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        // get Data
        ReadInFile("data.txt");

        // load all buildings
        for (int i = 0; i < transform.childCount; i++)
        {
            buildings.Add(transform.GetChild(i).gameObject);
        }

        // Initialize with totals.
        ColourBuildings();

        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged();
            ColourBuildings();
        });
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Building") {
                follow_text.text = hit.collider.name + ": " + data[hit.collider.name][current_system];
                follow_ui.transform.position = Input.mousePosition;
                follow_ui.SetActive(true);
            } else {
                follow_ui.SetActive(false);
            }
        }
        
    }

    void DropdownValueChanged()
    {
        current_system = dropdown.value;
    }

    void ColourBuildings() {
        float max = -1;
        foreach (KeyValuePair<string, List<int>> entry in data) {
            int count = 0;
            foreach (int item in entry.Value)
            {
                if (count == current_system) {
                    if (max < item) { max = item; }
                }
                count++;
            }
        }
        foreach (var building in buildings) {
            Debug.Log(building.name);
            building.GetComponent<Renderer>().material.color = parseColour((-(data[building.name][current_system] / max) + 1));
        }
    }


    Color parseColour(float color) {
        switch (current_system)
        {
            case 0: // total - gray
                return new Color(color, color, color); 
            case 1: // visual - blue
                return new Color(0f, 0f, color);
            case 2: // acoustic - green
                return new Color(0f, color, 0f);
            case 3: // IAQ - orange
                return new Color(color, color * 0.6f, 0f);
            case 4: // FUNC - purple
                return new Color(color * 0.6f, 0f, color * 0.6f);
            case 5: // COND - yellow
                return new Color(color, color, 0f);
            case 6: // thermal - red
                return new Color(color, 0f, 0f);
            default:
                return new Color(0f, 0f, 0f);
        }
    }

    void ReadInFile(string filename) {
        var sr = new StreamReader(Application.dataPath + "/" + filename);
        var fileContents = sr.ReadToEnd();
        sr.Close();
        var lines = fileContents.Split("\n"[0]);
        foreach (var line in lines)
        {
            string[] temp = line.Split(',');
            string building = "";
            List<int> temp_list = new List<int>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (i == 0) {
                    building = temp[i];
                } else {
                    temp_list.Add(int.Parse(temp[i]));
                }
            }
            data.Add(building, temp_list);
        }
    }
}
