using UnityEngine;
using System.Collections;
using Andoco.Unity.ProcArch;
using System.Text.RegularExpressions;

public class NodeMeshModifier : MonoBehaviour {

    public ArchitectureController architectureController;
    public string searchPattern;
    public Color color = Color.white;

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            var architecture = this.architectureController.CurrentArchitecture;
            
            var cArr = architecture.Mesh.colors;
            
            foreach (var item in architecture.MeshData)
            {
                Debug.Log(item.Key);
                if (Regex.IsMatch(item.Key, this.searchPattern))
                {
                    for (var i=item.Value.ColorsStart; i < item.Value.ColorsEnd; i++)
                    {
                        cArr[i] = color;
                    }
                }
            }
            
            architecture.Mesh.colors = cArr;
        }
    }
}
